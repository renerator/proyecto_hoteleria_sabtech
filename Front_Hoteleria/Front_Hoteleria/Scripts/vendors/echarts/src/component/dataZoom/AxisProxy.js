/**
 * @file Axis operator
 */
define(function(require) {

    var zrUtil = require('zrender/core/util');
    var numberUtil = require('../../util/number');
    var each = zrUtil.each;
    var asc = numberUtil.asc;

    /**
     * Operate single axis.
     * One axis can only operated by one axis operator.
     * Different dataZoomDtos may be defined to operate the same axis.
     * (i.e. 'inside' data zoom and 'slider' data zoom components)
     * So dataZoomDtos share one axisProxy in that case.
     *
     * @class
     */
    var AxisProxy = function (dimName, axisIndex, dataZoomDto, ecDto) {

        /**
         * @private
         * @type {string}
         */
        this._dimName = dimName;

        /**
         * @private
         */
        this._axisIndex = axisIndex;

        /**
         * @private
         * @type {Array.<number>}
         */
        this._valueWindow;

        /**
         * @private
         * @type {Array.<number>}
         */
        this._percentWindow;

        /**
         * @private
         * @type {Array.<number>}
         */
        this._dataExtent;

        /**
         * @readOnly
         * @type {module: echarts/Dto/Global}
         */
        this.ecDto = ecDto;

        /**
         * @private
         * @type {module: echarts/component/dataZoom/DataZoomDto}
         */
        this._dataZoomDto = dataZoomDto;
    };

    AxisProxy.prototype = {

        constructor: AxisProxy,

        /**
         * Whether the axisProxy is hosted by dataZoomDto.
         *
         * @public
         * @param {module: echarts/component/dataZoom/DataZoomDto} dataZoomDto
         * @return {boolean}
         */
        hostedBy: function (dataZoomDto) {
            return this._dataZoomDto === dataZoomDto;
        },

        /**
         * @return {Array.<number>}
         */
        getDataExtent: function () {
            return this._dataExtent.slice();
        },

        /**
         * @return {Array.<number>}
         */
        getDataValueWindow: function () {
            return this._valueWindow.slice();
        },

        /**
         * @return {Array.<number>}
         */
        getDataPercentWindow: function () {
            return this._percentWindow.slice();
        },

        /**
         * @public
         * @param {number} axisIndex
         * @return {Array} seriesDtos
         */
        getTargetSeriesDtos: function () {
            var seriesDtos = [];

            this.ecDto.eachSeries(function (seriesDto) {
                if (this._axisIndex === seriesDto.get(this._dimName + 'AxisIndex')) {
                    seriesDtos.push(seriesDto);
                }
            }, this);

            return seriesDtos;
        },

        getAxisDto: function () {
            return this.ecDto.getComponent(this._dimName + 'Axis', this._axisIndex);
        },

        getOtherAxisDto: function () {
            var axisDim = this._dimName;
            var ecDto = this.ecDto;
            var axisDto = this.getAxisDto();
            var isCartesian = axisDim === 'x' || axisDim === 'y';
            var otherAxisDim;
            var coordSysIndexName;
            if (isCartesian) {
                coordSysIndexName = 'gridIndex';
                otherAxisDim = axisDim === 'x' ? 'y' : 'x';
            }
            else {
                coordSysIndexName = 'polarIndex';
                otherAxisDim = axisDim === 'angle' ? 'radius' : 'angle';
            }
            var foundOtherAxisDto;
            ecDto.eachComponent(otherAxisDim + 'Axis', function (otherAxisDto) {
                if ((otherAxisDto.get(coordSysIndexName) || 0)
                    === (axisDto.get(coordSysIndexName) || 0)
                ) {
                    foundOtherAxisDto = otherAxisDto;
                }
            });
            return foundOtherAxisDto;
        },

        /**
         * Notice: reset should not be called before series.restoreData() called,
         * so it is recommanded to be called in "process stage" but not "Dto init
         * stage".
         *
         * @param {module: echarts/component/dataZoom/DataZoomDto} dataZoomDto
         */
        reset: function (dataZoomDto) {
            if (dataZoomDto !== this._dataZoomDto) {
                return;
            }

            // Culculate data window and data extent, and record them.
            var dataExtent = this._dataExtent = calculateDataExtent(
                this._dimName, this.getTargetSeriesDtos()
            );
            var dataWindow = calculateDataWindow(
                dataZoomDto.option, dataExtent, this
            );
            this._valueWindow = dataWindow.valueWindow;
            this._percentWindow = dataWindow.percentWindow;

            // Update axis setting then.
            setAxisDto(this);
        },

        /**
         * @param {module: echarts/component/dataZoom/DataZoomDto} dataZoomDto
         */
        restore: function (dataZoomDto) {
            if (dataZoomDto !== this._dataZoomDto) {
                return;
            }

            this._valueWindow = this._percentWindow = null;
            setAxisDto(this, true);
        },

        /**
         * @param {module: echarts/component/dataZoom/DataZoomDto} dataZoomDto
         */
        filterData: function (dataZoomDto) {
            if (dataZoomDto !== this._dataZoomDto) {
                return;
            }

            var axisDim = this._dimName;
            var seriesDtos = this.getTargetSeriesDtos();
            var filterMode = dataZoomDto.get('filterMode');
            var valueWindow = this._valueWindow;

            // FIXME
            // Toolbox may has dataZoom injected. And if there are stacked bar chart
            // with NaN data, NaN will be filtered and stack will be wrong.
            // So we need to force the mode to be set empty.
            // In fect, it is not a big deal that do not support filterMode-'filter'
            // when using toolbox#dataZoom, utill tooltip#dataZoom support "single axis
            // selection" some day, which might need "adapt to data extent on the
            // otherAxis", which is disabled by filterMode-'empty'.
            var otherAxisDto = this.getOtherAxisDto();
            if (dataZoomDto.get('$fromToolbox')
                && otherAxisDto
                && otherAxisDto.get('type') === 'category'
            ) {
                filterMode = 'empty';
            }

            // Process series data
            each(seriesDtos, function (seriesDto) {
                var seriesData = seriesDto.getData();

                seriesData && each(seriesDto.coordDimToDataDim(axisDim), function (dim) {
                    if (filterMode === 'empty') {
                        seriesDto.setData(
                            seriesData.map(dim, function (value) {
                                return !isInWindow(value) ? NaN : value;
                            })
                        );
                    }
                    else {
                        seriesData.filterSelf(dim, isInWindow);
                    }
                });
            });

            function isInWindow(value) {
                return value >= valueWindow[0] && value <= valueWindow[1];
            }
        }
    };

    function calculateDataExtent(axisDim, seriesDtos) {
        var dataExtent = [Infinity, -Infinity];

        each(seriesDtos, function (seriesDto) {
            var seriesData = seriesDto.getData();
            if (seriesData) {
                each(seriesDto.coordDimToDataDim(axisDim), function (dim) {
                    var seriesExtent = seriesData.getDataExtent(dim);
                    seriesExtent[0] < dataExtent[0] && (dataExtent[0] = seriesExtent[0]);
                    seriesExtent[1] > dataExtent[1] && (dataExtent[1] = seriesExtent[1]);
                });
            }
        }, this);

        return dataExtent;
    }

    function calculateDataWindow(opt, dataExtent, axisProxy) {
        var axisDto = axisProxy.getAxisDto();
        var scale = axisDto.axis.scale;
        var percentExtent = [0, 100];
        var percentWindow = [
            opt.start,
            opt.end
        ];
        var valueWindow = [];

        // In percent range is used and axis min/max/scale is set,
        // window should be based on min/max/0, but should not be
        // based on the extent of filtered data.
        dataExtent = dataExtent.slice();
        fixExtendByAxis(dataExtent, axisDto, scale);

        each(['startValue', 'endValue'], function (prop) {
            valueWindow.push(
                opt[prop] != null
                    ? scale.parse(opt[prop])
                    : null
            );
        });

        // Normalize bound.
        each([0, 1], function (idx) {
            var boundValue = valueWindow[idx];
            var boundPercent = percentWindow[idx];

            // start/end has higher priority over startValue/endValue,
            // because start/end can be consistent among different type
            // of axis but startValue/endValue not.

            if (boundPercent != null || boundValue == null) {
                if (boundPercent == null) {
                    boundPercent = percentExtent[idx];
                }
                // Use scale.parse to math round for category or time axis.
                boundValue = scale.parse(numberUtil.linearMap(
                    boundPercent, percentExtent, dataExtent, true
                ));
            }
            else { // boundPercent == null && boundValue != null
                boundPercent = numberUtil.linearMap(
                    boundValue, dataExtent, percentExtent, true
                );
            }
            // valueWindow[idx] = round(boundValue);
            // percentWindow[idx] = round(boundPercent);
            valueWindow[idx] = boundValue;
            percentWindow[idx] = boundPercent;
        });

        return {
            valueWindow: asc(valueWindow),
            percentWindow: asc(percentWindow)
        };
    }

    function fixExtendByAxis(dataExtent, axisDto, scale) {
        each(['min', 'max'], function (minMax, index) {
            var axisMax = axisDto.get(minMax, true);
            // Consider 'dataMin', 'dataMax'
            if (axisMax != null && (axisMax + '').toLowerCase() !== 'data' + minMax) {
                dataExtent[index] = scale.parse(axisMax);
            }
        });

        if (!axisDto.get('scale', true)) {
            dataExtent[0] > 0 && (dataExtent[0] = 0);
            dataExtent[1] < 0 && (dataExtent[1] = 0);
        }

        return dataExtent;
    }

    function setAxisDto(axisProxy, isRestore) {
        var axisDto = axisProxy.getAxisDto();

        var percentWindow = axisProxy._percentWindow;
        var valueWindow = axisProxy._valueWindow;

        if (!percentWindow) {
            return;
        }

        var isFull = isRestore || (percentWindow[0] === 0 && percentWindow[1] === 100);
        // [0, 500]: arbitrary value, guess axis extent.
        var precision = !isRestore && numberUtil.getPixelPrecision(valueWindow, [0, 500]);
        // toFixed() digits argument must be between 0 and 20
        var invalidPrecision = !isRestore && !(precision < 20 && precision >= 0);

        var useOrigin = isRestore || isFull || invalidPrecision;

        axisDto.setRange && axisDto.setRange(
            useOrigin ? null : +valueWindow[0].toFixed(precision),
            useOrigin ? null : +valueWindow[1].toFixed(precision)
        );
    }

    return AxisProxy;

});