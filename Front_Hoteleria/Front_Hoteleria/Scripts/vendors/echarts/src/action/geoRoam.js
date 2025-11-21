define(function (require) {

    var zrUtil = require('zrender/core/util');
    var roamHelper = require('./roamHelper');

    var echarts = require('../echarts');

    /**
     * @payload
     * @property {string} [componentType=series]
     * @property {number} [dx]
     * @property {number} [dy]
     * @property {number} [zoom]
     * @property {number} [originX]
     * @property {number} [originY]
     */
    echarts.registerAction({
        type: 'geoRoam',
        event: 'geoRoam',
        update: 'updateLayout'
    }, function (payload, ecDto) {
        var componentType = payload.componentType || 'series';

        ecDto.eachComponent(
            { mainType: componentType, query: payload },
            function (componentDto) {
                var geo = componentDto.coordinateSystem;
                if (geo.type !== 'geo') {
                    return;
                }

                var res = roamHelper.updateCenterAndZoom(
                    geo, payload, componentDto.get('scaleLimit')
                );

                componentDto.setCenter
                    && componentDto.setCenter(res.center);

                componentDto.setZoom
                    && componentDto.setZoom(res.zoom);

                // All map series with same `map` use the same geo coordinate system
                // So the center and zoom must be in sync. Include the series not selected by legend
                if (componentType === 'series') {
                    zrUtil.each(componentDto.seriesGroup, function (seriesDto) {
                        seriesDto.setCenter(res.center);
                        seriesDto.setZoom(res.zoom);
                    });
                }
            }
        );
    });
});