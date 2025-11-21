define(function (require) {

    var zrUtil = require('zrender/core/util');
    var graphic = require('../../util/graphic');
    var AxisBuilder = require('./AxisBuilder');
    var ifIgnoreOnTick = AxisBuilder.ifIgnoreOnTick;
    var getInterval = AxisBuilder.getInterval;

    var axisBuilderAttrs = [
        'axisLine', 'axisLabel', 'axisTick', 'axisName'
    ];
    var selfBuilderAttrs = [
        'splitLine', 'splitArea'
    ];

    var AxisView = require('../../echarts').extendComponentView({

        type: 'axis',

        render: function (axisDto, ecDto) {

            this.group.removeAll();

            if (!axisDto.get('show')) {
                return;
            }

            var gridDto = ecDto.getComponent('grid', axisDto.get('gridIndex'));

            var layout = layoutAxis(gridDto, axisDto);

            var axisBuilder = new AxisBuilder(axisDto, layout);

            zrUtil.each(axisBuilderAttrs, axisBuilder.add, axisBuilder);

            this.group.add(axisBuilder.getGroup());

            zrUtil.each(selfBuilderAttrs, function (name) {
                if (axisDto.get(name +'.show')) {
                    this['_' + name](axisDto, gridDto, layout.labelInterval);
                }
            }, this);
        },

        /**
         * @param {module:echarts/coord/cartesian/AxisDto} axisDto
         * @param {module:echarts/coord/cartesian/GridDto} gridDto
         * @param {number|Function} labelInterval
         * @private
         */
        _splitLine: function (axisDto, gridDto, labelInterval) {
            var axis = axisDto.axis;

            var splitLineDto = axisDto.getDto('splitLine');
            var lineStyleDto = splitLineDto.getDto('lineStyle');
            var lineWidth = lineStyleDto.get('width');
            var lineColors = lineStyleDto.get('color');

            var lineInterval = getInterval(splitLineDto, labelInterval);

            lineColors = zrUtil.isArray(lineColors) ? lineColors : [lineColors];

            var gridRect = gridDto.coordinateSystem.getRect();
            var isHorizontal = axis.isHorizontal();

            var splitLines = [];
            var lineCount = 0;

            var ticksCoords = axis.getTicksCoords();

            var p1 = [];
            var p2 = [];
            for (var i = 0; i < ticksCoords.length; i++) {
                if (ifIgnoreOnTick(axis, i, lineInterval)) {
                    continue;
                }

                var tickCoord = axis.toGlobalCoord(ticksCoords[i]);

                if (isHorizontal) {
                    p1[0] = tickCoord;
                    p1[1] = gridRect.y;
                    p2[0] = tickCoord;
                    p2[1] = gridRect.y + gridRect.height;
                }
                else {
                    p1[0] = gridRect.x;
                    p1[1] = tickCoord;
                    p2[0] = gridRect.x + gridRect.width;
                    p2[1] = tickCoord;
                }

                var colorIndex = (lineCount++) % lineColors.length;
                splitLines[colorIndex] = splitLines[colorIndex] || [];
                splitLines[colorIndex].push(new graphic.Line(graphic.subPixelOptimizeLine({
                    shape: {
                        x1: p1[0],
                        y1: p1[1],
                        x2: p2[0],
                        y2: p2[1]
                    },
                    style: {
                        lineWidth: lineWidth
                    },
                    silent: true
                })));
            }

            // Simple optimization
            // Batching the lines if color are the same
            var lineStyle = lineStyleDto.getLineStyle();
            for (var i = 0; i < splitLines.length; i++) {
                this.group.add(graphic.mergePath(splitLines[i], {
                    style: zrUtil.defaults({
                        stroke: lineColors[i % lineColors.length]
                    }, lineStyle),
                    silent: true
                }));
            }
        },

        /**
         * @param {module:echarts/coord/cartesian/AxisDto} axisDto
         * @param {module:echarts/coord/cartesian/GridDto} gridDto
         * @param {number|Function} labelInterval
         * @private
         */
        _splitArea: function (axisDto, gridDto, labelInterval) {
            var axis = axisDto.axis;

            var splitAreaDto = axisDto.getDto('splitArea');
            var areaStyleDto = splitAreaDto.getDto('areaStyle');
            var areaColors = areaStyleDto.get('color');

            var gridRect = gridDto.coordinateSystem.getRect();
            var ticksCoords = axis.getTicksCoords();

            var prevX = axis.toGlobalCoord(ticksCoords[0]);
            var prevY = axis.toGlobalCoord(ticksCoords[0]);

            var splitAreaRects = [];
            var count = 0;

            var areaInterval = getInterval(splitAreaDto, labelInterval);

            areaColors = zrUtil.isArray(areaColors) ? areaColors : [areaColors];

            for (var i = 1; i < ticksCoords.length; i++) {
                if (ifIgnoreOnTick(axis, i, areaInterval)) {
                    continue;
                }

                var tickCoord = axis.toGlobalCoord(ticksCoords[i]);

                var x;
                var y;
                var width;
                var height;
                if (axis.isHorizontal()) {
                    x = prevX;
                    y = gridRect.y;
                    width = tickCoord - x;
                    height = gridRect.height;
                }
                else {
                    x = gridRect.x;
                    y = prevY;
                    width = gridRect.width;
                    height = tickCoord - y;
                }

                var colorIndex = (count++) % areaColors.length;
                splitAreaRects[colorIndex] = splitAreaRects[colorIndex] || [];
                splitAreaRects[colorIndex].push(new graphic.Rect({
                    shape: {
                        x: x,
                        y: y,
                        width: width,
                        height: height
                    },
                    silent: true
                }));

                prevX = x + width;
                prevY = y + height;
            }

            // Simple optimization
            // Batching the rects if color are the same
            var areaStyle = areaStyleDto.getAreaStyle();
            for (var i = 0; i < splitAreaRects.length; i++) {
                this.group.add(graphic.mergePath(splitAreaRects[i], {
                    style: zrUtil.defaults({
                        fill: areaColors[i % areaColors.length]
                    }, areaStyle),
                    silent: true
                }));
            }
        }
    });

    AxisView.extend({
        type: 'xAxis'
    });
    AxisView.extend({
        type: 'yAxis'
    });

    /**
     * @inner
     */
    function layoutAxis(gridDto, axisDto) {
        var grid = gridDto.coordinateSystem;
        var axis = axisDto.axis;
        var layout = {};

        var rawAxisPosition = axis.position;
        var axisPosition = axis.onZero ? 'onZero' : rawAxisPosition;
        var axisDim = axis.dim;

        // [left, right, top, bottom]
        var rect = grid.getRect();
        var rectBound = [rect.x, rect.x + rect.width, rect.y, rect.y + rect.height];

        var posMap = {
            x: {top: rectBound[2], bottom: rectBound[3]},
            y: {left: rectBound[0], right: rectBound[1]}
        };
        posMap.x.onZero = Math.max(Math.min(getZero('y'), posMap.x.bottom), posMap.x.top);
        posMap.y.onZero = Math.max(Math.min(getZero('x'), posMap.y.right), posMap.y.left);

        function getZero(dim, val) {
            var theAxis = grid.getAxis(dim);
            return theAxis.toGlobalCoord(theAxis.dataToCoord(0));
        }

        // Axis position
        layout.position = [
            axisDim === 'y' ? posMap.y[axisPosition] : rectBound[0],
            axisDim === 'x' ? posMap.x[axisPosition] : rectBound[3]
        ];

        // Axis rotation
        var r = {x: 0, y: 1};
        layout.rotation = Math.PI / 2 * r[axisDim];

        // Tick and label direction, x y is axisDim
        var dirMap = {top: -1, bottom: 1, left: -1, right: 1};

        layout.labelDirection = layout.tickDirection = layout.nameDirection = dirMap[rawAxisPosition];
        if (axis.onZero) {
            layout.labelOffset = posMap[axisDim][rawAxisPosition] - posMap[axisDim].onZero;
        }

        if (axisDto.getDto('axisTick').get('inside')) {
            layout.tickDirection = -layout.tickDirection;
        }
        if (axisDto.getDto('axisLabel').get('inside')) {
            layout.labelDirection = -layout.labelDirection;
        }

        // Special label rotation
        var labelRotation = axisDto.getDto('axisLabel').get('rotate');
        layout.labelRotation = axisPosition === 'top' ? -labelRotation : labelRotation;

        // label interval when auto mode.
        layout.labelInterval = axis.getLabelInterval();

        // Over splitLine and splitArea
        layout.z2 = 1;

        return layout;
    }
});