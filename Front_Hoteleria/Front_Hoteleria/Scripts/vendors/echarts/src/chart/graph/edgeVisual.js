define(function (require) {

    function normalize(a) {
        if (!(a instanceof Array)) {
            a = [a, a];
        }
        return a;
    }
    return function (ecDto) {
        ecDto.eachSeriesByType('graph', function (seriesDto) {
            var edgeData = seriesDto.getEdgeData();
            var symbolType = normalize(seriesDto.get('edgeSymbol'));
            var symbolSize = normalize(seriesDto.get('edgeSymbolSize'));

            edgeData.setVisual('fromSymbol', symbolType && symbolType[0]);
            edgeData.setVisual('toSymbol', symbolType && symbolType[1]);
            edgeData.setVisual('fromSymbolSize', symbolSize && symbolSize[0]);
            edgeData.setVisual('toSymbolSize', symbolSize && symbolSize[1]);
            edgeData.setVisual('color', seriesDto.get('lineStyle.normal.color'));

            edgeData.each(function (idx) {
                var itemDto = edgeData.getItemDto(idx);
                var symbolType = normalize(itemDto.getShallow('symbol', true));
                var symbolSize = normalize(itemDto.getShallow('symbolSize', true));

                symbolType[0] && edgeData.setItemVisual(idx, 'fromSymbol', symbolType[0]);
                symbolType[1] && edgeData.setItemVisual(idx, 'toSymbol', symbolType[1]);
                symbolSize[0] && edgeData.setItemVisual(idx, 'fromSymbolSize', symbolSize[0]);
                symbolSize[1] && edgeData.setItemVisual(idx, 'toSymbolSize', symbolSize[1]);
            });
        });
    };
});