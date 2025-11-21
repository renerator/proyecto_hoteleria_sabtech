define(function (require) {

    'use strict';

    require('./AxisDto');

    require('../../echarts').extendComponentDto({

        type: 'polar',

        dependencies: ['polarAxis', 'angleAxis'],

        /**
         * @type {module:echarts/coord/polar/Polar}
         */
        coordinateSystem: null,

        /**
         * @param {string} axisType
         * @return {module:echarts/coord/polar/AxisDto}
         */
        findAxisDto: function (axisType) {
            var angleAxisDto;
            var ecDto = this.ecDto;
            ecDto.eachComponent(axisType, function (axisDto) {
                if (ecDto.getComponent(
                        'polar', axisDto.getShallow('polarIndex')
                    ) === this) {
                    angleAxisDto = axisDto;
                }
            }, this);
            return angleAxisDto;
        },

        defaultOption: {

            zlevel: 0,

            z: 0,

            center: ['50%', '50%'],

            radius: '80%'
        }
    });
});