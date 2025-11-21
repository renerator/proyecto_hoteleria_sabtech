define(function (require) {

    var ComponentDto = require('../../Dto/Component');
    var axisDtoCreator = require('../axisDtoCreator');
    var zrUtil =  require('zrender/core/util');

    var AxisDto = ComponentDto.extend({

        type: 'singleAxis',

        layoutMode: 'box',

        /**
         * @type {module:echarts/coord/single/SingleAxis}
         */
        axis: null,

        /**
         * @type {module:echarts/coord/single/Single}
         */
        coordinateSystem: null

    });

    var defaultOption = {

        left: '5%',
        top: '5%',
        right: '5%',
        bottom: '5%',

        type: 'value',

        position: 'bottom',

        orient: 'horizontal',
        // singleIndex: 0,

        axisLine: {
            show: true,
            lineStyle: {
                width: 2,
                type: 'solid'
            }
        },

        axisTick: {
            show: true,
            length: 6,
            lineStyle: {
                width: 2
            }
        },

        axisLabel: {
            show: true,
            interval: 'auto'
        },

        splitLine: {
            show: true,
            lineStyle: {
                type: 'dashed',
                opacity: 0.2
            }
        }
    };

    function getAxisType(axisName, option) {
        return option.type || (option.data ? 'category' : 'value');
    }

    zrUtil.merge(AxisDto.prototype, require('../axisDtoCommonMixin'));

    axisDtoCreator('single', AxisDto, getAxisType, defaultOption);

    return AxisDto;
});