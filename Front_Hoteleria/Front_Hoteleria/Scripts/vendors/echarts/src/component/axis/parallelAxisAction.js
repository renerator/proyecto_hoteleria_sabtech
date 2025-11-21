define(function (require) {

    var echarts = require('../../echarts');

    var actionInfo = {
        type: 'axisAreaSelect',
        event: 'axisAreaSelected',
        update: 'updateVisual'
    };

    /**
     * @payload
     * @property {string} parallelAxisId
     * @property {Array.<Array.<number>>} intervals
     */
    echarts.registerAction(actionInfo, function (payload, ecDto) {
        ecDto.eachComponent(
            {mainType: 'parallelAxis', query: payload},
            function (parallelAxisDto) {
                parallelAxisDto.axis.Dto.setActiveIntervals(payload.intervals);
            }
        );

    });
});