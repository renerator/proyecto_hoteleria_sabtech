define(function(require) {

    'use strict';

    var zrUtil = require('zrender/core/util');
    var ComponentDto = require('../../Dto/Component');
    var axisDtoCreator = require('../axisDtoCreator');

    var PolarAxisDto = ComponentDto.extend({
        type: 'polarAxis',
        /**
         * @type {module:echarts/coord/polar/AngleAxis|module:echarts/coord/polar/RadiusAxis}
         */
        axis: null
    });

    zrUtil.merge(PolarAxisDto.prototype, require('../axisDtoCommonMixin'));

    var polarAxisDefaultExtendedOption = {
        angle: {
            polarIndex: 0,

            startAngle: 90,

            clockwise: true,

            splitNumber: 12,

            axisLabel: {
                rotate: false
            }
        },
        radius: {
            polarIndex: 0,

            splitNumber: 5
        }
    };

    function getAxisType(axisDim, option) {
        // Default axis with data is category axis
        return option.type || (option.data ? 'category' : 'value');
    }

    axisDtoCreator('angle', PolarAxisDto, getAxisType, polarAxisDefaultExtendedOption.angle);
    axisDtoCreator('radius', PolarAxisDto, getAxisType, polarAxisDefaultExtendedOption.radius);

});