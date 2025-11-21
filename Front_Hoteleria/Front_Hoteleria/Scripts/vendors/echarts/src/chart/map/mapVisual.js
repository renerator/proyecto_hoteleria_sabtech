define(function (require) {
    return function (ecDto) {
        ecDto.eachSeriesByType('map', function (seriesDto) {
            var colorList = seriesDto.get('color');
            var itemStyleDto = seriesDto.getDto('itemStyle.normal');

            var areaColor = itemStyleDto.get('areaColor');
            var color = itemStyleDto.get('color')
                || colorList[seriesDto.seriesIndex % colorList.length];

            seriesDto.getData().setVisual({
                'areaColor': areaColor,
                'color': color
            });
        });
    };
});