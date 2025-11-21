define(function (require) {

    var graphic = require('../../util/graphic');
    var HeatmapLayer = require('./HeatmapLayer');
    var zrUtil = require('zrender/core/util');

    function getIsInPiecewiseRange(dataExtent, pieceList, selected) {
        var dataSpan = dataExtent[1] - dataExtent[0];
        pieceList = zrUtil.map(pieceList, function (piece) {
            return {
                interval: [
                    (piece.interval[0] - dataExtent[0]) / dataSpan,
                    (piece.interval[1] - dataExtent[0]) / dataSpan
                ]
            };
        });
        var len = pieceList.length;
        var lastIndex = 0;
        return function (val) {
            // Try to find in the location of the last found
            for (var i = lastIndex; i < len; i++) {
                var interval = pieceList[i].interval;
                if (interval[0] <= val && val <= interval[1]) {
                    lastIndex = i;
                    break;
                }
            }
            if (i === len) { // Not found, back interation
                for (var i = lastIndex - 1; i >= 0; i--) {
                    var interval = pieceList[i].interval;
                    if (interval[0] <= val && val <= interval[1]) {
                        lastIndex = i;
                        break;
                    }
                }
            }
            return i >= 0 && i < len && selected[i];
        };
    }

    function getIsInContinuousRange(dataExtent, range) {
        var dataSpan = dataExtent[1] - dataExtent[0];
        range = [
            (range[0] - dataExtent[0]) / dataSpan,
            (range[1] - dataExtent[0]) / dataSpan
        ];
        return function (val) {
            return val >= range[0] && val <= range[1];
        };
    }

    function isGeoCoordSys(coordSys) {
        var dimensions = coordSys.dimensions;
        // Not use coorSys.type === 'geo' because coordSys maybe extended
        return dimensions[0] === 'lng' && dimensions[1] === 'lat';
    }

    return require('../../echarts').extendChartView({

        type: 'heatmap',

        render: function (seriesDto, ecDto, api) {
            var visualMapOfThisSeries;
            ecDto.eachComponent('visualMap', function (visualMap) {
                visualMap.eachTargetSeries(function (targetSeries) {
                    if (targetSeries === seriesDto) {
                        visualMapOfThisSeries = visualMap;
                    }
                });
            });

            if (!visualMapOfThisSeries) {
                throw new Error('Heatmap must use with visualMap');
            }

            this.group.removeAll();
            var coordSys = seriesDto.coordinateSystem;
            if (coordSys.type === 'cartesian2d') {
                this._renderOnCartesian(coordSys, seriesDto, api);
            }
            else if (isGeoCoordSys(coordSys)) {
                this._renderOnGeo(
                    coordSys, seriesDto, visualMapOfThisSeries, api
                );
            }
        },

        _renderOnCartesian: function (cartesian, seriesDto, api) {
            var xAxis = cartesian.getAxis('x');
            var yAxis = cartesian.getAxis('y');
            var group = this.group;

            if (!(xAxis.type === 'category' && yAxis.type === 'category')) {
                throw new Error('Heatmap on cartesian must have two category axes');
            }
            if (!(xAxis.onBand && yAxis.onBand)) {
                throw new Error('Heatmap on cartesian must have two axes with boundaryGap true');
            }
            var width = xAxis.getBandWidth();
            var height = yAxis.getBandWidth();

            var data = seriesDto.getData();
            data.each(['x', 'y', 'z'], function (x, y, z, idx) {
                var itemDto = data.getItemDto(idx);
                var point = cartesian.dataToPoint([x, y]);
                // Ignore empty data
                if (isNaN(z)) {
                    return;
                }
                var rect = new graphic.Rect({
                    shape: {
                        x: point[0] - width / 2,
                        y: point[1] - height / 2,
                        width: width,
                        height: height
                    },
                    style: {
                        fill: data.getItemVisual(idx, 'color'),
                        opacity: data.getItemVisual(idx, 'opacity')
                    }
                });
                var style = itemDto.getDto('itemStyle.normal').getItemStyle(['color']);
                var hoverStl = itemDto.getDto('itemStyle.emphasis').getItemStyle();
                var labelDto = itemDto.getDto('label.normal');
                var hoverLabelDto = itemDto.getDto('label.emphasis');

                var rawValue = seriesDto.getRawValue(idx);
                var defaultText = '-';
                if (rawValue && rawValue[2] != null) {
                    defaultText = rawValue[2];
                }
                if (labelDto.get('show')) {
                    graphic.setText(style, labelDto);
                    style.text = seriesDto.getFormattedLabel(idx, 'normal') || defaultText;
                }
                if (hoverLabelDto.get('show')) {
                    graphic.setText(hoverStl, hoverLabelDto);
                    hoverStl.text = seriesDto.getFormattedLabel(idx, 'emphasis') || defaultText;
                }

                rect.setStyle(style);

                graphic.setHoverStyle(rect, hoverStl);

                group.add(rect);
                data.setItemGraphicEl(idx, rect);
            });
        },

        _renderOnGeo: function (geo, seriesDto, visualMapDto, api) {
            var inRangeVisuals = visualMapDto.targetVisuals.inRange;
            var outOfRangeVisuals = visualMapDto.targetVisuals.outOfRange;
            // if (!visualMapping) {
            //     throw new Error('Data range must have color visuals');
            // }

            var data = seriesDto.getData();
            var hmLayer = this._hmLayer || (this._hmLayer || new HeatmapLayer());
            hmLayer.blurSize = seriesDto.get('blurSize');
            hmLayer.pointSize = seriesDto.get('pointSize');
            hmLayer.minOpacity = seriesDto.get('minOpacity');
            hmLayer.maxOpacity = seriesDto.get('maxOpacity');

            var rect = geo.getViewRect().clone();
            var roamTransform = geo.getRoamTransform().transform;
            rect.applyTransform(roamTransform);

            // Clamp on viewport
            var x = Math.max(rect.x, 0);
            var y = Math.max(rect.y, 0);
            var x2 = Math.min(rect.width + rect.x, api.getWidth());
            var y2 = Math.min(rect.height + rect.y, api.getHeight());
            var width = x2 - x;
            var height = y2 - y;

            var points = data.mapArray(['lng', 'lat', 'value'], function (lng, lat, value) {
                var pt = geo.dataToPoint([lng, lat]);
                pt[0] -= x;
                pt[1] -= y;
                pt.push(value);
                return pt;
            });

            var dataExtent = visualMapDto.getExtent();
            var isInRange = visualMapDto.type === 'visualMap.continuous'
                ? getIsInContinuousRange(dataExtent, visualMapDto.option.range)
                : getIsInPiecewiseRange(
                    dataExtent, visualMapDto.getPieceList(), visualMapDto.option.selected
                );

            hmLayer.update(
                points, width, height,
                inRangeVisuals.color.getNormalizer(),
                {
                    inRange: inRangeVisuals.color.getColorMapper(),
                    outOfRange: outOfRangeVisuals.color.getColorMapper()
                },
                isInRange
            );
            var img = new graphic.Image({
                style: {
                    width: width,
                    height: height,
                    x: x,
                    y: y,
                    image: hmLayer.canvas
                },
                silent: true
            });
            this.group.add(img);
        }
    });
});