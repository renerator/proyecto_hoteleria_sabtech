define(function (require) {

    // var zrUtil = require('zrender/core/util');
    var graphic = require('../../util/graphic');

    var MapDraw = require('../../component/helper/MapDraw');

    require('../../echarts').extendChartView({

        type: 'map',

        render: function (mapDto, ecDto, api, payload) {
            // Not render if it is an toggleSelect action from self
            if (payload && payload.type === 'mapToggleSelect'
                && payload.from === this.uid
            ) {
                return;
            }

            var group = this.group;
            group.removeAll();
            // Not update map if it is an roam action from self
            if (!(payload && payload.type === 'geoRoam'
                && payload.component === 'series'
                && payload.name === mapDto.name)) {

                if (mapDto.needsDrawMap) {
                    var mapDraw = this._mapDraw || new MapDraw(api, true);
                    group.add(mapDraw.group);

                    mapDraw.draw(mapDto, ecDto, api, this, payload);

                    this._mapDraw = mapDraw;
                }
                else {
                    // Remove drawed map
                    this._mapDraw && this._mapDraw.remove();
                    this._mapDraw = null;
                }
            }
            else {
                var mapDraw = this._mapDraw;
                mapDraw && group.add(mapDraw.group);
            }

            mapDto.get('showLegendSymbol') && ecDto.getComponent('legend')
                && this._renderSymbols(mapDto, ecDto, api);
        },

        remove: function () {
            this._mapDraw && this._mapDraw.remove();
            this._mapDraw = null;
            this.group.removeAll();
        },

        _renderSymbols: function (mapDto, ecDto, api) {
            var data = mapDto.getData();
            var group = this.group;

            data.each('value', function (value, idx) {
                if (isNaN(value)) {
                    return;
                }

                var layout = data.getItemLayout(idx);

                if (!layout || !layout.point) {
                    // Not exists in map
                    return;
                }

                var point = layout.point;
                var offset = layout.offset;

                var circle = new graphic.Circle({
                    style: {
                        fill: data.getVisual('color')
                    },
                    shape: {
                        cx: point[0] + offset * 9,
                        cy: point[1],
                        r: 3
                    },
                    silent: true,
                    z2: 10
                });

                // First data on the same region
                if (!offset) {
                    var labelText = data.getName(idx);

                    var itemDto = data.getItemDto(idx);
                    var labelDto = itemDto.getDto('label.normal');
                    var hoverLabelDto = itemDto.getDto('label.emphasis');

                    var textStyleDto = labelDto.getDto('textStyle');
                    var hoverTextStyleDto = hoverLabelDto.getDto('textStyle');

                    var polygonGroups = data.getItemGraphicEl(idx);
                    circle.setStyle({
                        textPosition: 'bottom'
                    });

                    var onEmphasis = function () {
                        circle.setStyle({
                            text: hoverLabelDto.get('show') ? labelText : '',
                            textFill: hoverTextStyleDto.getTextColor(),
                            textFont: hoverTextStyleDto.getFont()
                        });
                    };

                    var onNormal = function () {
                        circle.setStyle({
                            text: labelDto.get('show') ? labelText : '',
                            textFill: textStyleDto.getTextColor(),
                            textFont: textStyleDto.getFont()
                        });
                    };

                    polygonGroups.on('mouseover', onEmphasis)
                        .on('mouseout', onNormal)
                        .on('emphasis', onEmphasis)
                        .on('normal', onNormal);

                    onNormal();
                }

                group.add(circle);
            });
        }
    });
});