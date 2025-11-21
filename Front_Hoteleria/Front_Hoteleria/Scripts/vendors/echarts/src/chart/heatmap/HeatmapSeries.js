define(function (require) {

    var SeriesDto = require('../../Dto/Series');
    var createListFromArray = require('../helper/createListFromArray');

    return SeriesDto.extend({
        type: 'series.heatmap',

        getInitialData: function (option, ecDto) {
            return createListFromArray(option.data, this, ecDto);
        },

        defaultOption: {

            // Cartesian2D or geo
            coordinateSystem: 'cartesian2d',

            zlevel: 0,

            z: 2,

            // Cartesian coordinate system
            xAxisIndex: 0,
            yAxisIndex: 0,

            // Geo coordinate system
            geoIndex: 0,

            blurSize: 30,

            pointSize: 20,

            maxOpacity: 1,

            minOpacity: 0
        }
    });
});