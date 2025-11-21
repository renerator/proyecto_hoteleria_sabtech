/**
 * @file Timeilne action
 */
define(function(require) {

    var echarts = require('../../echarts');

    echarts.registerAction(

        {type: 'timelineChange', event: 'timelineChanged', update: 'prepareAndUpdate'},

        function (payload, ecDto) {

            var timelineDto = ecDto.getComponent('timeline');
            if (timelineDto && payload.currentIndex != null) {
                timelineDto.setCurrentIndex(payload.currentIndex);

                if (!timelineDto.get('loop', true) && timelineDto.isIndexMax()) {
                    timelineDto.setPlayState(false);
                }
            }

            ecDto.resetOption('timeline');
        }
    );

    echarts.registerAction(

        {type: 'timelinePlayChange', event: 'timelinePlayChanged', update: 'update'},

        function (payload, ecDto) {
            var timelineDto = ecDto.getComponent('timeline');
            if (timelineDto && payload.playState != null) {
                timelineDto.setPlayState(payload.playState);
            }
        }
    );

});