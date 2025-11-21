define(function (require) {
    var echarts = require('../echarts');
    var zrUtil = require('zrender/core/util');
    return function (seriesType, actionInfos) {
        zrUtil.each(actionInfos, function (actionInfo) {
            actionInfo.update = 'updateView';
            /**
             * @payload
             * @property {string} seriesName
             * @property {string} name
             */
            echarts.registerAction(actionInfo, function (payload, ecDto) {
                var selected = {};
                ecDto.eachComponent(
                    {mainType: 'series', subType: seriesType, query: payload},
                    function (seriesDto) {
                        if (seriesDto[actionInfo.method]) {
                            seriesDto[actionInfo.method](payload.name);
                        }
                        var data = seriesDto.getData();
                        // Create selected map
                        data.each(function (idx) {
                            var name = data.getName(idx);
                            selected[name] = seriesDto.isSelected(name) || false;
                        });
                    }
                );
                return {
                    name: payload.name,
                    selected: selected
                };
            });
        });
    };
});