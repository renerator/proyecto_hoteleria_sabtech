define(function (require) {

    var echarts = require('../../echarts');
    var roamHelper = require('../../action/roamHelper');

    var actionInfo = {
        type: 'graphRoam',
        event: 'graphRoam',
        update: 'none'
    };

    /**
     * @payload
     * @property {string} name Series name
     * @property {number} [dx]
     * @property {number} [dy]
     * @property {number} [zoom]
     * @property {number} [originX]
     * @property {number} [originY]
     */

    echarts.registerAction(actionInfo, function (payload, ecDto) {
        ecDto.eachComponent({mainType: 'series', query: payload}, function (seriesDto) {
            var coordSys = seriesDto.coordinateSystem;

            var res = roamHelper.updateCenterAndZoom(coordSys, payload);

            seriesDto.setCenter
                && seriesDto.setCenter(res.center);

            seriesDto.setZoom
                && seriesDto.setZoom(res.zoom);
        });
    });
});