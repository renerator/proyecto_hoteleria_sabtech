/**
 * Single coordinates system.
 */
define(function (require) {

    var SingleAxis = require('./SingleAxis');
    var axisHelper = require('../axisHelper');
    var layout = require('../../util/layout');

    /**
     * Create a single coordinates system.
     *
     * @param {module:echarts/coord/single/AxisDto} axisDto
     * @param {module:echarts/Dto/Global} ecDto
     * @param {module:echarts/ExtensionAPI} api
     */
    function Single(axisDto, ecDto, api) {

        /**
         * @type {string}
         * @readOnly
         */
        this.dimension = 'oneDim';

        /**
         * Add it just for draw tooltip.
         *
         * @type {Array.<string>}
         * @readOnly
         */
        this.dimensions = ['oneDim'];

        /**
         * @private
         * @type {module:echarts/coord/single/SingleAxis}.
         */
        this._axis = null;

        /**
         * @private
         * @type {module:zrender/core/BoundingRect}
         */
        this._rect;

        this._init(axisDto, ecDto, api);

        /**
         * @type {module:echarts/coord/single/AxisDto}
         */
        this._Dto = axisDto;
    }

    Single.prototype = {

        type: 'single',

        constructor: Single,

        /**
         * Initialize single coordinate system.
         *
         * @param  {module:echarts/coord/single/AxisDto} axisDto
         * @param  {module:echarts/Dto/Global} ecDto
         * @param  {module:echarts/ExtensionAPI} api
         * @private
         */
        _init: function (axisDto, ecDto, api) {

            var dim = this.dimension;

            var axis = new SingleAxis(
                dim,
                axisHelper.createScaleByDto(axisDto),
                [0, 0],
                axisDto.get('type'),
                axisDto.get('position')
            );

            var isCategory = axis.type === 'category';
            axis.onBand = isCategory && axisDto.get('boundaryGap');
            axis.inverse = axisDto.get('inverse');
            axis.orient = axisDto.get('orient');

            axisDto.axis = axis;
            axis.Dto = axisDto;
            this._axis = axis;
        },

        /**
         * Update axis scale after data processed
         * @param  {module:echarts/Dto/Global} ecDto
         * @param  {module:echarts/ExtensionAPI} api
         */
        update: function (ecDto, api) {
            this._updateAxisFromSeries(ecDto);
        },

        /**
         * Update the axis extent from series.
         *
         * @param  {module:echarts/Dto/Global} ecDto
         * @private
         */
        _updateAxisFromSeries: function (ecDto) {

            ecDto.eachSeries(function (seriesDto) {

                var data = seriesDto.getData();
                var dim = this.dimension;
                this._axis.scale.unionExtent(
                    data.getDataExtent(seriesDto.coordDimToDataDim(dim))
                );
                axisHelper.niceScaleExtent(this._axis, this._axis.Dto);
            }, this);
        },

        /**
         * Resize the single coordinate system.
         *
         * @param  {module:echarts/coord/single/AxisDto} axisDto
         * @param  {module:echarts/ExtensionAPI} api
         */
        resize: function (axisDto, api) {
            this._rect = layout.getLayoutRect(
                {
                    left: axisDto.get('left'),
                    top: axisDto.get('top'),
                    right: axisDto.get('right'),
                    bottom: axisDto.get('bottom'),
                    width: axisDto.get('width'),
                    height: axisDto.get('height')
                },
                {
                    width: api.getWidth(),
                    height: api.getHeight()
                }
            );

            this._adjustAxis();
        },

        /**
         * @return {module:zrender/core/BoundingRect}
         */
        getRect: function () {
            return this._rect;
        },

        /**
         * @private
         */
        _adjustAxis: function () {

            var rect = this._rect;
            var axis = this._axis;

            var isHorizontal = axis.isHorizontal();
            var extent = isHorizontal ? [0, rect.width] : [0, rect.height];
            var idx =  axis.reverse ? 1 : 0;

            axis.setExtent(extent[idx], extent[1 - idx]);

            this._updateAxisTransform(axis, isHorizontal ? rect.x : rect.y);

        },

        /**
         * @param  {module:echarts/coord/single/SingleAxis} axis
         * @param  {number} coordBase
         */
        _updateAxisTransform: function (axis, coordBase) {

            var axisExtent = axis.getExtent();
            var extentSum = axisExtent[0] + axisExtent[1];
            var isHorizontal = axis.isHorizontal();

            axis.toGlobalCoord = isHorizontal ?
                function (coord) {
                    return coord + coordBase;
                } :
                function (coord) {
                    return extentSum - coord + coordBase;
                };

            axis.toLocalCoord = isHorizontal ?
                function (coord) {
                    return coord - coordBase;
                } :
                function (coord) {
                    return extentSum - coord + coordBase;
                };
        },

        /**
         * Get axis.
         *
         * @return {module:echarts/coord/single/SingleAxis}
         */
        getAxis: function () {
            return this._axis;
        },

        /**
         * Get axis, add it just for draw tooltip.
         *
         * @return {[type]} [description]
         */
        getBaseAxis: function () {
            return this._axis;
        },

        /**
         * If contain point.
         *
         * @param  {Array.<number>} point
         * @return {boolean}
         */
        containPoint: function (point) {
            var rect = this.getRect();
            var axis = this.getAxis();
            var orient = axis.orient;
            if (orient === 'horizontal') {
                return axis.contain(axis.toLocalCoord(point[0]))
                && (point[1] >= rect.y && point[1] <= (rect.y + rect.height));
            }
            else {
                return axis.contain(axis.toLocalCoord(point[1]))
                && (point[0] >= rect.y && point[0] <= (rect.y + rect.height));
            }
        },

        /**
         * @param {Array.<number>} point
         */
        pointToData: function (point) {
            var axis = this.getAxis();
            var orient = axis.orient;
            if (orient === 'horizontal') {
                return [
                    axis.coordToData(axis.toLocalCoord(point[0])),
                    point[1]
                ];
            }
            else {
                return [
                    axis.coordToData(axis.toLocalCoord(point[1])),
                    point[0]
                ];
            }
        },

        /**
         * Convert the series data to concrete point.
         *
         * @param  {*} value
         * @return {number}
         */
        dataToPoint: function (point) {
            var axis = this.getAxis();
            return [axis.toGlobalCoord(axis.dataToCoord(point[0])), point[1]];
        }
    };

    return Single;

});