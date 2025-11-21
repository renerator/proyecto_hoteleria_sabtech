define(function (require) {

    var SymbolDraw = require('../../chart/helper/SymbolDraw');
    var zrUtil = require('zrender/core/util');
    var formatUtil = require('../../util/format');
    var DtoUtil = require('../../util/Dto');
    var numberUtil = require('../../util/number');

    var addCommas = formatUtil.addCommas;
    var encodeHTML = formatUtil.encodeHTML;

    var List = require('../../data/List');

    var markerHelper = require('./markerHelper');

    function updateMarkerLayout(mpData, seriesDto, api) {
        var coordSys = seriesDto.coordinateSystem;
        mpData.each(function (idx) {
            var itemDto = mpData.getItemDto(idx);
            var point;
            var xPx = itemDto.getShallow('x');
            var yPx = itemDto.getShallow('y');
            if (xPx != null && yPx != null) {
                point = [
                    numberUtil.parsePercent(xPx, api.getWidth()),
                    numberUtil.parsePercent(yPx, api.getHeight())
                ];
            }
            // Chart like bar may have there own marker positioning logic
            else if (seriesDto.getMarkerPosition) {
                // Use the getMarkerPoisition
                point = seriesDto.getMarkerPosition(
                    mpData.getValues(mpData.dimensions, idx)
                );
            }
            else if (coordSys) {
                var x = mpData.get(coordSys.dimensions[0], idx);
                var y = mpData.get(coordSys.dimensions[1], idx);
                point = coordSys.dataToPoint([x, y]);
            }

            mpData.setItemLayout(idx, point);
        });
    }

    // FIXME
    var markPointFormatMixin = {
        formatTooltip: function (dataIndex) {
            var data = this.getData();
            var value = this.getRawValue(dataIndex);
            var formattedValue = zrUtil.isArray(value)
                ? zrUtil.map(value, addCommas).join(', ') : addCommas(value);
            var name = data.getName(dataIndex);
            return this.name + '<br />'
                + ((name ? encodeHTML(name) + ' : ' : '') + formattedValue);
        },

        getData: function () {
            return this._data;
        },

        setData: function (data) {
            this._data = data;
        }
    };

    zrUtil.defaults(markPointFormatMixin, DtoUtil.dataFormatMixin);

    require('../../echarts').extendComponentView({

        type: 'markPoint',

        init: function () {
            this._symbolDrawMap = {};
        },

        render: function (markPointDto, ecDto, api) {
            var symbolDrawMap = this._symbolDrawMap;
            for (var name in symbolDrawMap) {
                symbolDrawMap[name].__keep = false;
            }

            ecDto.eachSeries(function (seriesDto) {
                var mpDto = seriesDto.markPointDto;
                mpDto && this._renderSeriesMP(seriesDto, mpDto, api);
            }, this);

            for (var name in symbolDrawMap) {
                if (!symbolDrawMap[name].__keep) {
                    symbolDrawMap[name].remove();
                    this.group.remove(symbolDrawMap[name].group);
                }
            }
        },

        updateLayout: function (markPointDto, ecDto, api) {
            ecDto.eachSeries(function (seriesDto) {
                var mpDto = seriesDto.markPointDto;
                if (mpDto) {
                    updateMarkerLayout(mpDto.getData(), seriesDto, api);
                    this._symbolDrawMap[seriesDto.name].updateLayout(mpDto);
                }
            }, this);
        },

        _renderSeriesMP: function (seriesDto, mpDto, api) {
            var coordSys = seriesDto.coordinateSystem;
            var seriesName = seriesDto.name;
            var seriesData = seriesDto.getData();

            var symbolDrawMap = this._symbolDrawMap;
            var symbolDraw = symbolDrawMap[seriesName];
            if (!symbolDraw) {
                symbolDraw = symbolDrawMap[seriesName] = new SymbolDraw();
            }

            var mpData = createList(coordSys, seriesDto, mpDto);

            // FIXME
            zrUtil.mixin(mpDto, markPointFormatMixin);
            mpDto.setData(mpData);

            updateMarkerLayout(mpDto.getData(), seriesDto, api);

            mpData.each(function (idx) {
                var itemDto = mpData.getItemDto(idx);
                var symbolSize = itemDto.getShallow('symbolSize');
                if (typeof symbolSize === 'function') {
                    // FIXME 这里不兼容 ECharts 2.x，2.x 貌似参数是整个数据？
                    symbolSize = symbolSize(
                        mpDto.getRawValue(idx), mpDto.getDataParams(idx)
                    );
                }
                mpData.setItemVisual(idx, {
                    symbolSize: symbolSize,
                    color: itemDto.get('itemStyle.normal.color')
                        || seriesData.getVisual('color'),
                    symbol: itemDto.getShallow('symbol')
                });
            });

            // TODO Text are wrong
            symbolDraw.updateData(mpData);
            this.group.add(symbolDraw.group);

            // Set host Dto for tooltip
            // FIXME
            mpData.eachItemGraphicEl(function (el) {
                el.traverse(function (child) {
                    child.dataDto = mpDto;
                });
            });

            symbolDraw.__keep = true;
        }
    });

    /**
     * @inner
     * @param {module:echarts/coord/*} [coordSys]
     * @param {module:echarts/Dto/Series} seriesDto
     * @param {module:echarts/Dto/Dto} mpDto
     */
    function createList(coordSys, seriesDto, mpDto) {
        var coordDimsInfos;
        if (coordSys) {
            coordDimsInfos = zrUtil.map(coordSys && coordSys.dimensions, function (coordDim) {
                var info = seriesDto.getData().getDimensionInfo(
                    seriesDto.coordDimToDataDim(coordDim)[0]
                ) || {}; // In map series data don't have lng and lat dimension. Fallback to same with coordSys
                info.name = coordDim;
                return info;
            });
        }
        else {
            coordDimsInfos =[{
                name: 'value',
                type: 'float'
            }];
        }

        var mpData = new List(coordDimsInfos, mpDto);
        var dataOpt = zrUtil.map(mpDto.get('data'), zrUtil.curry(
                markerHelper.dataTransform, seriesDto
            ));
        if (coordSys) {
            dataOpt = zrUtil.filter(
                dataOpt, zrUtil.curry(markerHelper.dataFilter, coordSys)
            );
        }

        mpData.initData(dataOpt, null,
            coordSys ? markerHelper.dimValueGetter : function (item) {
                return item.value;
            }
        );
        return mpData;
    }

});