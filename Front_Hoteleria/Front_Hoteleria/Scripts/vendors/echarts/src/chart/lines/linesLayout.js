define(function (require) {

    return function (ecDto) {
        ecDto.eachSeriesByType('lines', function (seriesDto) {
            var coordSys = seriesDto.coordinateSystem;
            var fromData = seriesDto.fromData;
            var toData = seriesDto.toData;
            var lineData = seriesDto.getData();

            var dims = coordSys.dimensions;
            fromData.each(dims, function (x, y, idx) {
                fromData.setItemLayout(idx, coordSys.dataToPoint([x, y]));
            });
            toData.each(dims, function (x, y, idx) {
                toData.setItemLayout(idx, coordSys.dataToPoint([x, y]));
            });
            lineData.each(function (idx) {
                var p1 = fromData.getItemLayout(idx);
                var p2 = toData.getItemLayout(idx);
                var curveness = lineData.getItemDto(idx).get('lineStyle.normal.curveness');
                var cp1;
                if (curveness > 0) {
                    cp1 = [
                        (p1[0] + p2[0]) / 2 - (p1[1] - p2[1]) * curveness,
                        (p1[1] + p2[1]) / 2 - (p2[0] - p1[0]) * curveness
                    ];
                }
                lineData.setItemLayout(idx, [p1, p2, cp1]);
            });
        });
    };
});