define(function(require) {
    'use strict';

    var graphic = require('../util/graphic');
    var zrUtil = require('zrender/core/util');

    require('../coord/cartesian/Grid');

    require('./axis');

    // Grid view
    require('../echarts').extendComponentView({

        type: 'grid',

        render: function (gridDto, ecDto) {
            this.group.removeAll();
            if (gridDto.get('show')) {
                this.group.add(new graphic.Rect({
                    shape:gridDto.coordinateSystem.getRect(),
                    style: zrUtil.defaults({
                        fill: gridDto.get('backgroundColor')
                    }, gridDto.getItemStyle()),
                    silent: true
                }));
            }
        }
    });
});