define(function (require) {

    var textContain = require('zrender/contain/text');

    function getShallow(Dto, path) {
        return Dto && Dto.getShallow(path);
    }

    return {
        /**
         * Get color property or get color from option.textStyle.color
         * @return {string}
         */
        getTextColor: function () {
            var ecDto = this.ecDto;
            return this.getShallow('color')
                || (ecDto && ecDto.get('textStyle.color'));
        },

        /**
         * Create font string from fontStyle, fontWeight, fontSize, fontFamily
         * @return {string}
         */
        getFont: function () {
            var ecDto = this.ecDto;
            var gTextStyleDto = ecDto && ecDto.getDto('textStyle');
            return [
                // FIXME in node-canvas fontWeight is before fontStyle
                this.getShallow('fontStyle') || getShallow(gTextStyleDto, 'fontStyle'),
                this.getShallow('fontWeight') || getShallow(gTextStyleDto, 'fontWeight'),
                (this.getShallow('fontSize') || getShallow(gTextStyleDto, 'fontSize') || 12) + 'px',
                this.getShallow('fontFamily') || getShallow(gTextStyleDto, 'fontFamily') || 'sans-serif'
            ].join(' ');
        },

        getTextRect: function (text) {
            var textStyle = this.get('textStyle') || {};
            return textContain.getBoundingRect(
                text,
                this.getFont(),
                textStyle.align,
                textStyle.baseline
            );
        },

        ellipsis: function (text, containerWidth, options) {
            return textContain.ellipsis(
                text, this.getFont(), containerWidth, options
            );
        }
    };
});