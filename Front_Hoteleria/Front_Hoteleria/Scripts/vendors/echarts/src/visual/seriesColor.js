define(function (require) {
    var Gradient = require('zrender/graphic/Gradient');
    return function (seriesType, styleType, ecDto) {
        function encodeColor(seriesDto) {
            var colorAccessPath = [styleType, 'normal', 'color'];
            var colorList = ecDto.get('color');
            var data = seriesDto.getData();
            var color = seriesDto.get(colorAccessPath) // Set in itemStyle
                || colorList[seriesDto.seriesIndex % colorList.length];  // Default color

            // FIXME Set color function or use the platte color
            data.setVisual('color', color);

            // Only visible series has each data be visual encoded
            if (!ecDto.isSeriesFiltered(seriesDto)) {
                if (typeof color === 'function' && !(color instanceof Gradient)) {
                    data.each(function (idx) {
                        data.setItemVisual(
                            idx, 'color', color(seriesDto.getDataParams(idx))
                        );
                    });
                }

                data.each(function (idx) {
                    var itemDto = data.getItemDto(idx);
                    var color = itemDto.get(colorAccessPath, true);
                    if (color != null) {
                        data.setItemVisual(idx, 'color', color);
                    }
                });
            }
        }
        seriesType ? ecDto.eachSeriesByType(seriesType, encodeColor)
            : ecDto.eachSeries(encodeColor);
    };
});