define(function (require) {

    'use strict';

    var zrUtil = require('zrender/core/util');
    var graphic = require('../../util/graphic');

    zrUtil.extend(require('../../Dto/Dto').prototype, require('./barItemStyle'));

    function fixLayoutWithLineWidth(layout, lineWidth) {
        var signX = layout.width > 0 ? 1 : -1;
        var signY = layout.height > 0 ? 1 : -1;
        // In case width or height are too small.
        lineWidth = Math.min(lineWidth, Math.abs(layout.width), Math.abs(layout.height));
        layout.x += signX * lineWidth / 2;
        layout.y += signY * lineWidth / 2;
        layout.width -= signX * lineWidth;
        layout.height -= signY * lineWidth;
    }

    return require('../../echarts').extendChartView({

        type: 'bar',

        render: function (seriesDto, ecDto, api) {
            var coordinateSystemType = seriesDto.get('coordinateSystem');

            if (coordinateSystemType === 'cartesian2d') {
                this._renderOnCartesian(seriesDto, ecDto, api);
            }

            return this.group;
        },

        _renderOnCartesian: function (seriesDto, ecDto, api) {
            var group = this.group;
            var data = seriesDto.getData();
            var oldData = this._data;

            var cartesian = seriesDto.coordinateSystem;
            var baseAxis = cartesian.getBaseAxis();
            var isHorizontal = baseAxis.isHorizontal();

            var enableAnimation = seriesDto.get('animation');

            var barBorderWidthQuery = ['itemStyle', 'normal', 'barBorderWidth'];

            function createRect(dataIndex, isUpdate) {
                var layout = data.getItemLayout(dataIndex);
                var lineWidth = data.getItemDto(dataIndex).get(barBorderWidthQuery) || 0;
                fixLayoutWithLineWidth(layout, lineWidth);

                var rect = new graphic.Rect({
                    shape: zrUtil.extend({}, layout)
                });
                // Animation
                if (enableAnimation) {
                    var rectShape = rect.shape;
                    var animateProperty = isHorizontal ? 'height' : 'width';
                    var animateTarget = {};
                    rectShape[animateProperty] = 0;
                    animateTarget[animateProperty] = layout[animateProperty];
                    graphic[isUpdate? 'updateProps' : 'initProps'](rect, {
                        shape: animateTarget
                    }, seriesDto, dataIndex);
                }
                return rect;
            }
            data.diff(oldData)
                .add(function (dataIndex) {
                    // 空数据
                    if (!data.hasValue(dataIndex)) {
                        return;
                    }

                    var rect = createRect(dataIndex);

                    data.setItemGraphicEl(dataIndex, rect);

                    group.add(rect);

                })
                .update(function (newIndex, oldIndex) {
                    var rect = oldData.getItemGraphicEl(oldIndex);
                    // 空数据
                    if (!data.hasValue(newIndex)) {
                        group.remove(rect);
                        return;
                    }
                    if (!rect) {
                        rect = createRect(newIndex, true);
                    }

                    var layout = data.getItemLayout(newIndex);
                    var lineWidth = data.getItemDto(newIndex).get(barBorderWidthQuery) || 0;
                    fixLayoutWithLineWidth(layout, lineWidth);

                    graphic.updateProps(rect, {
                        shape: layout
                    }, seriesDto, newIndex);

                    data.setItemGraphicEl(newIndex, rect);

                    // Add back
                    group.add(rect);
                })
                .remove(function (idx) {
                    var rect = oldData.getItemGraphicEl(idx);
                    if (rect) {
                        // Not show text when animating
                        rect.style.text = '';
                        graphic.updateProps(rect, {
                            shape: {
                                width: 0
                            }
                        }, seriesDto, idx, function () {
                            group.remove(rect);
                        });
                    }
                })
                .execute();

            this._updateStyle(seriesDto, data, isHorizontal);

            this._data = data;
        },

        _updateStyle: function (seriesDto, data, isHorizontal) {
            function setLabel(style, Dto, color, labelText, labelPositionOutside) {
                graphic.setText(style, Dto, color);
                style.text = labelText;
                if (style.textPosition === 'outside') {
                    style.textPosition = labelPositionOutside;
                }
            }

            data.eachItemGraphicEl(function (rect, idx) {
                var itemDto = data.getItemDto(idx);
                var color = data.getItemVisual(idx, 'color');
                var opacity = data.getItemVisual(idx, 'opacity');
                var layout = data.getItemLayout(idx);
                var itemStyleDto = itemDto.getDto('itemStyle.normal');

                var hoverStyle = itemDto.getDto('itemStyle.emphasis').getBarItemStyle();

                rect.setShape('r', itemStyleDto.get('barBorderRadius') || 0);

                rect.useStyle(zrUtil.defaults(
                    {
                        fill: color,
                        opacity: opacity
                    },
                    itemStyleDto.getBarItemStyle()
                ));

                var labelPositionOutside = isHorizontal
                    ? (layout.height > 0 ? 'bottom' : 'top')
                    : (layout.width > 0 ? 'left' : 'right');

                var labelDto = itemDto.getDto('label.normal');
                var hoverLabelDto = itemDto.getDto('label.emphasis');
                var rectStyle = rect.style;
                if (labelDto.get('show')) {
                    setLabel(
                        rectStyle, labelDto, color,
                        zrUtil.retrieve(
                            seriesDto.getFormattedLabel(idx, 'normal'),
                            seriesDto.getRawValue(idx)
                        ),
                        labelPositionOutside
                    );
                }
                else {
                    rectStyle.text = '';
                }
                if (hoverLabelDto.get('show')) {
                    setLabel(
                        hoverStyle, hoverLabelDto, color,
                        zrUtil.retrieve(
                            seriesDto.getFormattedLabel(idx, 'emphasis'),
                            seriesDto.getRawValue(idx)
                        ),
                        labelPositionOutside
                    );
                }
                else {
                    hoverStyle.text = '';
                }
                graphic.setHoverStyle(rect, hoverStyle);
            });
        },

        remove: function (ecDto, api) {
            var group = this.group;
            if (ecDto.get('animation')) {
                if (this._data) {
                    this._data.eachItemGraphicEl(function (el) {
                        // Not show text when animating
                        el.style.text = '';
                        graphic.updateProps(el, {
                            shape: {
                                width: 0
                            }
                        }, ecDto, el.dataIndex, function () {
                            group.remove(el);
                        });
                    });
                }
            }
            else {
                group.removeAll();
            }
        }
    });
});