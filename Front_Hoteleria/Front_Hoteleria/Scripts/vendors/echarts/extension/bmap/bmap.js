/**
 * BMap component extension
 */
define(function (require) {

    require('echarts').registerCoordinateSystem(
        'bmap', require('./BMapCoordSys')
    );
    require('./BMapDto');
    require('./BMapView');

    // Action
    require('echarts').registerAction({
        type: 'bmapRoam',
        event: 'bmapRoam',
        update: 'updateLayout'
    }, function (payload, ecDto) {
        ecDto.eachComponent('bmap', function (bMapDto) {
            var bmap = bMapDto.getBMap();
            var center = bmap.getCenter();
            bMapDto.setCenterAndZoom([center.lng, center.lat], bmap.getZoom());
        });
    });

    return {
        version: '1.0.0'
    };
});