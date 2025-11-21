define(function (require) {

    require('../coord/geo/GeoDto');

    require('../coord/geo/geoCreator');

    require('./geo/GeoView');

    require('../action/geoRoam');

    var echarts = require('../echarts');
    var zrUtil = require('zrender/core/util');

    function makeAction(method, actionInfo) {
        actionInfo.update = 'updateView';
        echarts.registerAction(actionInfo, function (payload, ecDto) {
            var selected = {};

            ecDto.eachComponent(
                { mainType: 'geo', query: payload},
                function (geoDto) {
                    geoDto[method](payload.name);
                    var geo = geoDto.coordinateSystem;
                    zrUtil.each(geo.regions, function (region) {
                        selected[region.name] = geoDto.isSelected(region.name) || false;
                    });
                }
            );

            return {
                selected: selected,
                name: payload.name
            }
        });
    }

    makeAction('toggleSelected', {
        type: 'geoToggleSelect',
        event: 'geoselectchanged'
    });
    makeAction('select', {
        type: 'geoSelect',
        event: 'geoselected'
    });
    makeAction('unSelect', {
        type: 'geoUnSelect',
        event: 'geounselected'
    });
});