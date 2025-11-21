// Pick color from palette for each data item
define(function (require) {

    return function (seriesType, ecDto) {
        var globalColorList = ecDto.get('color');
        var offset = 0;
        ecDto.eachRawSeriesByType(seriesType, function (seriesDto) {
            var colorList = seriesDto.get('color', true);
            var dataAll = seriesDto.getRawData();
            if (!ecDto.isSeriesFiltered(seriesDto)) {
                var data = seriesDto.getData();
                data.each(function (idx) {
                    var itemDto = data.getItemDto(idx);
                    var rawIdx = data.getRawIndex(idx);
                    // If series.itemStyle.normal.color is a function. itemVisual may be encoded
                    var singleDataColor = data.getItemVisual(idx, 'color', true);
                    if (!singleDataColor) {
                        var paletteColor = colorList ? colorList[rawIdx % colorList.length]
                            : globalColorList[(rawIdx + offset) % globalColorList.length];
                        var color = itemDto.get('itemStyle.normal.color') || paletteColor;
                        // Legend may use the visual info in data before processed
                        dataAll.setItemVisual(rawIdx, 'color', color);
                        data.setItemVisual(idx, 'color', color);
                    }
                    else {
                        // Set data all color for legend
                        dataAll.setItemVisual(rawIdx, 'color', singleDataColor);
                    }
                });
            }
            offset += dataAll.count();
        });
    };
});