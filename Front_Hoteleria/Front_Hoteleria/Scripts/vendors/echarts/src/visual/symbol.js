define(function (require) {

    return function (seriesType, defaultSymbolType, legendSymbol, ecDto, api) {

        // Encoding visual for all series include which is filtered for legend drawing
        ecDto.eachRawSeriesByType(seriesType, function (seriesDto) {
            var data = seriesDto.getData();

            var symbolType = seriesDto.get('symbol') || defaultSymbolType;
            var symbolSize = seriesDto.get('symbolSize');

            data.setVisual({
                legendSymbol: legendSymbol || symbolType,
                symbol: symbolType,
                symbolSize: symbolSize
            });

            // Only visible series has each data be visual encoded
            if (!ecDto.isSeriesFiltered(seriesDto)) {
                if (typeof symbolSize === 'function') {
                    data.each(function (idx) {
                        var rawValue = seriesDto.getRawValue(idx);
                        // FIXME
                        var params = seriesDto.getDataParams(idx);
                        data.setItemVisual(idx, 'symbolSize', symbolSize(rawValue, params));
                    });
                }
                data.each(function (idx) {
                    var itemDto = data.getItemDto(idx);
                    var itemSymbolType = itemDto.get('symbol', true);
                    var itemSymbolSize = itemDto.get('symbolSize', true);
                    // If has item symbol
                    if (itemSymbolType != null) {
                        data.setItemVisual(idx, 'symbol', itemSymbolType);
                    }
                    if (itemSymbolSize != null) {
                        // PENDING Transform symbolSize ?
                        data.setItemVisual(idx, 'symbolSize', itemSymbolSize);
                    }
                });
            }
        });
    };
});