define(function (require) {

    var graphic = require('../../util/graphic');
    var zrUtil = require('zrender/core/util');
    var symbolUtil = require('../../util/symbol');

    function normalizeSymbolSize(symbolSize) {
        if (!zrUtil.isArray(symbolSize)) {
            symbolSize = [+symbolSize, +symbolSize];
        }
        return symbolSize;
    }
    return require('../../echarts').extendChartView({
        type: 'radar',

        render: function (seriesDto, ecDto, api) {
            var polar = seriesDto.coordinateSystem;
            var group = this.group;

            var data = seriesDto.getData();
            var oldData = this._data;

            function createSymbol(data, idx) {
                var symbolType = data.getItemVisual(idx, 'symbol') || 'circle';
                var color = data.getItemVisual(idx, 'color');
                if (symbolType === 'none') {
                    return;
                }
                var symbolPath = symbolUtil.createSymbol(
                    symbolType, -0.5, -0.5, 1, 1, color
                );
                symbolPath.attr({
                    style: {
                        strokeNoScale: true
                    },
                    z2: 100,
                    scale: normalizeSymbolSize(data.getItemVisual(idx, 'symbolSize'))
                });
                return symbolPath;
            }

            function updateSymbols(oldPoints, newPoints, symbolGroup, data, idx, isInit) {
                // Simply rerender all
                symbolGroup.removeAll();
                for (var i = 0; i < newPoints.length - 1; i++) {
                    var symbolPath = createSymbol(data, idx);
                    if (symbolPath) {
                        symbolPath.__dimIdx = i;
                        if (oldPoints[i]) {
                            symbolPath.attr('position', oldPoints[i]);
                            graphic[isInit ? 'initProps' : 'updateProps'](
                                symbolPath, {
                                    position: newPoints[i]
                                }, seriesDto, idx
                            );
                        }
                        else {
                            symbolPath.attr('position', newPoints[i]);
                        }
                        symbolGroup.add(symbolPath);
                    }
                }
            }

            function getInitialPoints(points) {
                return zrUtil.map(points, function (pt) {
                    return [polar.cx, polar.cy];
                });
            }
            data.diff(oldData)
                .add(function (idx) {
                    var points = data.getItemLayout(idx);
                    if (!points) {
                        return;
                    }
                    var polygon = new graphic.Polygon();
                    var polyline = new graphic.Polyline();
                    var target = {
                        shape: {
                            points: points
                        }
                    };
                    polygon.shape.points = getInitialPoints(points);
                    polyline.shape.points = getInitialPoints(points);
                    graphic.initProps(polygon, target, seriesDto, idx);
                    graphic.initProps(polyline, target, seriesDto, idx);

                    var itemGroup = new graphic.Group();
                    var symbolGroup = new graphic.Group();
                    itemGroup.add(polyline);
                    itemGroup.add(polygon);
                    itemGroup.add(symbolGroup);

                    updateSymbols(
                        polyline.shape.points, points, symbolGroup, data, idx, true
                    );

                    data.setItemGraphicEl(idx, itemGroup);
                })
                .update(function (newIdx, oldIdx) {
                    var itemGroup = oldData.getItemGraphicEl(oldIdx);
                    var polyline = itemGroup.childAt(0);
                    var polygon = itemGroup.childAt(1);
                    var symbolGroup = itemGroup.childAt(2);
                    var target = {
                        shape: {
                            points: data.getItemLayout(newIdx)
                        }
                    };
                    if (!target.shape.points) {
                        return;
                    }
                    updateSymbols(
                        polyline.shape.points, target.shape.points, symbolGroup, data, newIdx, false
                    );

                    graphic.updateProps(polyline, target, seriesDto);
                    graphic.updateProps(polygon, target, seriesDto);

                    data.setItemGraphicEl(newIdx, itemGroup);
                })
                .remove(function (idx) {
                    group.remove(oldData.getItemGraphicEl(idx));
                })
                .execute();

            data.eachItemGraphicEl(function (itemGroup, idx) {
                var itemDto = data.getItemDto(idx);
                var polyline = itemGroup.childAt(0);
                var polygon = itemGroup.childAt(1);
                var symbolGroup = itemGroup.childAt(2);
                var color = data.getItemVisual(idx, 'color');

                group.add(itemGroup);

                polyline.useStyle(
                    zrUtil.extend(
                        itemDto.getDto('lineStyle.normal').getLineStyle(),
                        {
                            fill: 'none',
                            stroke: color
                        }
                    )
                );
                polyline.hoverStyle = itemDto.getDto('lineStyle.emphasis').getLineStyle();

                var areaStyleDto = itemDto.getDto('areaStyle.normal');
                var hoverAreaStyleDto = itemDto.getDto('areaStyle.emphasis');
                var polygonIgnore = areaStyleDto.isEmpty() && areaStyleDto.parentDto.isEmpty();
                var hoverPolygonIgnore = hoverAreaStyleDto.isEmpty() && hoverAreaStyleDto.parentDto.isEmpty();

                hoverPolygonIgnore = hoverPolygonIgnore && polygonIgnore;
                polygon.ignore = polygonIgnore;

                polygon.useStyle(
                    zrUtil.defaults(
                        areaStyleDto.getAreaStyle(),
                        {
                            fill: color,
                            opacity: 0.7
                        }
                    )
                );
                polygon.hoverStyle = hoverAreaStyleDto.getAreaStyle();

                var itemStyle = itemDto.getDto('itemStyle.normal').getItemStyle(['color']);
                var itemHoverStyle = itemDto.getDto('itemStyle.emphasis').getItemStyle();
                var labelDto = itemDto.getDto('label.normal');
                var labelHoverDto = itemDto.getDto('label.emphasis');
                symbolGroup.eachChild(function (symbolPath) {
                    symbolPath.setStyle(itemStyle);
                    symbolPath.hoverStyle = zrUtil.clone(itemHoverStyle);

                    var defaultText = data.get(data.dimensions[symbolPath.__dimIdx], idx);
                    graphic.setText(symbolPath.style, labelDto, color);
                    symbolPath.setStyle({
                        text: labelDto.get('show') ? zrUtil.retrieve(
                            seriesDto.getFormattedLabel(
                                idx, 'normal', null, symbolPath.__dimIdx
                            ),
                            defaultText
                        ) : ''
                    });

                    graphic.setText(symbolPath.hoverStyle, labelHoverDto, color);
                    symbolPath.hoverStyle.text = labelHoverDto.get('show') ? zrUtil.retrieve(
                        seriesDto.getFormattedLabel(
                            idx, 'emphasis', null, symbolPath.__dimIdx
                        ),
                        defaultText
                    ) : '';
                });

                function onEmphasis() {
                    polygon.attr('ignore', hoverPolygonIgnore);
                }

                function onNormal() {
                    polygon.attr('ignore', polygonIgnore);
                }

                itemGroup.off('mouseover').off('mouseout').off('normal').off('emphasis');
                itemGroup.on('emphasis', onEmphasis)
                    .on('mouseover', onEmphasis)
                    .on('normal', onNormal)
                    .on('mouseout', onNormal);

                graphic.setHoverStyle(itemGroup);
            });

            this._data = data;
        },

        remove: function () {
            this.group.removeAll();
            this._data = null;
        }
    });
});