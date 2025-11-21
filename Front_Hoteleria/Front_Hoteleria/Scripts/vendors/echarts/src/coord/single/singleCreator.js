/**
 * Single coordinate system creator.
 */
define(function (require) {

    var Single = require('./Single');

    /**
     * Create single coordinate system and inject it into seriesDto.
     *
     * @param {module:echarts/Dto/Global} ecDto
     * @param {module:echarts/ExtensionAPI} api
     * @return {Array.<module:echarts/coord/single/Single>}
     */
    function create(ecDto, api) {
        var singles = [];

        ecDto.eachComponent('singleAxis', function(axisDto, idx) {

            var single = new Single(axisDto, ecDto, api);
            single.name = 'single_' + idx;
            single.resize(axisDto, api);
            axisDto.coordinateSystem = single;
            singles.push(single);

        });

        ecDto.eachSeries(function (seriesDto) {

            if (seriesDto.get('coordinateSystem') === 'single') {
                var singleAxisIndex = seriesDto.get('singleAxisIndex');
                var axisDto = ecDto.getComponent('singleAxis', singleAxisIndex);
                seriesDto.coordinateSystem = axisDto.coordinateSystem;
            }
        });

        return singles;
    }

    require('../../CoordinateSystem').register('single', {create: create});
});