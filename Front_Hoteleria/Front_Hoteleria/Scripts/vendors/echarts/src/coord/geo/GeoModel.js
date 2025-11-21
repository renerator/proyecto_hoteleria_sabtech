define(function (require) {

    'use strict';
    var DtoUtil = require('../../util/Dto');
    var ComponentDto = require('../../Dto/Component');
    var Dto = require('../../Dto/Dto');
    var zrUtil = require('zrender/core/util');

    var selectableMixin = require('../../component/helper/selectableMixin');

    var geoCreator = require('./geoCreator');

    var GeoDto = ComponentDto.extend({

        type: 'geo',

        /**
         * @type {module:echarts/coord/geo/Geo}
         */
        coordinateSystem: null,

        init: function (option) {
            ComponentDto.prototype.init.apply(this, arguments);

            // Default label emphasis `position` and `show`
            DtoUtil.defaultEmphasis(
                option.label, ['position', 'show', 'textStyle', 'distance', 'formatter']
            );
        },

        optionUpdated: function () {
            var option = this.option;
            var self = this;

            option.regions = geoCreator.getFilledRegions(option.regions, option.map);

            this._optionDtoMap = zrUtil.reduce(option.regions || [], function (obj, regionOpt) {
                if (regionOpt.name) {
                    obj[regionOpt.name] = new Dto(regionOpt, self);
                }
                return obj;
            }, {});

            this.updateSelectedMap(option.regions);
        },

        defaultOption: {

            zlevel: 0,

            z: 0,

            show: true,

            left: 'center',

            top: 'center',

            // 自适应
            // width:,
            // height:,
            // right
            // bottom

            // Map type
            map: '',

            // Default on center of map
            center: null,

            zoom: 1,

            scaleLimit: null,

            // selectedMode: false

            label: {
                normal: {
                    show: false,
                    textStyle: {
                        color: '#000'
                    }
                },
                emphasis: {
                    show: true,
                    textStyle: {
                        color: 'rgb(100,0,0)'
                    }
                }
            },

            itemStyle: {
                normal: {
                    // color: 各异,
                    borderWidth: 0.5,
                    borderColor: '#444',
                    color: '#eee'
                },
                emphasis: {                 // 也是选中样式
                    color: 'rgba(255,215,0,0.8)'
                }
            },

            regions: []
        },

        /**
         * Get Dto of region
         * @param  {string} name
         * @return {module:echarts/Dto/Dto}
         */
        getRegionDto: function (name) {
            return this._optionDtoMap[name];
        },

        /**
         * Format label
         * @param {string} name Region name
         * @param {string} [status='normal'] 'normal' or 'emphasis'
         * @return {string}
         */
        getFormattedLabel: function (name, status) {
            var formatter = this.get('label.' + status + '.formatter');
            var params = {
                name: name
            };
            if (typeof formatter === 'function') {
                params.status = status;
                return formatter(params);
            }
            else if (typeof formatter === 'string') {
                return formatter.replace('{a}', params.seriesName);
            }
        },

        setZoom: function (zoom) {
            this.option.zoom = zoom;
        },

        setCenter: function (center) {
            this.option.center = center;
        }
    });

    zrUtil.mixin(GeoDto, selectableMixin);

    return GeoDto;
});