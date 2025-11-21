define(function (require) {

    'use strict';

    var zrUtil = require('zrender/core/util');
    var graphic = require('../../util/graphic');
    var AxisBuilder = require('./AxisBuilder');

    var axisBuilderAttrs = [
        'axisLine', 'axisLabel', 'axisTick', 'axisName'
    ];
    var selfBuilderAttrs = [
        'splitLine', 'splitArea'
    ];

    require('../../echarts').extendComponentView({

        type: 'radiusAxis',

        render: function (radiusAxisDto, ecDto) {
            this.group.removeAll();
            if (!radiusAxisDto.get('show')) {
                return;
            }
            var polarDto = ecDto.getComponent('polar', radiusAxisDto.get('polarIndex'));
            var angleAxis = polarDto.coordinateSystem.getAngleAxis();
            var radiusAxis = radiusAxisDto.axis;
            var polar = polarDto.coordinateSystem;
            var ticksCoords = radiusAxis.getTicksCoords();
            var axisAngle = angleAxis.getExtent()[0];
            var radiusExtent = radiusAxis.getExtent();

            var layout = layoutAxis(polar, radiusAxisDto, axisAngle);
            var axisBuilder = new AxisBuilder(radiusAxisDto, layout);
            zrUtil.each(axisBuilderAttrs, axisBuilder.add, axisBuilder);
            this.group.add(axisBuilder.getGroup());

            zrUtil.each(selfBuilderAttrs, function (name) {
                if (radiusAxisDto.get(name +'.show')) {
                    this['_' + name](radiusAxisDto, polar, axisAngle, radiusExtent, ticksCoords);
                }
            }, this);
        },

        /**
         * @private
         */
        _splitLine: function (radiusAxisDto, polar, axisAngle, radiusExtent, ticksCoords) {
            var splitLineDto = radiusAxisDto.getDto('splitLine');
            var lineStyleDto = splitLineDto.getDto('lineStyle');
            var lineColors = lineStyleDto.get('color');
            var lineCount = 0;

            lineColors = lineColors instanceof Array ? lineColors : [lineColors];

            var splitLines = [];

            for (var i = 0; i < ticksCoords.length; i++) {
                var colorIndex = (lineCount++) % lineColors.length;
                splitLines[colorIndex] = splitLines[colorIndex] || [];
                splitLines[colorIndex].push(new graphic.Circle({
                    shape: {
                        cx: polar.cx,
                        cy: polar.cy,
                        r: ticksCoords[i]
                    },
                    silent: true
                }));
            }

            // Simple optimization
            // Batching the lines if color are the same
            for (var i = 0; i < splitLines.length; i++) {
                this.group.add(graphic.mergePath(splitLines[i], {
                    style: zrUtil.defaults({
                        stroke: lineColors[i % lineColors.length],
                        fill: null
                    }, lineStyleDto.getLineStyle()),
                    silent: true
                }));
            }
        },

        /**
         * @private
         */
        _splitArea: function (radiusAxisDto, polar, axisAngle, radiusExtent, ticksCoords) {

            var splitAreaDto = radiusAxisDto.getDto('splitArea');
            var areaStyleDto = splitAreaDto.getDto('areaStyle');
            var areaColors = areaStyleDto.get('color');
            var lineCount = 0;

            areaColors = areaColors instanceof Array ? areaColors : [areaColors];

            var splitAreas = [];

            var prevRadius = ticksCoords[0];
            for (var i = 1; i < ticksCoords.length; i++) {
                var colorIndex = (lineCount++) % areaColors.length;
                splitAreas[colorIndex] = splitAreas[colorIndex] || [];
                splitAreas[colorIndex].push(new graphic.Sector({
                    shape: {
                        cx: polar.cx,
                        cy: polar.cy,
                        r0: prevRadius,
                        r: ticksCoords[i],
                        startAngle: 0,
                        endAngle: Math.PI * 2
                    },
                    silent: true
                }));
                prevRadius = ticksCoords[i];
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

    /**
     * @inner
     */
    function layoutAxis(polar, radiusAxisDto, axisAngle) {
        return {
            position: [polar.cx, polar.cy],
            rotation: axisAngle / 180 * Math.PI,
            labelDirection: -1,
            tickDirection: -1,
            nameDirection: 1,
            labelRotation: radiusAxisDto.getDto('axisLabel').get('rotate'),
            // Over splitLine and splitArea
            z2: 1
        };
    }
});