define(function (require) {

    var zrUtil = require('zrender/core/util');
    var graphic = require('../../util/graphic');
    var Dto = require('../../Dto/Dto');
    var numberUtil = require('../../util/number');
    var remRadian = numberUtil.remRadian;
    var isRadianAroundZero = numberUtil.isRadianAroundZero;

    var PI = Math.PI;

    function makeAxisEventDataBase(axisDto) {
        var eventData = {
            componentType: axisDto.mainType
        };
        eventData[axisDto.mainType + 'Index'] = axisDto.componentIndex;
        return eventData;
    }

    /**
     * A final axis is translated and rotated from a "standard axis".
     * So opt.position and opt.rotation is required.
     *
     * A standard axis is and axis from [0, 0] to [0, axisExtent[1]],
     * for example: (0, 0) ------------> (0, 50)
     *
     * nameDirection or tickDirection or labelDirection is 1 means tick
     * or label is below the standard axis, whereas is -1 means above
     * the standard axis. labelOffset means offset between label and axis,
     * which is useful when 'onZero', where axisLabel is in the grid and
     * label in outside grid.
     *
     * Tips: like always,
     * positive rotation represents anticlockwise, and negative rotation
     * represents clockwise.
     * The direction of position coordinate is the same as the direction
     * of screen coordinate.
     *
     * Do not need to consider axis 'inverse', which is auto processed by
     * axis extent.
     *
     * @param {module:zrender/container/Group} group
     * @param {Object} axisDto
     * @param {Object} opt Standard axis parameters.
     * @param {Array.<number>} opt.position [x, y]
     * @param {number} opt.rotation by radian
     * @param {number} [opt.nameDirection=1] 1 or -1 Used when nameLocation is 'middle'.
     * @param {number} [opt.tickDirection=1] 1 or -1
     * @param {number} [opt.labelDirection=1] 1 or -1
     * @param {number} [opt.labelOffset=0] Usefull when onZero.
     * @param {string} [opt.axisName] default get from axisDto.
     * @param {number} [opt.labelRotation] by degree, default get from axisDto.
     * @param {number} [opt.labelInterval] Default label interval when label
     *                                     interval from Dto is null or 'auto'.
     * @param {number} [opt.strokeContainThreshold] Default label interval when label
     * @param {number} [opt.axisLineSilent=true] If axis line is silent
     */
    var AxisBuilder = function (axisDto, opt) {

        /**
         * @readOnly
         */
        this.opt = opt;

        /**
         * @readOnly
         */
        this.axisDto = axisDto;

        // Default value
        zrUtil.defaults(
            opt,
            {
                labelOffset: 0,
                nameDirection: 1,
                tickDirection: 1,
                labelDirection: 1,
                silent: true
            }
        );

        /**
         * @readOnly
         */
        this.group = new graphic.Group({
            position: opt.position.slice(),
            rotation: opt.rotation
        });
    };

    AxisBuilder.prototype = {

        constructor: AxisBuilder,

        hasBuilder: function (name) {
            return !!builders[name];
        },

        add: function (name) {
            builders[name].call(this);
        },

        getGroup: function () {
            return this.group;
        }

    };

    var builders = {

        /**
         * @private
         */
        axisLine: function () {
            var opt = this.opt;
            var axisDto = this.axisDto;

            if (!axisDto.get('axisLine.show')) {
                return;
            }

            var extent = this.axisDto.axis.getExtent();

            this.group.add(new graphic.Line({
                shape: {
                    x1: extent[0],
                    y1: 0,
                    x2: extent[1],
                    y2: 0
                },
                style: zrUtil.extend(
                    {lineCap: 'round'},
                    axisDto.getDto('axisLine.lineStyle').getLineStyle()
                ),
                strokeContainThreshold: opt.strokeContainThreshold,
                silent: !!opt.axisLineSilent,
                z2: 1
            }));
        },

        /**
         * @private
         */
        axisTick: function () {
            var axisDto = this.axisDto;

            if (!axisDto.get('axisTick.show')) {
                return;
            }

            var axis = axisDto.axis;
            var tickDto = axisDto.getDto('axisTick');
            var opt = this.opt;

            var lineStyleDto = tickDto.getDto('lineStyle');
            var tickLen = tickDto.get('length');
            var tickInterval = getInterval(tickDto, opt.labelInterval);
            var ticksCoords = axis.getTicksCoords();
            var tickLines = [];

            for (var i = 0; i < ticksCoords.length; i++) {
                // Only ordinal scale support tick interval
                if (ifIgnoreOnTick(axis, i, tickInterval)) {
                     continue;
                }

                var tickCoord = ticksCoords[i];

                // Tick line
                tickLines.push(new graphic.Line(graphic.subPixelOptimizeLine({
                    shape: {
                        x1: tickCoord,
                        y1: 0,
                        x2: tickCoord,
                        y2: opt.tickDirection * tickLen
                    },
                    style: {
                        lineWidth: lineStyleDto.get('width')
                    },
                    silent: true
                })));
            }

            this.group.add(graphic.mergePath(tickLines, {
                style: lineStyleDto.getLineStyle(),
                z2: 2,
                silent: true
            }));
        },

        /**
         * @param {module:echarts/coord/cartesian/AxisDto} axisDto
         * @param {module:echarts/coord/cartesian/GridDto} gridDto
         * @private
         */
        axisLabel: function () {
            var axisDto = this.axisDto;

            if (!axisDto.get('axisLabel.show')) {
                return;
            }

            var opt = this.opt;
            var axis = axisDto.axis;
            var labelDto = axisDto.getDto('axisLabel');
            var textStyleDto = labelDto.getDto('textStyle');
            var labelMargin = labelDto.get('margin');
            var ticks = axis.scale.getTicks();
            var labels = axisDto.getFormattedLabels();

            // Special label rotate.
            var labelRotation = opt.labelRotation;
            if (labelRotation == null) {
                labelRotation = labelDto.get('rotate') || 0;
            }
            // To radian.
            labelRotation = labelRotation * PI / 180;

            var labelLayout = innerTextLayout(opt, labelRotation, opt.labelDirection);
            var categoryData = axisDto.get('data');

            var textEls = [];
            var isSilent = axisDto.get('silent');
            for (var i = 0; i < ticks.length; i++) {
                if (ifIgnoreOnTick(axis, i, opt.labelInterval)) {
                     continue;
                }

                var itemTextStyleDto = textStyleDto;
                if (categoryData && categoryData[i] && categoryData[i].textStyle) {
                    itemTextStyleDto = new Dto(
                        categoryData[i].textStyle, textStyleDto, axisDto.ecDto
                    );
                }
                var textColor = itemTextStyleDto.getTextColor();

                var tickCoord = axis.dataToCoord(ticks[i]);
                var pos = [
                    tickCoord,
                    opt.labelOffset + opt.labelDirection * labelMargin
                ];
                var labelBeforeFormat = axis.scale.getLabel(ticks[i]);

                var textEl = new graphic.Text({
                    style: {
                        text: labels[i],
                        textAlign: itemTextStyleDto.get('align', true) || labelLayout.textAlign,
                        textVerticalAlign: itemTextStyleDto.get('baseline', true) || labelLayout.verticalAlign,
                        textFont: itemTextStyleDto.getFont(),
                        fill: typeof textColor === 'function' ? textColor(labelBeforeFormat) : textColor
                    },
                    position: pos,
                    rotation: labelLayout.rotation,
                    silent: isSilent,
                    z2: 10
                });
                // Pack data for mouse event
                textEl.eventData = makeAxisEventDataBase(axisDto);
                textEl.eventData.targetType = 'axisLabel';
                textEl.eventData.value = labelBeforeFormat;

                textEls.push(textEl);
                this.group.add(textEl);
            }

            function isTwoLabelOverlapped(current, next) {
                var firstRect = current && current.getBoundingRect().clone();
                var nextRect = next && next.getBoundingRect().clone();
                if (firstRect && nextRect) {
                    firstRect.applyTransform(current.getLocalTransform());
                    nextRect.applyTransform(next.getLocalTransform());
                    return firstRect.intersect(nextRect);
                }
            }
            if (axis.type !== 'category') {
                // If min or max are user set, we need to check
                // If the tick on min(max) are overlap on their neighbour tick
                // If they are overlapped, we need to hide the min(max) tick label
                if (axisDto.getMin ? axisDto.getMin() : axisDto.get('min')) {
                    var firstLabel = textEls[0];
                    var nextLabel = textEls[1];
                    if (isTwoLabelOverlapped(firstLabel, nextLabel)) {
                        firstLabel.ignore = true;
                    }
                }
                if (axisDto.getMax ? axisDto.getMax() : axisDto.get('max')) {
                    var lastLabel = textEls[textEls.length - 1];
                    var prevLabel = textEls[textEls.length - 2];
                    if (isTwoLabelOverlapped(prevLabel, lastLabel)) {
                        lastLabel.ignore = true;
                    }
                }
            }
        },

        /**
         * @private
         */
        axisName: function () {
            var opt = this.opt;
            var axisDto = this.axisDto;

            var name = this.opt.axisName;
            // If name is '', do not get name from axisMode.
            if (name == null) {
                name = axisDto.get('name');
            }

            if (!name) {
                return;
            }

            var nameLocation = axisDto.get('nameLocation');
            var nameDirection = opt.nameDirection;
            var textStyleDto = axisDto.getDto('nameTextStyle');
            var gap = axisDto.get('nameGap') || 0;

            var extent = this.axisDto.axis.getExtent();
            var gapSignal = extent[0] > extent[1] ? -1 : 1;
            var pos = [
                nameLocation === 'start'
                    ? extent[0] - gapSignal * gap
                    : nameLocation === 'end'
                    ? extent[1] + gapSignal * gap
                    : (extent[0] + extent[1]) / 2, // 'middle'
                // Reuse labelOffset.
                nameLocation === 'middle' ? opt.labelOffset + nameDirection * gap : 0
            ];

            var labelLayout;

            if (nameLocation === 'middle') {
                labelLayout = innerTextLayout(opt, opt.rotation, nameDirection);
            }
            else {
                labelLayout = endTextLayout(opt, nameLocation, extent);
            }

            var textEl = new graphic.Text({
                style: {
                    text: name,
                    textFont: textStyleDto.getFont(),
                    fill: textStyleDto.getTextColor()
                        || axisDto.get('axisLine.lineStyle.color'),
                    textAlign: labelLayout.textAlign,
                    textVerticalAlign: labelLayout.verticalAlign
                },
                position: pos,
                rotation: labelLayout.rotation,
                silent: axisDto.get('silent'),
                z2: 1
            });

            textEl.eventData = makeAxisEventDataBase(axisDto);
            textEl.eventData.targetType = 'axisName';
            textEl.eventData.name = name;

            this.group.add(textEl);
        }

    };

    /**
     * @inner
     */
    function innerTextLayout(opt, textRotation, direction) {
        var rotationDiff = remRadian(textRotation - opt.rotation);
        var textAlign;
        var verticalAlign;

        if (isRadianAroundZero(rotationDiff)) { // Label is parallel with axis line.
            verticalAlign = direction > 0 ? 'top' : 'bottom';
            textAlign = 'center';
        }
        else if (isRadianAroundZero(rotationDiff - PI)) { // Label is inverse parallel with axis line.
            verticalAlign = direction > 0 ? 'bottom' : 'top';
            textAlign = 'center';
        }
        else {
            verticalAlign = 'middle';

            if (rotationDiff > 0 && rotationDiff < PI) {
                textAlign = direction > 0 ? 'right' : 'left';
            }
            else {
                textAlign = direction > 0 ? 'left' : 'right';
            }
        }

        return {
            rotation: rotationDiff,
            textAlign: textAlign,
            verticalAlign: verticalAlign
        };
    }

    /**
     * @inner
     */
    function endTextLayout(opt, textPosition, extent) {
        var rotationDiff = remRadian(-opt.rotation);
        var textAlign;
        var verticalAlign;
        var inverse = extent[0] > extent[1];
        var onLeft = (textPosition === 'start' && !inverse)
            || (textPosition !== 'start' && inverse);

        if (isRadianAroundZero(rotationDiff - PI / 2)) {
            verticalAlign = onLeft ? 'bottom' : 'top';
            textAlign = 'center';
        }
        else if (isRadianAroundZero(rotationDiff - PI * 1.5)) {
            verticalAlign = onLeft ? 'top' : 'bottom';
            textAlign = 'center';
        }
        else {
            verticalAlign = 'middle';
            if (rotationDiff < PI * 1.5 && rotationDiff > PI / 2) {
                textAlign = onLeft ? 'left' : 'right';
            }
            else {
                textAlign = onLeft ? 'right' : 'left';
            }
        }

        return {
            rotation: rotationDiff,
            textAlign: textAlign,
            verticalAlign: verticalAlign
        };
    }

    /**
     * @static
     */
    var ifIgnoreOnTick = AxisBuilder.ifIgnoreOnTick = function (axis, i, interval) {
        var rawTick;
        var scale = axis.scale;
        return scale.type === 'ordinal'
            && (
                typeof interval === 'function'
                    ? (
                        rawTick = scale.getTicks()[i],
                        !interval(rawTick, scale.getLabel(rawTick))
                    )
                    : i % (interval + 1)
            );
    };

    /**
     * @static
     */
    var getInterval = AxisBuilder.getInterval = function (Dto, labelInterval) {
        var interval = Dto.get('interval');
        if (interval == null || interval == 'auto') {
            interval = labelInterval;
        }
        return interval;
    };

    return AxisBuilder;

});