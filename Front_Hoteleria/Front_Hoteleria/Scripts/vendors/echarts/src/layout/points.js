define(function (require) {

    return function (seriesType, ecDto, api) {
        ecDto.eachSeriesByType(seriesType, function (seriesDto) {
            var data = seriesDto.getData();
            var coordSys = seriesDto.coordinateSystem;

            if (coordSys) {
                var dims = coordSys.dimensions;
                data.each(dims, function (x, y, idx) {
                    var point;
                    if (!isNaN(x) && !isNaN(y)) {
                        point = coordSys.dataToPoint([x, y]);
                    }
                    else {
                        // Also {Array.<number>}, not undefined to avoid if...else... statement
                        point = [NaN, NaN];
                    }

                    data.setItemLayout(idx, point);
                }, true);
            }
        });
    };
});