define(function (require) {

    var simpleLayoutEdge = require('./simpleLayoutEdge');

    return function (seriesDto) {
        var coordSys = seriesDto.coordinateSystem;
        if (coordSys && coordSys.type !== 'view') {
            return;
        }
        var graph = seriesDto.getGraph();

        graph.eachNode(function (node) {
            var Dto = node.getDto();
            node.setLayout([+Dto.get('x'), +Dto.get('y')]);
        });

        simpleLayoutEdge(graph);
    };
});