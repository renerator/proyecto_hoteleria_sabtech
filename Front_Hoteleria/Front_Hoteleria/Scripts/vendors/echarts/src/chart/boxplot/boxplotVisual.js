define(function (require) {

    var borderColorQuery = ['itemStyle', 'normal', 'borderColor'];

    return function (ecDto, api) {

        var globalColors = ecDto.get('color');

        ecDto.eachRawSeriesByType('boxplot', function (seriesDto) {

            var defaulColor = globalColors[seriesDto.seriesIndex % globalColors.length];
            var data = seriesDto.getData();

            data.setVisual({
                legendSymbol: 'roundRect',
                // Use name 'color' but not 'borderColor' for legend usage and
                // visual coding from other component like dataRange.
                color: seriesDto.get(borderColorQuery) || defaulColor
            });

            // Only visible series has each data be visual encoded
            if (!ecDto.isSeriesFiltered(seriesDto)) {
                data.each(function (idx) {
                    var itemDto = data.getItemDto(idx);
                    data.setItemVisual(
                        idx,
                        {color: itemDto.get(borderColorQuery, true)}
                    );
                });
            }
        });

    };
});