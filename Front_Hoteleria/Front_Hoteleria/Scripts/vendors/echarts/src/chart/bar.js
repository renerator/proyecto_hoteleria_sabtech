define(function (require) {

    var zrUtil = require('zrender/core/util');

    require('../coord/cartesian/Grid');

    require('./bar/BarSeries');
    require('./bar/BarView');

    var barLayoutGrid = require('../layout/barGrid');
    var echarts = require('../echarts');

    echarts.registerLayout(zrUtil.curry(barLayoutGrid, 'bar'));
    // Visual coding for legend
    echarts.registerVisualCoding('chart', function (ecDto) {
        ecDto.eachSeriesByType('bar', function (seriesDto) {
            var data = seriesDto.getData();
            data.setVisual('legendSymbol', 'roundRect');
        });
    });

    // In case developer forget to include grid component
    require('../component/grid');
});