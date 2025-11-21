define(function (require) {
    'use strict';

    var zrUtil = require('zrender/core/util');
    var graphic = require('../../util/graphic');
    var Dto = require('../../Dto/Dto');

    var elementList = ['axisLine', 'axisLabel', 'axisTick', 'splitLine', 'splitArea'];

    function getAxisLineShape(polar, r0, r, angle) {
        var start = polar.coordToPoint([r0, angle]);
        var end = polar.coordToPoint([r, angle]);

        return {
            x1: start[0],
            y1: start[1],
            x2: end[0],
            y2: end[1]
        };
    }
    require('../../echarts').extendComponentView({

        type: 'angleAxis',

        render: function (angleAxisDto, ecDto) {
            this.group.removeAll();
            if (!angleAxisDto.get('show')) {
                return;
            }

            var polarDto = ecDto.getComponent('polar', angleAxisDto.get('polarIndex'));
            var angleAxis = angleAxisDto.axis;
            var polar = polarDto.coordinateSystem;
            var radiusExtent = polar.getRadiusAxis().getExtent();
            var ticksAngles = angleAxis.getTicksCoords();

            if (angleAxis.type !== 'category') {
                // Remove the last tick which will overlap the first tick
                ticksAngles.pop();
            }

            zrUtil.each(elementList, function (name) {
                if (angleAxisDto.get(name +'.show')) {
                    this['_' + name](angleAxisDto, polar, ticksAngles, radiusExtent);
                }
            }, this);
        },

        /**
         * @private
         */
        _axisLine: function (angleAxisDto, polar, ticksAngles, radiusExtent) {
            var lineStyleDto = angleAxisDto.getDto('axisLine.lineStyle');

            var circle = new graphic.Circle({
                shape: {
                    cx: polar.cx,
                    cy: polar.cy,
                    r: radiusExtent[1]
                },
                style: lineStyleDto.getLineStyle(),
                z2: 1,
                silent: true
            });
            circle.style.fill = null;

            this.group.add(circle);
        },

        /**
         * @private
         */
        _axisTick: function (angleAxisDto, polar, ticksAngles, radiusExtent) {
            var tickDto = angleAxisDto.getDto('axisTick');

            var tickLen = (tickDto.get('inside') ? -1 : 1) * tickDto.get('length');

            var lines = zrUtil.map(ticksAngles, function (tickAngle) {
                return new graphic.Line({
                    shape: getAxisLineShape(polar, radiusExtent[1], radiusExtent[1] + tickLen, tickAngle)
                });
            });
            this.group.add(graphic.mergePath(
                lines, {
                    style: tickDto.getDto('lineStyle').getLineStyle()
                }
            ));
        },

        /**
         * @private
         */
        _axisLabel: function (angleAxisDto, polar, ticksAngles, radiusExtent) {
            var axis = angleAxisDto.axis;

            var categoryData = angleAxisDto.get('data');

            var labelDto = angleAxisDto.getDto('axisLabel');
            var axisTextStyleDto = labelDto.getDto('textStyle');

            var labels = angleAxisDto.getFormattedLabels();

            var labelMargin = labelDto.get('margin');
            var labelsAngles = axis.getLabelsCoords();

            // Use length of ticksAngles because it may remove the last tick to avoid overlapping
            for (var i = 0; i < ticksAngles.length; i++) {
                var r = radiusExtent[1];
                var p = polar.coordToPoint([r + labelMargin, labelsAngles[i]]);
                var cx = polar.cx;
                var cy = polar.cy;

                var labelTextAlign = Math.abs(p[0] - cx) / r < 0.3
                    ? 'center' : (p[0] > cx ? 'left' : 'right');
                var labelTextBaseline = Math.abs(p[1] - cy) / r < 0.3
                    ? 'middle' : (p[1] > cy ? 'top' : 'bottom');

                var textStyleDto = axisTextStyleDto;
                if (categoryData && categoryData[i] && categoryData[i].textStyle) {
                    textStyleDto = new Dto(
                        categoryData[i].textStyle, axisTextStyleDto
                    );
                }
                this.group.add(new graphic.Text({
                    style: {
                        x: p[0],
                        y: p[1],
                        fill: textStyleDto.getTextColor(),
                        text: labels[i],
                        textAlign: labelTextAlign,
                        textVerticalAlign: labelTextBaseline,
                        textFont: textStyleDto.getFont()
                    },
                    silent: true
                }));
            }
        },

        /**
         * @private
         */
        _splitLine: function (angleAxisDto, polar, ticksAngles, radiusExtent) {
            var splitLineDto = angleAxisDto.getDto('splitLine');
            var lineStyleDto = splitLineDto.getDto('lineStyle');
            var lineColors = lineStyleDto.get('color');
            var lineCount = 0;

            lineColors = lineColors instanceof Array ? lineColors : [lineColors];

            var splitLines = [];

            for (var i = 0; i < ticksAngles.length; i++) {
                var colorIndex = (lineCount++) % lineColors.length;
                splitLines[colorIndex] = splitLines[colorIndex] || [];
                splitLines[colorIndex].push(new graphic.Line({
                    shape: getAxisLineShape(polar, radiusExtent[0], radiusExtent[1], ticksAngles[i])
                }));
            }

            // Simple optimization
            // Batching the lines if color are the same
            for (var i = 0; i < splitLines.length; i++) {
                this.group.add(graphic.mergePath(splitLines[i], {
                    style: zrUtil.defaults({
                        stroke: lineColors[i % lineColors.length]
                    }, lineStyleDto.getLineStyle()),
                    silent: true,
                    z: angleAxisDto.get('z')
                }));
            }
        },

        /**
         * @private
         */
        _splitArea: function (angleAxisDto, polar, ticksAngles, radiusExtent) {

            var splitAreaDto = angleAxisDto.getDto('splitArea');
            var areaStyleDto = splitAreaDto.getDto('areaStyle');
            var areaColors = areaStyleDto.get('color');
            var lineCount = 0;

            areaColors = areaColors instanceof Array ? areaColors : [areaColors];

            var splitAreas = [];

            var RADIAN = Math.PI / 180;
            var prevAngle = -ticksAngles[0] * RADIAN;
            var r0 = Math.min(radiusExtent[0], radiusExtent[1]);
            var r1 = Math.max(radiusExtent[0], radiusExtent[1]);

            var clockwise = angleAxisDto.get('clockwise');

            for (var i = 1; i < ticksAngles.length; i++) {
                var colorIndex = (lineCount++) % areaColors.length;
                splitAreas[colorIndex] = splitAreas[colorIndex] || [];
                splitAreas[colorIndex].push(new graphic.Sector({
                    shape: {
                        cx: polar.cx,
                        cy: polar.cy,
                        r0: r0,
                        r: r1,
                        startAngle: prevAngle,
                        endAngle: -ticksAngles[i] * RADIAN,
                        clockwise: clockwise
                    },
                    silent: true
                }));
                prevAngle = -ticksAngles[i] * RADIAN;
            }

            // Simple optimization
            // Batching the lines if color are the same
            for (var i = 0; i < splitAreas.length; i++) {
                this.group.add(graphic.mergePath(splitAreas[i], {
                    style: zrUtil.defaults({
                        fill: areaColors[i % areaColors.length]
                    }, areaStyleDto.getAreaStyle()),
                    silent: true
                }));
            }
        }
    });
});