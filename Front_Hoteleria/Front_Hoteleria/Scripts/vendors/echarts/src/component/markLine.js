define(function (require) {

    require('./marker/MarkLineDto');
    require('./marker/MarkLineView');

    require('../echarts').registerPreprocessor(function (opt) {
        // Make sure markLine component is enabled
        opt.markLine = opt.markLine || {};
    });
});