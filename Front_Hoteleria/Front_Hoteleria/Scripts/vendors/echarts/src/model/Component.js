/**
 * Component Dto
 *
 * @module echarts/Dto/Component
 */
define(function(require) {

    var Dto = require('./Dto');
    var zrUtil = require('zrender/core/util');
    var arrayPush = Array.prototype.push;
    var componentUtil = require('../util/component');
    var clazzUtil = require('../util/clazz');
    var layout = require('../util/layout');

    /**
     * @alias module:echarts/Dto/Component
     * @constructor
     * @param {Object} option
     * @param {module:echarts/Dto/Dto} parentDto
     * @param {module:echarts/Dto/Dto} ecDto
     */
    var ComponentDto = Dto.extend({

        type: 'component',

        /**
         * @readOnly
         * @type {string}
         */
        id: '',

        /**
         * @readOnly
         */
        name: '',

        /**
         * @readOnly
         * @type {string}
         */
        mainType: '',

        /**
         * @readOnly
         * @type {string}
         */
        subType: '',

        /**
         * @readOnly
         * @type {number}
         */
        componentIndex: 0,

        /**
         * @type {Object}
         * @protected
         */
        defaultOption: null,

        /**
         * @type {module:echarts/Dto/Global}
         * @readOnly
         */
        ecDto: null,

        /**
         * key: componentType
         * value:  Component Dto list, can not be null.
         * @type {Object.<string, Array.<module:echarts/Dto/Dto>>}
         * @readOnly
         */
        dependentDtos: [],

        /**
         * @type {string}
         * @readOnly
         */
        uid: null,

        /**
         * Support merge layout params.
         * Only support 'box' now (left/right/top/bottom/width/height).
         * @type {string|Object} Object can be {ignoreSize: true}
         * @readOnly
         */
        layoutMode: null,


        init: function (option, parentDto, ecDto, extraOpt) {
            this.mergeDefaultAndTheme(this.option, this.ecDto);
        },

        mergeDefaultAndTheme: function (option, ecDto) {
            var layoutMode = this.layoutMode;
            var inputPositionParams = layoutMode
                ? layout.getLayoutParams(option) : {};

            var themeDto = ecDto.getTheme();
            zrUtil.merge(option, themeDto.get(this.mainType));
            zrUtil.merge(option, this.getDefaultOption());

            if (layoutMode) {
                layout.mergeLayoutParam(option, inputPositionParams, layoutMode);
            }
        },

        mergeOption: function (option) {
            zrUtil.merge(this.option, option, true);

            var layoutMode = this.layoutMode;
            if (layoutMode) {
                layout.mergeLayoutParam(this.option, option, layoutMode);
            }
        },

        // Hooker after init or mergeOption
        optionUpdated: function (ecDto) {},

        getDefaultOption: function () {
            if (!this.hasOwnProperty('__defaultOption')) {
                var optList = [];
                var Class = this.constructor;
                while (Class) {
                    var opt = Class.prototype.defaultOption;
                    opt && optList.push(opt);
                    Class = Class.superClass;
                }

                var defaultOption = {};
                for (var i = optList.length - 1; i >= 0; i--) {
                    defaultOption = zrUtil.merge(defaultOption, optList[i], true);
                }
                this.__defaultOption = defaultOption;
            }
            return this.__defaultOption;
        }

    });

    // Reset ComponentDto.extend, add preConstruct.
    clazzUtil.enableClassExtend(
        ComponentDto,
        function (option, parentDto, ecDto, extraOpt) {
            // Set dependentDtos, componentIndex, name, id, mainType, subType.
            zrUtil.extend(this, extraOpt);

            this.uid = componentUtil.getUID('componentDto');

            // this.setReadOnly([
            //     'type', 'id', 'uid', 'name', 'mainType', 'subType',
            //     'dependentDtos', 'componentIndex'
            // ]);
        }
    );

    // Add capability of registerClass, getClass, hasClass, registerSubTypeDefaulter and so on.
    clazzUtil.enableClassManagement(
        ComponentDto, {registerWhenExtend: true}
    );
    componentUtil.enableSubTypeDefaulter(ComponentDto);

    // Add capability of ComponentDto.topologicalTravel.
    componentUtil.enableTopologicalTravel(ComponentDto, getDependencies);

    function getDependencies(componentType) {
        var deps = [];
        zrUtil.each(ComponentDto.getClassesByMainType(componentType), function (Clazz) {
            arrayPush.apply(deps, Clazz.prototype.dependencies || []);
        });
        // Ensure main type
        return zrUtil.map(deps, function (type) {
            return clazzUtil.parseClassType(type).main;
        });
    }

    zrUtil.mixin(ComponentDto, require('./mixin/boxLayout'));

    return ComponentDto;
});