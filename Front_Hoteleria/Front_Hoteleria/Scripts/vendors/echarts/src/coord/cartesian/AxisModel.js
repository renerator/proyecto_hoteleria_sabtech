define(function(require) {

    'use strict';

    var ComponentDto = require('../../Dto/Component');
    var zrUtil = require('zrender/core/util');
    var axisDtoCreator = require('../axisDtoCreator');

    var AxisDto = ComponentDto.extend({

        type: 'cartesian2dAxis',

        /**
         * @type {module:echarts/coord/cartesian/Axis2D}
         */
        axis: null,

        /**
         * @override
         */
        init: function () {
            AxisDto.superApply(this, 'init', arguments);
            this._resetRange();
        },

        /**
         * @override
         */
        mergeOption: function () {
            AxisDto.superApply(this, 'mergeOption', arguments);
            this._resetRange();
        },

        /**
         * @override
         */
        restoreData: function () {
            AxisDto.superApply(this, 'restoreData', arguments);
            this._resetRange();
        },

        /**
         * @public
         * @param {number} rangeStart
         * @param {number} rangeEnd
         */
        setRange: function (rangeStart, rangeEnd) {
            this.option.rangeStart = rangeStart;
            this.option.rangeEnd = rangeEnd;
        },

        /**
         * @public
         * @return {Array.<number|string|Date>}
         */
        getMin: function () {
            var option = this.option;
            return option.rangeStart != null ? option.rangeStart : option.min;
        },

        /**
         * @public
         * @return {Array.<number|string|Date>}
         */
        getMax: function () {
            var option = this.option;
            return option.rangeEnd != null ? option.rangeEnd : option.max;
        },

        /**
         * @public
         * @return {boolean}
         */
        getNeedCrossZero: function () {
            var option = this.option;
            return (option.rangeStart != null || option.rangeEnd != null)
                ? false : !option.scale;
        },

        /**
         * @private
         */
        _resetRange: function () {
            // rangeStart and rangeEnd is readonly.
            this.option.rangeStart = this.option.rangeEnd = null;
        }

    });

    function getAxisType(axisDim, option) {
        // Default axis with data is category axis
        return option.type || (option.data ? 'category' : 'value');
    }

    zrUtil.merge(AxisDto.prototype, require('../axisDtoCommonMixin'));

    var extraOption = {
        gridIndex: 0
    };

    axisDtoCreator('x', AxisDto, getAxisType, extraOption);
    axisDtoCreator('y', AxisDto, getAxisType, extraOption);

    return AxisDto;
});