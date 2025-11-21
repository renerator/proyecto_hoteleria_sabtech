define(function(require) {

    require('../coord/parallel/parallelCreator');
    require('../coord/parallel/ParallelDto');
    require('./parallelAxis');

    var echarts = require('../echarts');

    // Parallel view
    echarts.extendComponentView({
        type: 'parallel'
    });

    echarts.registerPreprocessor(
        require('../coord/parallel/parallelPreprocessor')
    );

});