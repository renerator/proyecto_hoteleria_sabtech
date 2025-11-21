define(function(require) {

    'use strict';

    var List = require('../../data/List');
    var completeDimensions = require('../../data/helper/completeDimensions');
    var WhiskerBoxDraw = require('../helper/WhiskerBoxDraw');
    var zrUtil = require('zrender/core/util');

    function getItemValue(item) {
        return item.value == null ? item : item.value;
    }

    var seriesDtoMixin = {

        /**
         * @private
         * @type {string}
         */
        _baseAxisDim: null,

        /**
         * @override
         */
        getInitialData: function (option, ecDto) {
            // When both types of xAxis and yAxis are 'value', layout is
            // needed to be specified by user. Otherwise, layout can be
            // judged by which axis is category.

            var categories;

            var xAxisDto = ecDto.getComponent('xAxis', this.get('xAxisIndex'));
            var yAxisDto = ecDto.getComponent('yAxis', this.get('yAxisIndex'));
            var xAxisType = xAxisDto.get('type');
            var yAxisType = yAxisDto.get('type');
            var addOrdinal;

            // FIXME
            // 考虑时间轴

            if (xAxisType === 'category') {
                option.layout = 'horizontal';
                categories = xAxisDto.getCategories();
                addOrdinal = true;
            }
            else if (yAxisType  === 'category') {
                option.layout = 'vertical';
                categories = yAxisDto.getCategories();
                addOrdinal = true;
            }
            else {
                option.layout = option.layout || 'horizontal';
            }

            this._baseAxisDim = option.layout === 'horizontal' ? 'x' : 'y';

            var data = option.data;
            var dimensions = this.dimensions = ['base'].concat(this.valueDimensions);
            completeDimensions(dimensions, data);

            var list = new List(dimensions, this);
            list.initData(data, categories ? categories.slice() : null, function (dataItem, dimName, idx, dimIdx) {
                var value = getItemValue(dataItem);
                return addOrdinal ? (dimName === 'base' ? idx : value[dimIdx - 1]) : value[dimIdx];
            });

            return list;
        },

        /**
         * Used by Gird.
         * @param {string} axisDim 'x' or 'y'
         * @return {Array.<string>} dimensions on the axis.
         */
        coordDimToDataDim: function (axisDim) {
            var dims = this.valueDimensions.slice();
            var baseDim = ['base'];
            var map = {
                horizontal: {x: baseDim, y: dims},
                vertical: {x: dims, y: baseDim}
            };
            return map[this.get('layout')][axisDim];
        },

        /**
         * @override
         * @param {string|number} dataDim
         * @return {string} coord dimension
         */
        dataDimToCoordDim: function (dataDim) {
            var dim;

            zrUtil.each(['x', 'y'], function (coordDim, index) {
                var dataDims = this.coordDimToDataDim(coordDim);
                if (zrUtil.indexOf(dataDims, dataDim) >= 0) {
                    dim = coordDim;
                }
            }, this);

            return dim;
        },

        /**
         * If horizontal, base axis is x, otherwise y.
         * @override
         */
        getBaseAxis: function () {
            var dim = this._baseAxisDim;
            return this.ecDto.getComponent(dim + 'Axis', this.get(dim + 'AxisIndex')).axis;
        }
    };

    var viewMixin = {

        init: function () {
            /**
             * Old data.
             * @private
             * @type {module:echarts/chart/helper/WhiskerBoxDraw}
             */
            var whiskerBoxDraw = this._whiskerBoxDraw = new WhiskerBoxDraw(
                this.getStyleUpdater()
            );
            this.group.add(whiskerBoxDraw.group);
        },

        render: function (seriesDto, ecDto, api) {
            this._whiskerBoxDraw.updateData(seriesDto.getData());
        },

        remove: function (ecDto) {
            this._whiskerBoxDraw.remove();
        }
    };

    return {
        seriesDtoMixin: seriesDtoMixin,
        viewMixin: viewMixin
    };
});