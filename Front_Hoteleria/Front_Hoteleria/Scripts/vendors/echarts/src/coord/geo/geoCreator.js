define(function (require) {

    var Geo = require('./Geo');

    var layout = require('../../util/layout');
    var zrUtil = require('zrender/core/util');

    var mapDataStores = {};

    /**
     * Resize method bound to the geo
     * @param {module:echarts/coord/geo/GeoDto|module:echarts/chart/map/MapDto} geoDto
     * @param {module:echarts/ExtensionAPI} api
     */
    function resizeGeo (geoDto, api) {
        var rect = this.getBoundingRect();

        var boxLayoutOption = geoDto.getBoxLayoutParams();
        // 0.75 rate
        boxLayoutOption.aspect = rect.width / rect.height * 0.75;

        var viewRect = layout.getLayoutRect(boxLayoutOption, {
            width: api.getWidth(),
            height: api.getHeight()
        });

        this.setViewRect(viewRect.x, viewRect.y, viewRect.width, viewRect.height);

        this.setCenter(geoDto.get('center'));
        this.setZoom(geoDto.get('zoom'));
    }

    /**
     * @param {module:echarts/coord/Geo} geo
     * @param {module:echarts/Dto/Dto} Dto
     * @inner
     */
    function setGeoCoords(geo, Dto) {
        zrUtil.each(Dto.get('geoCoord'), function (geoCoord, name) {
            geo.addGeoCoord(name, geoCoord);
        });
    }

    function mapNotExistsError(name) {
        console.error('Map ' + name + ' not exists');
    }

    var geoCreator = {

        // For deciding which dimensions to use when creating list data
        dimensions: Geo.prototype.dimensions,

        create: function (ecDto, api) {
            var geoList = [];

            // FIXME Create each time may be slow
            ecDto.eachComponent('geo', function (geoDto, idx) {
                var name = geoDto.get('map');
                var mapData = mapDataStores[name];
                if (!mapData) {
                    mapNotExistsError(name);
                }
                var geo = new Geo(
                    name + idx, name,
                    mapData && mapData.geoJson, mapData && mapData.specialAreas,
                    geoDto.get('nameMap')
                );
                geo.zoomLimit = geoDto.get('scaleLimit');
                geoList.push(geo);

                setGeoCoords(geo, geoDto);

                geoDto.coordinateSystem = geo;
                geo.Dto = geoDto;

                // Inject resize method
                geo.resize = resizeGeo;

                geo.resize(geoDto, api);
            });

            ecDto.eachSeries(function (seriesDto) {
                var coordSys = seriesDto.get('coordinateSystem');
                if (coordSys === 'geo') {
                    var geoIndex = seriesDto.get('geoIndex') || 0;
                    seriesDto.coordinateSystem = geoList[geoIndex];
                }
            });

            // If has map series
            var mapDtoGroupBySeries = {};

            ecDto.eachSeriesByType('map', function (seriesDto) {
                var mapType = seriesDto.get('map');

                mapDtoGroupBySeries[mapType] = mapDtoGroupBySeries[mapType] || [];

                mapDtoGroupBySeries[mapType].push(seriesDto);
            });

            zrUtil.each(mapDtoGroupBySeries, function (mapSeries, mapType) {
                var mapData = mapDataStores[mapType];
                if (!mapData) {
                    mapNotExistsError(name);
                }

                var nameMapList = zrUtil.map(mapSeries, function (singleMapSeries) {
                    return singleMapSeries.get('nameMap');
                });
                var geo = new Geo(
                    mapType, mapType,
                    mapData && mapData.geoJson, mapData && mapData.specialAreas,
                    zrUtil.mergeAll(nameMapList)
                );
                geo.zoomLimit = zrUtil.retrieve.apply(null, zrUtil.map(mapSeries, function (singleMapSeries) {
                    return singleMapSeries.get('scaleLimit');
                }));
                geoList.push(geo);

                // Inject resize method
                geo.resize = resizeGeo;

                geo.resize(mapSeries[0], api);

                zrUtil.each(mapSeries, function (singleMapSeries) {
                    singleMapSeries.coordinateSystem = geo;

                    setGeoCoords(geo, singleMapSeries);
                });
            });

            return geoList;
        },

        /**
         * @param {string} mapName
         * @param {Object|string} geoJson
         * @param {Object} [specialAreas]
         *
         * @example
         *     $.get('USA.json', function (geoJson) {
         *         echarts.registerMap('USA', geoJson);
         *         // Or
         *         echarts.registerMap('USA', {
         *             geoJson: geoJson,
         *             specialAreas: {}
         *         })
         *     });
         */
        registerMap: function (mapName, geoJson, specialAreas) {
            if (geoJson.geoJson && !geoJson.features) {
                specialAreas = geoJson.specialAreas;
                geoJson = geoJson.geoJson;
            }
            if (typeof geoJson === 'string') {
                geoJson = (typeof JSON !== 'undefined' && JSON.parse)
                    ? JSON.parse(geoJson) : (new Function('return (' + geoJson + ');'))();
            }
            mapDataStores[mapName] = {
                geoJson: geoJson,
                specialAreas: specialAreas
            };
        },

        /**
         * @param {string} mapName
         * @return {Object}
         */
        getMap: function (mapName) {
            return mapDataStores[mapName];
        },

        /**
         * Fill given regions array
         * @param  {Array.<Object>} originRegionArr
         * @param  {string} mapName
         * @return {Array}
         */
        getFilledRegions: function (originRegionArr, mapName) {
            // Not use the original
            var regionsArr = (originRegionArr || []).slice();

            var map = geoCreator.getMap(mapName);
            var geoJson = map && map.geoJson;

            var dataNameMap = {};
            var features = geoJson.features;
            for (var i = 0; i < regionsArr.length; i++) {
                dataNameMap[regionsArr[i].name] = regionsArr[i];
            }

            for (var i = 0; i < features.length; i++) {
                var name = features[i].properties.name;
                if (!dataNameMap[name]) {
                    regionsArr.push({
                        name: name
                    });
                }
            }
            return regionsArr;
        }
    };

    // Inject methods into echarts
    var echarts = require('../../echarts');

    echarts.registerMap = geoCreator.registerMap;

    echarts.getMap = geoCreator.getMap;

    // TODO
    echarts.loadMap = function () {};

    echarts.registerCoordinateSystem('geo', geoCreator);

    return geoCreator;
});