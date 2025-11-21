define(function (require) {

    var graphic = require('../../util/graphic');
    var zrUtil = require('zrender/core/util');

    /**
     * Piece of pie including Sector, Label, LabelLine
     * @constructor
     * @extends {module:zrender/graphic/Group}
     */
    function FunnelPiece(data, idx) {

        graphic.Group.call(this);

        var polygon = new graphic.Polygon();
        var labelLine = new graphic.Polyline();
        var text = new graphic.Text();
        this.add(polygon);
        this.add(labelLine);
        this.add(text);

        this.updateData(data, idx, true);

        // Hover to change label and labelLine
        function onEmphasis() {
            labelLine.ignore = labelLine.hoverIgnore;
            text.ignore = text.hoverIgnore;
        }
        function onNormal() {
            labelLine.ignore = labelLine.normalIgnore;
            text.ignore = text.normalIgnore;
        }
        this.on('emphasis', onEmphasis)
            .on('normal', onNormal)
            .on('mouseover', onEmphasis)
            .on('mouseout', onNormal);
    }

    var funnelPieceProto = FunnelPiece.prototype;

    function getLabelStyle(data, idx, state, labelDto) {
        var textStyleDto = labelDto.getDto('textStyle');
        var position = labelDto.get('position');
        var isLabelInside = position === 'inside' || position === 'inner' || position === 'center';
        return {
            fill: textStyleDto.getTextColor()
                || (isLabelInside ? '#fff' : data.getItemVisual(idx, 'color')),
            textFont: textStyleDto.getFont(),
            text: zrUtil.retrieve(
                data.hostDto.getFormattedLabel(idx, state),
                data.getName(idx)
            )
        };
    }

    var opacityAccessPath = ['itemStyle', 'normal', 'opacity'];
    funnelPieceProto.updateData = function (data, idx, firstCreate) {

        var polygon = this.childAt(0);

        var seriesDto = data.hostDto;
        var itemDto = data.getItemDto(idx);
        var layout = data.getItemLayout(idx);
        var opacity = data.getItemDto(idx).get(opacityAccessPath);
        opacity = opacity == null ? 1 : opacity;

        // Reset style
        polygon.useStyle({});

        if (firstCreate) {
            polygon.setShape({
                points: layout.points
            });
            polygon.setStyle({ opacity : 0 });
            graphic.initProps(polygon, {
                style: {
                    opacity: opacity
                }
            }, seriesDto, idx);
        }
        else {
            graphic.updateProps(polygon, {
                style: {
                    opacity: opacity
                },
                shape: {
                    points: layout.points
                }
            }, seriesDto, idx);
        }

        // Update common style
        var itemStyleDto = itemDto.getDto('itemStyle');
        var visualColor = data.getItemVisual(idx, 'color');

        polygon.setStyle(
            zrUtil.defaults(
                {
                    fill: visualColor
                },
                itemStyleDto.getDto('normal').getItemStyle(['opacity'])
            )
        );
        polygon.hoverStyle = itemStyleDto.getDto('emphasis').getItemStyle();

        this._updateLabel(data, idx);

        graphic.setHoverStyle(this);
    };

    funnelPieceProto._updateLabel = function (data, idx) {

        var labelLine = this.childAt(1);
        var labelText = this.childAt(2);

        var seriesDto = data.hostDto;
        var itemDto = data.getItemDto(idx);
        var layout = data.getItemLayout(idx);
        var labelLayout = layout.label;
        var visualColor = data.getItemVisual(idx, 'color');

        graphic.updateProps(labelLine, {
            shape: {
                points: labelLayout.linePoints || labelLayout.linePoints
            }
        }, seriesDto, idx);

        graphic.updateProps(labelText, {
            style: {
                x: labelLayout.x,
                y: labelLayout.y
            }
        }, seriesDto, idx);
        labelText.attr({
            style: {
                textAlign: labelLayout.textAlign,
                textVerticalAlign: labelLayout.verticalAlign,
                textFont: labelLayout.font
            },
            rotation: labelLayout.rotation,
            origin: [labelLayout.x, labelLayout.y],
            z2: 10
        });

        var labelDto = itemDto.getDto('label.normal');
        var labelHoverDto = itemDto.getDto('label.emphasis');
        var labelLineDto = itemDto.getDto('labelLine.normal');
        var labelLineHoverDto = itemDto.getDto('labelLine.emphasis');

        labelText.setStyle(getLabelStyle(data, idx, 'normal', labelDto));

        labelText.ignore = labelText.normalIgnore = !labelDto.get('show');
        labelText.hoverIgnore = !labelHoverDto.get('show');

        labelLine.ignore = labelLine.normalIgnore = !labelLineDto.get('show');
        labelLine.hoverIgnore = !labelLineHoverDto.get('show');

        // Default use item visual color
        labelLine.setStyle({
            stroke: visualColor
        });
        labelLine.setStyle(labelLineDto.getDto('lineStyle').getLineStyle());

        labelText.hoverStyle = getLabelStyle(data, idx, 'emphasis', labelHoverDto);
        labelLine.hoverStyle = labelLineHoverDto.getDto('lineStyle').getLineStyle();
    };

    zrUtil.inherits(FunnelPiece, graphic.Group);


    var Funnel = require('../../view/Chart').extend({

        type: 'funnel',

        render: function (seriesDto, ecDto, api) {
            var data = seriesDto.getData();
            var oldData = this._data;

            var group = this.group;

            data.diff(oldData)
                .add(function (idx) {
                    var funnelPiece = new FunnelPiece(data, idx);

                    data.setItemGraphicEl(idx, funnelPiece);

                    group.add(funnelPiece);
                })
                .update(function (newIdx, oldIdx) {
                    var piePiece = oldData.getItemGraphicEl(oldIdx);

                    piePiece.updateData(data, newIdx);

                    group.add(piePiece);
                    data.setItemGraphicEl(newIdx, piePiece);
                })
                .remove(function (idx) {
                    var piePiece = oldData.getItemGraphicEl(idx);
                    group.remove(piePiece);
                })
                .execute();

            this._data = data;
        },

        remove: function () {
            this.group.removeAll();
            this._data = null;
        }
    });

    return Funnel;
});