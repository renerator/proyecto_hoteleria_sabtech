define(function (require) {

    return function (ecDto, api) {
        ecDto.eachSeriesByType('radar', function (seriesDto) {
            var data = seriesDto.getData();
            var points = [];
            var coordSys = seriesDto.coordinateSystem;
            if (!coordSys) {
                return;
            }

            function pointsConverter(val, idx) {
                points[idx] = points[idx] || [];
                points[idx][i] = coordSys.dataToPoint(val, i);
            }
            for (var i = 0; i < coordSys.getIndicatorAxes().length; i++) {
                var dim = data.dimensions[i];
                data.each(dim, pointsConverter);
            }

            data.each(function (idx) {
                // Close polygon
                points[idx][0] && points[idx].push(points[idx][0].slice());
                data.setItemLayout(idx, points[idx]);
            });
        });
    };
});