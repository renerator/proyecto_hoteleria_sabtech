define(function (require) {

    var axisDefault = require('./axisDefault');
    var zrUtil = require('zrender/core/util');
    var ComponentDto = require('../Dto/Component');
    var layout = require('../util/layout');

    // FIXME axisType is fixed ?
    var AXIS_TYPES = ['value', 'category', 'time', 'log'];

    /**
     * Generate sub axis Dto class
     * @param {string} axisName 'x' 'y' 'radius' 'angle' 'parallel'
     * @param {module:echarts/Dto/Component} BaseAxisDtoClass
     * @param {Function} axisTypeDefaulter
     * @param {Object} [extraDefaultOption]
     */
    return function (axisName, BaseAxisDtoClass, axisTypeDefaulter, extraDefaultOption) {

        zrUtil.each(AXIS_TYPES, function (axisType) {

            BaseAxisDtoClass.extend({

                type: axisName + 'Axis.' + axisType,

                mergeDefaultAndTheme: function (option, ecDto) {
                    var layoutMode = this.layoutMode;
                    var inputPositionParams = layoutMode
                        ? layout.getLayoutParams(option) : {};

                    var themeDto = ecDto.getTheme();
                    zrUtil.merge(option, themeDto.get(axisType + 'Axis'));
                    zrUtil.merge(option, this.getDefaultOption());

                    option.type = axisTypeDefaulter(axisName, option);

                    if (layoutMode) {
                        layout.mergeLayoutParam(option, inputPositionParams, layoutMode);
                    }
                },

                defaultOption: zrUtil.mergeAll(
                    [
                        {},
                        axisDefault[axisType + 'Axis'],
                        extraDefaultOption
                    ],
                    true
                )
            });
        });

        ComponentDto.registerSubTypeDefaulter(
            axisName + 'Axis',
            zrUtil.curry(axisTypeDefaulter, axisName)
        );
    };
});