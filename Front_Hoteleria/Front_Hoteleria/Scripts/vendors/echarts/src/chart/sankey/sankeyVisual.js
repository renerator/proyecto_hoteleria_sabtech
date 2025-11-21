define(function (require) {

    var VisualMapping = require('../../visual/VisualMapping');

    return function (ecDto, payload) {
        ecDto.eachSeriesByType('sankey', function (seriesDto) {
            var graph = seriesDto.getGraph();
            var nodes = graph.nodes;

            nodes.sort(function (a, b) {
                return a.getLayout().value - b.getLayout().value;
            });

            var minValue = nodes[0].getLayout().value;
            var maxValue = nodes[nodes.length - 1].getLayout().value;

            nodes.forEach(function (node) {
                var mapping = new VisualMapping({
                    type: 'color',
                    mappingMethod: 'linear',
                    dataExtent: [minValue, maxValue],
                    visual: seriesDto.get('color')
                });

                var mapValueToColor = mapping.mapValueToVisual(node.getLayout().value);
                node.setVisual('color', mapValueToColor);
                // If set itemStyle.normal.color
                var itemDto = node.getDto();
                var customColor = itemDto.get('itemStyle.normal.color');
                if (customColor != null) {
                    node.setVisual('color', customColor);
                }
            });

        }) ;
    };
});