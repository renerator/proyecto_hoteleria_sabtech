define(function (require) {

    /**
     * @payload
     * @property {string} parallelAxisId
     * @property {Array.<number>} extent
     */
    return function (ecDto, payload) {

        ecDto.eachSeriesByType('parallel', function (seriesDto) {

            var itemStyleDto = seriesDto.getDto('itemStyle.normal');
            var globalColors = ecDto.get('color');

            var color = itemStyleDto.get('color')
                || globalColors[seriesDto.seriesIndex % globalColors.length];
            var inactiveOpacity = seriesDto.get('inactiveOpacity');
            var activeOpacity = seriesDto.get('activeOpacity');
            var lineStyle = seriesDto.getDto('lineStyle.normal').getLineStyle();

            var coordSys = seriesDto.coordinateSystem;
            var data = seriesDto.getData();

            var opacityMap = {
                normal: lineStyle.opacity,
                active: activeOpacity,
                inactive: inactiveOpacity
            };

            coordSys.eachActiveState(data, function (activeState, dataIndex) {
                data.setItemVisual(dataIndex, 'opacity', opacityMap[activeState]);
            });

            data.setVisual('color', color);
        });
    };
});