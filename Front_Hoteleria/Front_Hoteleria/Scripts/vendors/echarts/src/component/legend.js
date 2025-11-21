/**
 * Legend component entry file8
 */
define(function (require) {

    require('./legend/LegendDto');
    require('./legend/legendAction');
    require('./legend/LegendView');

    var echarts = require('../echarts');
    // Series Filter
    echarts.registerProcessor('filter', require('./legend/legendFilter'));
});