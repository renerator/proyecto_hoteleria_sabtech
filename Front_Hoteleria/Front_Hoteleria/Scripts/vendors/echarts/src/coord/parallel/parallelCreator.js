/**
 * Parallel coordinate system creater.
 */
define(function(require) {

    var Parallel = require('./Parallel');

    function create(ecDto, api) {
        var coordSysList = [];

        ecDto.eachComponent('parallel', function (parallelDto, idx) {
            var coordSys = new Parallel(parallelDto, ecDto, api);

            coordSys.name = 'parallel_' + idx;
            coordSys.resize(parallelDto, api);

            parallelDto.coordinateSystem = coordSys;
            coordSys.Dto = parallelDto;

            coordSysList.push(coordSys);
        });

        // Inject the coordinateSystems into seriesDto
        ecDto.eachSeries(function (seriesDto) {
            if (seriesDto.get('coordinateSystem') === 'parallel') {
                var parallelIndex = seriesDto.get('parallelIndex');
                seriesDto.coordinateSystem = coordSysList[parallelIndex];
            }
        });

        return coordSysList;
    }

    require('../../CoordinateSystem').register('parallel', {create: create});

});