define(function (require) {

    var simpleLayoutHelper = require('./simpleLayoutHelper');
    var simpleLayoutEdge = require('./simpleLayoutEdge');
    return function (ecDto, api) {
        ecDto.eachSeriesByType('graph', function (seriesDto) {
            var layout = seriesDto.get('layout');
            var coordSys = seriesDto.coordinateSystem;
            if (coordSys && coordSys.type !== 'view') {
                var data = seriesDto.getData();
                data.each(coordSys.dimensions, function (x, y, idx) {
                    if (!isNaN(x) && !isNaN(y)) {
                        data.setItemLayout(idx, coordSys.dataToPoint([x, y]));
                    }
                    else {
                        // Also {Array.<number>}, not undefined to avoid if...else... statement
                        data.setItemLayout(idx, [NaN, NaN]);
                    }
                });

                simpleLayoutEdge(data.graph);
            }
            else if (!layout || layout === 'none') {
                simpleLayoutHelper(seriesDto);
            }
        });
    };
});