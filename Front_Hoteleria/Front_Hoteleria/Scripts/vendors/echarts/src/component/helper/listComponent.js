define(function (require) {
    // List layout
    var layout = require('../../util/layout');
    var formatUtil = require('../../util/format');
    var graphic = require('../../util/graphic');

    function positionGroup(group, Dto, api) {
        layout.positionGroup(
            group, Dto.getBoxLayoutParams(),
            {
                width: api.getWidth(),
                height: api.getHeight()
            },
            Dto.get('padding')
        );
    }

    return {
        /**
         * Layout list like component.
         * It will box layout each items in group of component and then position the whole group in the viewport
         * @param {module:zrender/group/Group} group
         * @param {module:echarts/Dto/Component} componentDto
         * @param {module:echarts/ExtensionAPI}
         */
        layout: function (group, componentDto, api) {
            var rect = layout.getLayoutRect(componentDto.getBoxLayoutParams(), {
                width: api.getWidth(),
                height: api.getHeight()
            }, componentDto.get('padding'));
            layout.box(
                componentDto.get('orient'),
                group,
                componentDto.get('itemGap'),
                rect.width,
                rect.height
            );

            positionGroup(group, componentDto, api);
        },

        addBackground: function (group, componentDto) {
            var padding = formatUtil.normalizeCssArray(
                componentDto.get('padding')
            );
            var boundingRect = group.getBoundingRect();
            var style = componentDto.getItemStyle(['color', 'opacity']);
            style.fill = componentDto.get('backgroundColor');
            var rect = new graphic.Rect({
                shape: {
                    x: boundingRect.x - padding[3],
                    y: boundingRect.y - padding[0],
                    width: boundingRect.width + padding[1] + padding[3],
                    height: boundingRect.height + padding[0] + padding[2]
                },
                style: style,
                silent: true,
                z2: -1
            });
            graphic.subPixelOptimizeRect(rect);

            group.add(rect);
        }
    };
});