 define(function(require) {

    var graphic = require('../../util/graphic');
    var layout = require('../../util/layout');
    var zrUtil = require('zrender/core/util');

    var TEXT_PADDING = 8;
    var ITEM_GAP = 8;
    var ARRAY_LENGTH = 5;

    function Breadcrumb(containerGroup, onSelect) {
        /**
         * @private
         * @type {module:zrender/container/Group}
         */
        this.group = new graphic.Group();

        containerGroup.add(this.group);

        /**
         * @private
         * @type {Function}
         */
        this._onSelect = onSelect || zrUtil.noop;
    }

    Breadcrumb.prototype = {

        constructor: Breadcrumb,

        render: function (seriesDto, api, targetNode) {
            var Dto = seriesDto.getDto('breadcrumb');
            var thisGroup = this.group;

            thisGroup.removeAll();

            if (!Dto.get('show') || !targetNode) {
                return;
            }

            var normalStyleDto = Dto.getDto('itemStyle.normal');
            // var emphasisStyleDto = Dto.getDto('itemStyle.emphasis');
            var textStyleDto = normalStyleDto.getDto('textStyle');

            var layoutParam = {
                pos: {
                    left: Dto.get('left'),
                    right: Dto.get('right'),
                    top: Dto.get('top'),
                    bottom: Dto.get('bottom')
                },
                box: {
                    width: api.getWidth(),
                    height: api.getHeight()
                },
                emptyItemWidth: Dto.get('emptyItemWidth'),
                totalWidth: 0,
                renderList: []
            };

            this._prepare(
                Dto, targetNode, layoutParam, textStyleDto
            );
            this._renderContent(
                Dto, targetNode, layoutParam, normalStyleDto, textStyleDto
            );

            layout.positionGroup(thisGroup, layoutParam.pos, layoutParam.box);
        },

        /**
         * Prepare render list and total width
         * @private
         */
        _prepare: function (Dto, targetNode, layoutParam, textStyleDto) {
            for (var node = targetNode; node; node = node.parentNode) {
                var text = node.getDto().get('name');
                var textRect = textStyleDto.getTextRect(text);
                var itemWidth = Math.max(
                    textRect.width + TEXT_PADDING * 2,
                    layoutParam.emptyItemWidth
                );
                layoutParam.totalWidth += itemWidth + ITEM_GAP;
                layoutParam.renderList.push({node: node, text: text, width: itemWidth});
            }
        },

        /**
         * @private
         */
        _renderContent: function (
            Dto, targetNode, layoutParam, normalStyleDto, textStyleDto
        ) {
            // Start rendering.
            var lastX = 0;
            var emptyItemWidth = layoutParam.emptyItemWidth;
            var height = Dto.get('height');
            var availableSize = layout.getAvailableSize(layoutParam.pos, layoutParam.box);
            var totalWidth = layoutParam.totalWidth;
            var renderList = layoutParam.renderList;

            for (var i = renderList.length - 1; i >= 0; i--) {
                var item = renderList[i];
                var itemWidth = item.width;
                var text = item.text;

                // Hdie text and shorten width if necessary.
                if (totalWidth > availableSize.width) {
                    totalWidth -= itemWidth - emptyItemWidth;
                    itemWidth = emptyItemWidth;
                    text = '';
                }

                this.group.add(new graphic.Polygon({
                    shape: {
                        points: makeItemPoints(
                            lastX, 0, itemWidth, height,
                            i === renderList.length - 1, i === 0
                        )
                    },
                    style: zrUtil.defaults(
                        normalStyleDto.getItemStyle(),
                        {
                            lineJoin: 'bevel',
                            text: text,
                            textFill: textStyleDto.getTextColor(),
                            textFont: textStyleDto.getFont()
                        }
                    ),
                    z: 10,
                    onclick: zrUtil.bind(this._onSelect, this, item.node)
                }));

                lastX += itemWidth + ITEM_GAP;
            }
        },

        /**
         * @override
         */
        remove: function () {
            this.group.removeAll();
        }
    };

    function makeItemPoints(x, y, itemWidth, itemHeight, head, tail) {
        var points = [
            [head ? x : x - ARRAY_LENGTH, y],
            [x + itemWidth, y],
            [x + itemWidth, y + itemHeight],
            [head ? x : x - ARRAY_LENGTH, y + itemHeight]
        ];
        !tail && points.splice(2, 0, [x + itemWidth + ARRAY_LENGTH, y + itemHeight / 2]);
        !head && points.push([x, y + itemHeight / 2]);
        return points;
    }

    return Breadcrumb;
});