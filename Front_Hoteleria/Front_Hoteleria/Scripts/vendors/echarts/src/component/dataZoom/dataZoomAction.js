/**
 * @file Data zoom action
 */
define(function(require) {

    var zrUtil = require('zrender/core/util');
    var DtoUtil = require('../../util/Dto');
    var echarts = require('../../echarts');


    echarts.registerAction('dataZoom', function (payload, ecDto) {

        var linkedNodesFinder = DtoUtil.createLinkedNodesFinder(
            zrUtil.bind(ecDto.eachComponent, ecDto, 'dataZoom'),
            DtoUtil.eachAxisDim,
            function (Dto, dimNames) {
                return Dto.get(dimNames.axisIndex);
            }
        );

        var effectedDtos = [];

        ecDto.eachComponent(
            {mainType: 'dataZoom', query: payload},
            function (Dto, index) {
                effectedDtos.push.apply(
                    effectedDtos, linkedNodesFinder(Dto).nodes
                );
            }
        );

        zrUtil.each(effectedDtos, function (dataZoomDto, index) {
            dataZoomDto.setRawRange({
                start: payload.start,
                end: payload.end,
                startValue: payload.startValue,
                endValue: payload.endValue
            });
        });

    });

});