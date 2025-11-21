define(function(require) {

    var zrUtil = require('zrender/core/util');
    var Component = require('../../Dto/Component');

    require('./AxisDto');

    Component.extend({

        type: 'parallel',

        dependencies: ['parallelAxis'],

        /**
         * @type {module:echarts/coord/parallel/Parallel}
         */
        coordinateSystem: null,

        /**
         * Each item like: 'dim0', 'dim1', 'dim2', ...
         * @type {Array.<string>}
         * @readOnly
         */
        dimensions: null,

        /**
         * Coresponding to dimensions.
         * @type {Array.<number>}
         * @readOnly
         */
        parallelAxisIndex: null,

        defaultOption: {
            zlevel: 0,                  // 一级层叠
            z: 0,                       // 二级层叠
            left: 80,
            top: 60,
            right: 80,
            bottom: 60,
            // width: {totalWidth} - left - right,
            // height: {totalHeight} - top - bottom,

            layout: 'horizontal',      // 'horizontal' or 'vertical'

            parallelAxisDefault: null
        },

        /**
         * @override
         */
        init: function () {
            Component.prototype.init.apply(this, arguments);

            this.mergeOption({});
        },

        /**
         * @override
         */
        mergeOption: function (newOption) {
            var thisOption = this.option;

            newOption && zrUtil.merge(thisOption, newOption, true);

            this._initDimensions();
        },

        /**
         * Whether series or axis is in this coordinate system.
         * @param {module:echarts/Dto/Series|module:echarts/coord/parallel/AxisDto} Dto
         * @param {module:echarts/Dto/Global} ecDto
         */
        contains: function (Dto, ecDto) {
            var parallelIndex = Dto.get('parallelIndex');
            return parallelIndex != null
                && ecDto.getComponent('parallel', parallelIndex) === this;
        },

        /**
         * @private
         */
        _initDimensions: function () {
            var dimensions = this.dimensions = [];
            var parallelAxisIndex = this.parallelAxisIndex = [];

            var axisDtos = zrUtil.filter(this.dependentDtos.parallelAxis, function (axisDto) {
                // Can not use this.contains here, because
                // initialization has not been completed yet.
                return axisDto.get('parallelIndex') === this.componentIndex;
            });

            zrUtil.each(axisDtos, function (axisDto) {
                dimensions.push('dim' + axisDto.get('dim'));
                parallelAxisIndex.push(axisDto.componentIndex);
            });
        }

    });

});