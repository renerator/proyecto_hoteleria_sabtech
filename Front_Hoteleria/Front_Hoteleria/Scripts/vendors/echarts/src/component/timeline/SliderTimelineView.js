/**
 * @file Silder timeline view
 */
define(function (require) {

    var zrUtil = require('zrender/core/util');
    var graphic = require('../../util/graphic');
    var layout = require('../../util/layout');
    var TimelineView = require('./TimelineView');
    var TimelineAxis = require('./TimelineAxis');
    var symbolUtil = require('../../util/symbol');
    var axisHelper = require('../../coord/axisHelper');
    var BoundingRect = require('zrender/core/BoundingRect');
    var matrix = require('zrender/core/matrix');
    var numberUtil = require('../../util/number');
    var formatUtil = require('../../util/format');
    var encodeHTML = formatUtil.encodeHTML;

    var bind = zrUtil.bind;
    var each = zrUtil.each;

    var PI = Math.PI;

    return TimelineView.extend({

        type: 'timeline.slider',

        init: function (ecDto, api) {

            this.api = api;

            /**
             * @private
             * @type {module:echarts/component/timeline/TimelineAxis}
             */
            this._axis;

            /**
             * @private
             * @type {module:zrender/core/BoundingRect}
             */
            this._viewRect;

            /**
             * @type {number}
             */
            this._timer;

            /**
             * @type {module:zrende/Element}
             */
            this._currentPointer;

            /**
             * @type {module:zrender/container/Group}
             */
            this._mainGroup;

            /**
             * @type {module:zrender/container/Group}
             */
            this._labelGroup;
        },

        /**
         * @override
         */
        render: function (timelineDto, ecDto, api, payload) {
            this.Dto = timelineDto;
            this.api = api;
            this.ecDto = ecDto;

            this.group.removeAll();

            if (timelineDto.get('show', true)) {

                var layoutInfo = this._layout(timelineDto, api);
                var mainGroup = this._createGroup('mainGroup');
                var labelGroup = this._createGroup('labelGroup');

                /**
                 * @private
                 * @type {module:echarts/component/timeline/TimelineAxis}
                 */
                var axis = this._axis = this._createAxis(layoutInfo, timelineDto);

                timelineDto.formatTooltip = function (dataIndex) {
                    return encodeHTML(axis.scale.getLabel(dataIndex));
                };

                each(
                    ['AxisLine', 'AxisTick', 'Control', 'CurrentPointer'],
                    function (name) {
                        this['_render' + name](layoutInfo, mainGroup, axis, timelineDto);
                    },
                    this
                );

                this._renderAxisLabel(layoutInfo, labelGroup, axis, timelineDto);

                this._position(layoutInfo, timelineDto);
            }

            this._doPlayStop();
        },

        /**
         * @override
         */
        remove: function () {
            this._clearTimer();
            this.group.removeAll();
        },

        /**
         * @override
         */
        dispose: function () {
            this._clearTimer();
        },

        _layout: function (timelineDto, api) {
            var labelPosOpt = timelineDto.get('label.normal.position');
            var orient = timelineDto.get('orient');
            var viewRect = getViewRect(timelineDto, api);
            // Auto label offset.
            if (labelPosOpt == null || labelPosOpt === 'auto') {
                labelPosOpt = orient === 'horizontal'
                    ? ((viewRect.y + viewRect.height / 2) < api.getHeight() / 2 ? '-' : '+')
                    : ((viewRect.x + viewRect.width / 2) < api.getWidth() / 2 ? '+' : '-');
            }
            else if (isNaN(labelPosOpt)) {
                labelPosOpt = ({
                    horizontal: {top: '-', bottom: '+'},
                    vertical: {left: '-', right: '+'}
                })[orient][labelPosOpt];
            }

            // FIXME
            // 暂没有实现用户传入
            // var labelAlign = timelineDto.get('label.normal.textStyle.align');
            // var labelBaseline = timelineDto.get('label.normal.textStyle.baseline');
            var labelAlignMap = {
                horizontal: 'center',
                vertical: (labelPosOpt >= 0 || labelPosOpt === '+') ? 'left' : 'right'
            };

            var labelBaselineMap = {
                horizontal: (labelPosOpt >= 0 || labelPosOpt === '+') ? 'top' : 'bottom',
                vertical: 'middle'
            };
            var rotationMap = {
                horizontal: 0,
                vertical: PI / 2
            };

            // Position
            var mainLength = orient === 'vertical' ? viewRect.height : viewRect.width;

            var controlDto = timelineDto.getDto('controlStyle');
            var showControl = controlDto.get('show');
            var controlSize = showControl ? controlDto.get('itemSize') : 0;
            var controlGap = showControl ? controlDto.get('itemGap') : 0;
            var sizePlusGap = controlSize + controlGap;

            // Special label rotate.
            var labelRotation = timelineDto.get('label.normal.rotate') || 0;
            labelRotation = labelRotation * PI / 180; // To radian.

            var playPosition;
            var prevBtnPosition;
            var nextBtnPosition;
            var axisExtent;
            var controlPosition = controlDto.get('position', true);
            var showControl = controlDto.get('show', true);
            var showPlayBtn = showControl && controlDto.get('showPlayBtn', true);
            var showPrevBtn = showControl && controlDto.get('showPrevBtn', true);
            var showNextBtn = showControl && controlDto.get('showNextBtn', true);
            var xLeft = 0;
            var xRight = mainLength;

            // position[0] means left, position[1] means middle.
            if (controlPosition === 'left' || controlPosition === 'bottom') {
                showPlayBtn && (playPosition = [0, 0], xLeft += sizePlusGap);
                showPrevBtn && (prevBtnPosition = [xLeft, 0], xLeft += sizePlusGap);
                showNextBtn && (nextBtnPosition = [xRight - controlSize, 0], xRight -= sizePlusGap);
            }
            else { // 'top' 'right'
                showPlayBtn && (playPosition = [xRight - controlSize, 0], xRight -= sizePlusGap);
                showPrevBtn && (prevBtnPosition = [0, 0], xLeft += sizePlusGap);
                showNextBtn && (nextBtnPosition = [xRight - controlSize, 0], xRight -= sizePlusGap);
            }
            axisExtent = [xLeft, xRight];

            if (timelineDto.get('inverse')) {
                axisExtent.reverse();
            }

            return {
                viewRect: viewRect,
                mainLength: mainLength,
                orient: orient,

                rotation: rotationMap[orient],
                labelRotation: labelRotation,
                labelPosOpt: labelPosOpt,
                labelAlign: labelAlignMap[orient],
                labelBaseline: labelBaselineMap[orient],

                // Based on mainGroup.
                playPosition: playPosition,
                prevBtnPosition: prevBtnPosition,
                nextBtnPosition: nextBtnPosition,
                axisExtent: axisExtent,

                controlSize: controlSize,
                controlGap: controlGap
            };
        },

        _position: function (layoutInfo, timelineDto) {
            // Position is be called finally, because bounding rect is needed for
            // adapt content to fill viewRect (auto adapt offset).

            // Timeline may be not all in the viewRect when 'offset' is specified
            // as a number, because it is more appropriate that label aligns at
            // 'offset' but not the other edge defined by viewRect.

            var mainGroup = this._mainGroup;
            var labelGroup = this._labelGroup;

            var viewRect = layoutInfo.viewRect;
            if (layoutInfo.orient === 'vertical') {
                // transfrom to horizontal, inverse rotate by left-top point.
                var m = matrix.create();
                var rotateOriginX = viewRect.x;
                var rotateOriginY = viewRect.y + viewRect.height;
                matrix.translate(m, m, [-rotateOriginX, -rotateOriginY]);
                matrix.rotate(m, m, -PI / 2);
                matrix.translate(m, m, [rotateOriginX, rotateOriginY]);
                viewRect = viewRect.clone();
                viewRect.applyTransform(m);
            }

            var viewBound = getBound(viewRect);
            var mainBound = getBound(mainGroup.getBoundingRect());
            var labelBound = getBound(labelGroup.getBoundingRect());

            var mainPosition = mainGroup.position;
            var labelsPosition = labelGroup.position;

            labelsPosition[0] = mainPosition[0] = viewBound[0][0];

            var labelPosOpt = layoutInfo.labelPosOpt;

            if (isNaN(labelPosOpt)) { // '+' or '-'
                var mainBoundIdx = labelPosOpt === '+' ? 0 : 1;
                toBound(mainPosition, mainBound, viewBound, 1, mainBoundIdx);
                toBound(labelsPosition, labelBound, viewBound, 1, 1 - mainBoundIdx);
            }
            else {
                var mainBoundIdx = labelPosOpt >= 0 ? 0 : 1;
                toBound(mainPosition, mainBound, viewBound, 1, mainBoundIdx);
                labelsPosition[1] = mainPosition[1] + labelPosOpt;
            }

            mainGroup.position = mainPosition;
            labelGroup.position = labelsPosition;
            mainGroup.rotation = labelGroup.rotation = layoutInfo.rotation;

            setOrigin(mainGroup);
            setOrigin(labelGroup);

            function setOrigin(targetGroup) {
                var pos = targetGroup.position;
                targetGroup.origin = [
                    viewBound[0][0] - pos[0],
                    viewBound[1][0] - pos[1]
                ];
            }

            function getBound(rect) {
                // [[xmin, xmax], [ymin, ymax]]
                return [
                    [rect.x, rect.x + rect.width],
                    [rect.y, rect.y + rect.height]
                ];
            }

            function toBound(fromPos, from, to, dimIdx, boundIdx) {
                fromPos[dimIdx] += to[dimIdx][boundIdx] - from[dimIdx][boundIdx];
            }
        },

        _createAxis: function (layoutInfo, timelineDto) {
            var data = timelineDto.getData();
            var axisType = timelineDto.get('axisType');

            var scale = axisHelper.createScaleByDto(timelineDto, axisType);
            var dataExtent = data.getDataExtent('value');
            scale.setExtent(dataExtent[0], dataExtent[1]);
            this._customizeScale(scale, data);
            scale.niceTicks();

            var axis = new TimelineAxis('value', scale, layoutInfo.axisExtent, axisType);
            axis.Dto = timelineDto;

            return axis;
        },

        _customizeScale: function (scale, data) {

            scale.getTicks = function () {
                return data.mapArray(['value'], function (value) {
                    return value;
                });
            };

            scale.getTicksLabels = function () {
                return zrUtil.map(this.getTicks(), scale.getLabel, scale);
            };
        },

        _createGroup: function (name) {
            var newGroup = this['_' + name] = new graphic.Group();
            this.group.add(newGroup);
            return newGroup;
        },

        _renderAxisLine: function (layoutInfo, group, axis, timelineDto) {
            var axisExtent = axis.getExtent();

            if (!timelineDto.get('lineStyle.show')) {
                return;
            }

            group.add(new graphic.Line({
                shape: {
                    x1: axisExtent[0], y1: 0,
                    x2: axisExtent[1], y2: 0
                },
                style: zrUtil.extend(
                    {lineCap: 'round'},
                    timelineDto.getDto('lineStyle').getLineStyle()
                ),
                silent: true,
                z2: 1
            }));
        },

        /**
         * @private
         */
        _renderAxisTick: function (layoutInfo, group, axis, timelineDto) {
            var data = timelineDto.getData();
            var ticks = axis.scale.getTicks();

            each(ticks, function (value, dataIndex) {

                var tickCoord = axis.dataToCoord(value);
                var itemDto = data.getItemDto(dataIndex);
                var itemStyleDto = itemDto.getDto('itemStyle.normal');
                var hoverStyleDto = itemDto.getDto('itemStyle.emphasis');
                var symbolOpt = {
                    position: [tickCoord, 0],
                    onclick: bind(this._changeTimeline, this, dataIndex)
                };
                var el = giveSymbol(itemDto, itemStyleDto, group, symbolOpt);
                graphic.setHoverStyle(el, hoverStyleDto.getItemStyle());

                if (itemDto.get('tooltip')) {
                    el.dataIndex = dataIndex;
                    el.dataDto = timelineDto;
                }
                else {
                    el.dataIndex = el.dataDto = null;
                }

            }, this);
        },

        /**
         * @private
         */
        _renderAxisLabel: function (layoutInfo, group, axis, timelineDto) {
            var labelDto = timelineDto.getDto('label.normal');

            if (!labelDto.get('show')) {
                return;
            }

            var data = timelineDto.getData();
            var ticks = axis.scale.getTicks();
            var labels = axisHelper.getFormattedLabels(
                axis, labelDto.get('formatter')
            );
            var labelInterval = axis.getLabelInterval();

            each(ticks, function (tick, dataIndex) {
                if (axis.isLabelIgnored(dataIndex, labelInterval)) {
                    return;
                }

                var itemDto = data.getItemDto(dataIndex);
                var itemTextStyleDto = itemDto.getDto('label.normal.textStyle');
                var hoverTextStyleDto = itemDto.getDto('label.emphasis.textStyle');
                var tickCoord = axis.dataToCoord(tick);
                var textEl = new graphic.Text({
                    style: {
                        text: labels[dataIndex],
                        textAlign: layoutInfo.labelAlign,
                        textVerticalAlign: layoutInfo.labelBaseline,
                        textFont: itemTextStyleDto.getFont(),
                        fill: itemTextStyleDto.getTextColor()
                    },
                    position: [tickCoord, 0],
                    rotation: layoutInfo.labelRotation - layoutInfo.rotation,
                    onclick: bind(this._changeTimeline, this, dataIndex),
                    silent: false
                });

                group.add(textEl);
                graphic.setHoverStyle(textEl, hoverTextStyleDto.getItemStyle());

            }, this);
        },

        /**
         * @private
         */
        _renderControl: function (layoutInfo, group, axis, timelineDto) {
            var controlSize = layoutInfo.controlSize;
            var rotation = layoutInfo.rotation;

            var itemStyle = timelineDto.getDto('controlStyle.normal').getItemStyle();
            var hoverStyle = timelineDto.getDto('controlStyle.emphasis').getItemStyle();
            var rect = [0, -controlSize / 2, controlSize, controlSize];
            var playState = timelineDto.getPlayState();
            var inverse = timelineDto.get('inverse', true);

            makeBtn(
                layoutInfo.nextBtnPosition,
                'controlStyle.nextIcon',
                bind(this._changeTimeline, this, inverse ? '-' : '+')
            );
            makeBtn(
                layoutInfo.prevBtnPosition,
                'controlStyle.prevIcon',
                bind(this._changeTimeline, this, inverse ? '+' : '-')
            );
            makeBtn(
                layoutInfo.playPosition,
                'controlStyle.' + (playState ? 'stopIcon' : 'playIcon'),
                bind(this._handlePlayClick, this, !playState),
                true
            );

            function makeBtn(position, iconPath, onclick, willRotate) {
                if (!position) {
                    return;
                }
                var opt = {
                    position: position,
                    origin: [controlSize / 2, 0],
                    rotation: willRotate ? -rotation : 0,
                    rectHover: true,
                    style: itemStyle,
                    onclick: onclick
                };
                var btn = makeIcon(timelineDto, iconPath, rect, opt);
                group.add(btn);
                graphic.setHoverStyle(btn, hoverStyle);
            }
        },

        _renderCurrentPointer: function (layoutInfo, group, axis, timelineDto) {
            var data = timelineDto.getData();
            var currentIndex = timelineDto.getCurrentIndex();
            var pointerDto = data.getItemDto(currentIndex).getDto('checkpointStyle');
            var me = this;

            var callback = {
                onCreate: function (pointer) {
                    pointer.draggable = true;
                    pointer.drift = bind(me._handlePointerDrag, me);
                    pointer.ondragend = bind(me._handlePointerDragend, me);
                    pointerMoveTo(pointer, currentIndex, axis, timelineDto, true);
                },
                onUpdate: function (pointer) {
                    pointerMoveTo(pointer, currentIndex, axis, timelineDto);
                }
            };

            // Reuse when exists, for animation and drag.
            this._currentPointer = giveSymbol(
                pointerDto, pointerDto, this._mainGroup, {}, this._currentPointer, callback
            );
        },

        _handlePlayClick: function (nextState) {
            this._clearTimer();
            this.api.dispatchAction({
                type: 'timelinePlayChange',
                playState: nextState,
                from: this.uid
            });
        },

        _handlePointerDrag: function (dx, dy, e) {
            this._clearTimer();
            this._pointerChangeTimeline([e.offsetX, e.offsetY]);
        },

        _handlePointerDragend: function (e) {
            this._pointerChangeTimeline([e.offsetX, e.offsetY], true);
        },

        _pointerChangeTimeline: function (mousePos, trigger) {
            var toCoord = this._toAxisCoord(mousePos)[0];

            var axis = this._axis;
            var axisExtent = numberUtil.asc(axis.getExtent().slice());

            toCoord > axisExtent[1] && (toCoord = axisExtent[1]);
            toCoord < axisExtent[0] && (toCoord = axisExtent[0]);

            this._currentPointer.position[0] = toCoord;
            this._currentPointer.dirty();

            var targetDataIndex = this._findNearestTick(toCoord);
            var timelineDto = this.Dto;

            if (trigger || (
                targetDataIndex !== timelineDto.getCurrentIndex()
                && timelineDto.get('realtime')
            )) {
                this._changeTimeline(targetDataIndex);
            }
        },

        _doPlayStop: function () {
            this._clearTimer();

            if (this.Dto.getPlayState()) {
                this._timer = setTimeout(
                    bind(handleFrame, this),
                    this.Dto.get('playInterval')
                );
            }

            function handleFrame() {
                // Do not cache
                var timelineDto = this.Dto;
                this._changeTimeline(
                    timelineDto.getCurrentIndex()
                    + (timelineDto.get('rewind', true) ? -1 : 1)
                );
            }
        },

        _toAxisCoord: function (vertex) {
            var trans = this._mainGroup.getLocalTransform();
            return graphic.applyTransform(vertex, trans, true);
        },

        _findNearestTick: function (axisCoord) {
            var data = this.Dto.getData();
            var dist = Infinity;
            var targetDataIndex;
            var axis = this._axis;

            data.each(['value'], function (value, dataIndex) {
                var coord = axis.dataToCoord(value);
                var d = Math.abs(coord - axisCoord);
                if (d < dist) {
                    dist = d;
                    targetDataIndex = dataIndex;
                }
            });

            return targetDataIndex;
        },

        _clearTimer: function () {
            if (this._timer) {
                clearTimeout(this._timer);
                this._timer = null;
            }
        },

        _changeTimeline: function (nextIndex) {
            var currentIndex = this.Dto.getCurrentIndex();

            if (nextIndex === '+') {
                nextIndex = currentIndex + 1;
            }
            else if (nextIndex === '-') {
                nextIndex = currentIndex - 1;
            }

            this.api.dispatchAction({
                type: 'timelineChange',
                currentIndex: nextIndex,
                from: this.uid
            });
        }

    });

    function getViewRect(Dto, api) {
        return layout.getLayoutRect(
            Dto.getBoxLayoutParams(),
            {
                width: api.getWidth(),
                height: api.getHeight()
            },
            Dto.get('padding')
        );
    }

    function makeIcon(timelineDto, objPath, rect, opts) {
        var icon = graphic.makePath(
            timelineDto.get(objPath).replace(/^path:\/\//, ''),
            zrUtil.clone(opts || {}),
            new BoundingRect(rect[0], rect[1], rect[2], rect[3]),
            'center'
        );

        return icon;
    }

    /**
     * Create symbol or update symbol
     */
    function giveSymbol(hostDto, itemStyleDto, group, opt, symbol, callback) {
        var symbolType = hostDto.get('symbol');
        var color = itemStyleDto.get('color');
        var symbolSize = hostDto.get('symbolSize');
        var halfSymbolSize = symbolSize / 2;
        var itemStyle = itemStyleDto.getItemStyle(['color', 'symbol', 'symbolSize']);

        if (!symbol) {
            symbol = symbolUtil.createSymbol(
                symbolType, -halfSymbolSize, -halfSymbolSize, symbolSize, symbolSize, color
            );
            group.add(symbol);
            callback && callback.onCreate(symbol);
        }
        else {
            symbol.setStyle(itemStyle);
            symbol.setColor(color);
            group.add(symbol); // Group may be new, also need to add.
            callback && callback.onUpdate(symbol);
        }

        opt = zrUtil.merge({
            rectHover: true,
            style: itemStyle,
            z2: 100
        }, opt, true);

        symbol.attr(opt);

        return symbol;
    }

    function pointerMoveTo(pointer, dataIndex, axis, timelineDto, noAnimation) {
        if (pointer.dragging) {
            return;
        }

        var pointerDto = timelineDto.getDto('checkpointStyle');
        var toCoord = axis.dataToCoord(timelineDto.getData().get(['value'], dataIndex));

        if (noAnimation || !pointerDto.get('animation', true)) {
            pointer.attr({position: [toCoord, 0]});
        }
        else {
            pointer.stopAnimation(true);
            pointer.animateTo(
                {position: [toCoord, 0]},
                pointerDto.get('animationDuration', true),
                pointerDto.get('animationEasing', true)
            );
        }
    }

});