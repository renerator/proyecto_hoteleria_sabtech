/**
 * @file Data range visual coding.
 */
define(function (require) {

    var echarts = require('../../echarts');
    var VisualMapping = require('../../visual/VisualMapping');
    var zrUtil = require('zrender/core/util');

    echarts.registerVisualCoding('component', function (ecDto) {
        ecDto.eachComponent('visualMap', function (visualMapDto) {
            processSingleVisualMap(visualMapDto, ecDto);
        });
    });

    function processSingleVisualMap(visualMapDto, ecDto) {
        var visualMappings = visualMapDto.targetVisuals;
        var visualTypesMap = {};
        zrUtil.each(['inRange', 'outOfRange'], function (state) {
            var visualTypes = VisualMapping.prepareVisualTypes(visualMappings[state]);
            visualTypesMap[state] = visualTypes;
        });

        visualMapDto.eachTargetSeries(function (seriesDto) {
            var data = seriesDto.getData();
            var dimension = visualMapDto.getDataDimension(data);
            var dataIndex;

            function getVisual(key) {
                return data.getItemVisual(dataIndex, key);
            }

            function setVisual(key, value) {
                data.setItemVisual(dataIndex, key, value);
            }

            data.each([dimension], function (value, index) {
                // For performance consideration, do not use curry.
                dataIndex = index;
                var valueState = visualMapDto.getValueState(value);
                var mappings = visualMappings[valueState];
                var visualTypes = visualTypesMap[valueState];
                for (var i = 0, len = visualTypes.length; i < len; i++) {
                    var type = visualTypes[i];
                    mappings[type] && mappings[type].applyVisual(value, getVisual, setVisual);
                }
            }, true);
        });
    }

});
