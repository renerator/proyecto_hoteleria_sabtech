define(function (require) {

    var PointerPath = require('./PointerPath');

    var graphic = require('../../util/graphic');
    var numberUtil = require('../../util/number');
    var parsePercent = numberUtil.parsePercent;

    function parsePosition(seriesDto, api) {
        var center = seriesDto.get('center');
        var width = api.getWidth();
        var height = api.getHeight();
        var size = Math.min(width, height);
        var cx = parsePercent(center[0], api.getWidth());
        var cy = parsePercent(center[1], api.getHeight());
        var r = parsePercent(seriesDto.get('radius'), size / 2);

        return {
            cx: cx,
            cy: cy,
            r: r
        };
    }

    function formatLabel(label, labelFormatter) {
        if (labelFormatter) {
            if (typeof labelFormatter === 'string') {
                label = labelFormatter.replace('{value}', label);
            }
            else if (typeof labelFormatter === 'function') {
                label = labelFormatter(label);
            }
        }

        return label;
    }

    var PI2 = Math.PI * 2;

    var GaugeView = require('../../view/Chart').extend({

        type: 'gauge',

        render: function (seriesDto, ecDto, api) {

            this.group.removeAll();

            var colorList = seriesDto.get('axisLine.lineStyle.color');
            var posInfo = parsePosition(seriesDto, api);

            this._renderMain(
                seriesDto, ecDto, api, colorList, posInfo
            );
        },

        _renderMain: function (seriesDto, ecDto, api, colorList, posInfo) {
            var group = this.group;

            var axisLineDto = seriesDto.getDto('axisLine');
            var lineStyleDto = axisLineDto.getDto('lineStyle');

            var clockwise = seriesDto.get('clockwise');
            var startAngle = -seriesDto.get('startAngle') / 180 * Math.PI;
            var endAngle = -seriesDto.get('endAngle') / 180 * Math.PI;

            var angleRangeSpan = (endAngle - startAngle) % PI2;

            var prevEndAngle = startAngle;
            var axisLineWidth = lineStyleDto.get('width');

            for (var i = 0; i < colorList.length; i++) {
                // Clamp
                var percent = Math.min(Math.max(colorList[i][0], 0), 1);
                var endAngle = startAngle + angleRangeSpan * percent;
                var sector = new graphic.Sector({
                    shape: {
                        startAngle: prevEndAngle,
                        endAngle: endAngle,
                        cx: posInfo.cx,
                        cy: posInfo.cy,
                        clockwise: clockwise,
                        r0: posInfo.r - axisLineWidth,
                        r: posInfo.r
                    },
                    silent: true
                });

                sector.setStyle({
                    fill: colorList[i][1]
                });

                sector.setStyle(lineStyleDto.getLineStyle(
                    // Because we use sector to simulate arc
                    // so the properties for stroking are useless
                    ['color', 'borderWidth', 'borderColor']
                ));

                group.add(sector);

                prevEndAngle = endAngle;
            }

            var getColor = function (percent) {
                // Less than 0
                if (percent <= 0) {
                    return colorList[0][1];
                }
                for (var i = 0; i < colorList.length; i++) {
                    if (colorList[i][0] >= percent
                        && (i === 0 ? 0 : colorList[i - 1][0]) < percent
                    ) {
                        return colorList[i][1];
                    }
                }
                // More than 1
                return colorList[i - 1][1];
            };

            if (!clockwise) {
                var tmp = startAngle;
                startAngle = endAngle;
                endAngle = tmp;
            }

            this._renderTicks(
                seriesDto, ecDto, api, getColor, posInfo,
                startAngle, endAngle, clockwise
            );

            this._renderPointer(
                seriesDto, ecDto, api, getColor, posInfo,
                startAngle, endAngle, clockwise
            );

            this._renderTitle(
                seriesDto, ecDto, api, getColor, posInfo
            );
            this._renderDetail(
                seriesDto, ecDto, api, getColor, posInfo
            );
        },

        _renderTicks: function (
            seriesDto, ecDto, api, getColor, posInfo,
            startAngle, endAngle, clockwise
        ) {
            var group = this.group;
            var cx = posInfo.cx;
            var cy = posInfo.cy;
            var r = posInfo.r;

            var minVal = seriesDto.get('min');
            var maxVal = seriesDto.get('max');

            var splitLineDto = seriesDto.getDto('splitLine');
            var tickDto = seriesDto.getDto('axisTick');
            var labelDto = seriesDto.getDto('axisLabel');

            var splitNumber = seriesDto.get('splitNumber');
            var subSplitNumber = tickDto.get('splitNumber');

            var splitLineLen = parsePercent(
                splitLineDto.get('length'), r
            );
            var tickLen = parsePercent(
                tickDto.get('length'), r
            );

            var angle = startAngle;
            var step = (endAngle - startAngle) / splitNumber;
            var subStep = step / subSplitNumber;

            var splitLineStyle = splitLineDto.getDto('lineStyle').getLineStyle();
            var tickLineStyle = tickDto.getDto('lineStyle').getLineStyle();
            var textStyleDto = labelDto.getDto('textStyle');

            for (var i = 0; i <= splitNumber; i++) {
                var unitX = Math.cos(angle);
                var unitY = Math.sin(angle);
                // Split line
                if (splitLineDto.get('show')) {
                    var splitLine = new graphic.Line({
                        shape: {
                            x1: unitX * r + cx,
                            y1: unitY * r + cy,
                            x2: unitX * (r - splitLineLen) + cx,
                            y2: unitY * (r - splitLineLen) + cy
                        },
                        style: splitLineStyle,
                        silent: true
                    });
                    if (splitLineStyle.stroke === 'auto') {
                        splitLine.setStyle({
                            stroke: getColor(i / splitNumber)
                        });
                    }

                    group.add(splitLine);
                }

                // Label
                if (labelDto.get('show')) {
                    var label = formatLabel(
                        numberUtil.round(i / splitNumber * (maxVal - minVal) + minVal),
                        labelDto.get('formatter')
                    );

                    var text = new graphic.Text({
                        style: {
                            text: label,
                            x: unitX * (r - splitLineLen - 5) + cx,
                            y: unitY * (r - splitLineLen - 5) + cy,
                            fill: textStyleDto.getTextColor(),
                            textFont: textStyleDto.getFont(),
                            textVerticalAlign: unitY < -0.4 ? 'top' : (unitY > 0.4 ? 'bottom' : 'middle'),
                            textAlign: unitX < -0.4 ? 'left' : (unitX > 0.4 ? 'right' : 'center')
                        },
                        silent: true
                    });
                    if (text.style.fill === 'auto') {
                        text.setStyle({
                            fill: getColor(i / splitNumber)
                        });
                    }

                    group.add(text);
                }

                // Axis tick
                if (tickDto.get('show') && i !== splitNumber) {
                    for (var j = 0; j <= subSplitNumber; j++) {
                        var unitX = Math.cos(angle);
                        var unitY = Math.sin(angle);
                        var tickLine = new graphic.Line({
                            shape: {
                                x1: unitX * r + cx,
                                y1: unitY * r + cy,
                                x2: unitX * (r - tickLen) + cx,
                                y2: unitY * (r - tickLen) + cy
                            },
                            silent: true,
                            style: tickLineStyle
                        });

                        if (tickLineStyle.stroke === 'auto') {
                            tickLine.setStyle({
                                stroke: getColor((i + j / subSplitNumber) / splitNumber)
                            });
                        }

                        group.add(tickLine);
                        angle += subStep;
                    }
                    angle -= subStep;
                }
                else {
                    angle += step;
                }
            }
        },

        _renderPointer: function (
            seriesDto, ecDto, api, getColor, posInfo,
            startAngle, endAngle, clockwise
        ) {
            var valueExtent = [+seriesDto.get('min'), +seriesDto.get('max')];
            var angleExtent = [startAngle, endAngle];

            if (!clockwise) {
                angleExtent = angleExtent.reverse();
            }

            var data = seriesDto.getData();
            var oldData = this._data;

            var group = this.group;

            data.diff(oldData)
                .add(function (idx) {
                    var pointer = new PointerPath({
                        shape: {
                            angle: startAngle
                        }
                    });

                    graphic.updateProps(pointer, {
                        shape: {
                            angle: numberUtil.linearMap(data.get('value', idx), valueExtent, angleExtent, true)
                        }
                    }, seriesDto);

                    group.add(pointer);
                    data.setItemGraphicEl(idx, pointer);
                })
                .update(function (newIdx, oldIdx) {
                    var pointer = oldData.getItemGraphicEl(oldIdx);

                    graphic.updateProps(pointer, {
                        shape: {
                            angle: numberUtil.linearMap(data.get('value', newIdx), valueExtent, angleExtent, true)
                        }
                    }, seriesDto);

                    group.add(pointer);
                    data.setItemGraphicEl(newIdx, pointer);
                })
                .remove(function (idx) {
                    var pointer = oldData.getItemGraphicEl(idx);
                    group.remove(pointer);
                })
                .execute();

            data.eachItemGraphicEl(function (pointer, idx) {
                var itemDto = data.getItemDto(idx);
                var pointerDto = itemDto.getDto('pointer');

                pointer.setShape({
                    x: posInfo.cx,
                    y: posInfo.cy,
                    width: parsePercent(
                        pointerDto.get('width'), posInfo.r
                    ),
                    r: parsePercent(pointerDto.get('length'), posInfo.r)
                });

                pointer.useStyle(itemDto.getDto('itemStyle.normal').getItemStyle());

                if (pointer.style.fill === 'auto') {
                    pointer.setStyle('fill', getColor(
                        (data.get('value', idx) - valueExtent[0]) / (valueExtent[1] - valueExtent[0])
                    ));
                }

                graphic.setHoverStyle(
                    pointer, itemDto.getDto('itemStyle.emphasis').getItemStyle()
                );
            });

            this._data = data;
        },

        _renderTitle: function (
            seriesDto, ecDto, api, getColor, posInfo
        ) {
            var titleDto = seriesDto.getDto('title');
            if (titleDto.get('show')) {
                var textStyleDto = titleDto.getDto('textStyle');
                var offsetCenter = titleDto.get('offsetCenter');
                var x = posInfo.cx + parsePercent(offsetCenter[0], posInfo.r);
                var y = posInfo.cy + parsePercent(offsetCenter[1], posInfo.r);
                var text = new graphic.Text({
                    style: {
                        x: x,
                        y: y,
                        // FIXME First data name ?
                        text: seriesDto.getData().getName(0),
                        fill: textStyleDto.getTextColor(),
                        textFont: textStyleDto.getFont(),
                        textAlign: 'center',
                        textVerticalAlign: 'middle'
                    }
                });
                this.group.add(text);
            }
        },

        _renderDetail: function (
            seriesDto, ecDto, api, getColor, posInfo
        ) {
            var detailDto = seriesDto.getDto('detail');
            var minVal = seriesDto.get('min');
            var maxVal = seriesDto.get('max');
            if (detailDto.get('show')) {
                var textStyleDto = detailDto.getDto('textStyle');
                var offsetCenter = detailDto.get('offsetCenter');
                var x = posInfo.cx + parsePercent(offsetCenter[0], posInfo.r);
                var y = posInfo.cy + parsePercent(offsetCenter[1], posInfo.r);
                var width = parsePercent(detailDto.get('width'), posInfo.r);
                var height = parsePercent(detailDto.get('height'), posInfo.r);
                var value = seriesDto.getData().get('value', 0);
                var rect = new graphic.Rect({
                    shape: {
                        x: x - width / 2,
                        y: y - height / 2,
                        width: width,
                        height: height
                    },
                    style: {
                        text: formatLabel(
                            // FIXME First data name ?
                            value, detailDto.get('formatter')
                        ),
                        fill: detailDto.get('backgroundColor'),
                        textFill: textStyleDto.getTextColor(),
                        textFont: textStyleDto.getFont()
                    }
                });
                if (rect.style.textFill === 'auto') {
                    rect.setStyle('textFill', getColor(
                        numberUtil.linearMap(value, [minVal, maxVal], [0, 1], true)
                    ));
                }
                rect.setStyle(detailDto.getItemStyle(['color']));
                this.group.add(rect);
            }
        }
    });

    return GaugeView;
});