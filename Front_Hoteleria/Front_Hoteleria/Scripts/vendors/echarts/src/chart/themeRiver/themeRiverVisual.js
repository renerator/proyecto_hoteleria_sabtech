define(function (require) {

    return function (ecDto) {
        ecDto.eachSeriesByType('themeRiver', function (seriesDto) {
            var data = seriesDto.getData();
            var rawData = seriesDto.getRawData();
            var colorList = seriesDto.get('color');

            data.each(function (index) {
                var name = data.getName(index);
                var rawIndex = data.getRawIndex(index);
                // use rawData just for drawing legend
                rawData.setItemVisual(
                    rawIndex,
                    'color',
                    colorList[(seriesDto.nameMap[name] - 1) % colorList.length]
                );
            });
        });
   };
});