/**
 * @module echarts/Dto/Dto
 */
define(function (require) {

    var zrUtil = require('zrender/core/util');
    var clazzUtil = require('../util/clazz');

    /**
     * @alias module:echarts/Dto/Dto
     * @constructor
     * @param {Object} option
     * @param {module:echarts/Dto/Dto} [parentDto]
     * @param {module:echarts/Dto/Global} [ecDto]
     * @param {Object} extraOpt
     */
    function Dto(option, parentDto, ecDto, extraOpt) {
        /**
         * @type {module:echarts/Dto/Dto}
         * @readOnly
         */
        this.parentDto = parentDto;

        /**
         * @type {module:echarts/Dto/Global}
         * @readOnly
         */
        this.ecDto = ecDto;

        /**
         * @type {Object}
         * @protected
         */
        this.option = option;

        // Simple optimization
        if (this.init) {
            if (arguments.length <= 4) {
                this.init(option, parentDto, ecDto, extraOpt);
            }
            else {
                this.init.apply(this, arguments);
            }
        }
    }

    Dto.prototype = {

        constructor: Dto,

        /**
         * Dto 的初始化函数
         * @param {Object} option
         */
        init: null,

        /**
         * 从新的 Option merge
         */
        mergeOption: function (option) {
            zrUtil.merge(this.option, option, true);
        },

        /**
         * @param {string} path
         * @param {boolean} [ignoreParent=false]
         * @return {*}
         */
        get: function (path, ignoreParent) {
            if (!path) {
                return this.option;
            }

            if (typeof path === 'string') {
                path = path.split('.');
            }

            var obj = this.option;
            var parentDto = this.parentDto;
            for (var i = 0; i < path.length; i++) {
                // Ignore empty
                if (!path[i]) {
                    continue;
                }
                // obj could be number/string/... (like 0)
                obj = (obj && typeof obj === 'object') ? obj[path[i]] : null;
                if (obj == null) {
                    break;
                }
            }
            if (obj == null && parentDto && !ignoreParent) {
                obj = parentDto.get(path);
            }
            return obj;
        },

        /**
         * @param {string} key
         * @param {boolean} [ignoreParent=false]
         * @return {*}
         */
        getShallow: function (key, ignoreParent) {
            var option = this.option;
            var val = option && option[key];
            var parentDto = this.parentDto;
            if (val == null && parentDto && !ignoreParent) {
                val = parentDto.getShallow(key);
            }
            return val;
        },

        /**
         * @param {string} path
         * @param {module:echarts/Dto/Dto} [parentDto]
         * @return {module:echarts/Dto/Dto}
         */
        getDto: function (path, parentDto) {
            var obj = this.get(path, true);
            var thisParentDto = this.parentDto;
            var Dto = new Dto(
                obj, parentDto || (thisParentDto && thisParentDto.getDto(path)),
                this.ecDto
            );
            return Dto;
        },

        /**
         * If Dto has option
         */
        isEmpty: function () {
            return this.option == null;
        },

        restoreData: function () {},

        // Pending
        clone: function () {
            var Ctor = this.constructor;
            return new Ctor(zrUtil.clone(this.option));
        },

        setReadOnly: function (properties) {
            clazzUtil.setReadOnly(this, properties);
        }
    };

    // Enable Dto.extend.
    clazzUtil.enableClassExtend(Dto);

    var mixin = zrUtil.mixin;
    mixin(Dto, require('./mixin/lineStyle'));
    mixin(Dto, require('./mixin/areaStyle'));
    mixin(Dto, require('./mixin/textStyle'));
    mixin(Dto, require('./mixin/itemStyle'));

    return Dto;
});