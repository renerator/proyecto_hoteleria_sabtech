define(function (require) {

    return function (ecDto) {
        var legendDtos = ecDto.findComponents({
            mainType: 'legend'
        });
        if (!legendDtos || !legendDtos.length) {
            return;
        }
        ecDto.eachSeriesByType('graph', function (graphSeries) {
            var categoriesData = graphSeries.getCategoriesData();
            var graph = graphSeries.getGraph();
            var data = graph.data;

            var categoryNames = categoriesData.mapArray(categoriesData.getName);

            data.filterSelf(function (idx) {
                var Dto = data.getItemDto(idx);
                var category = Dto.getShallow('category');
                if (category != null) {
                    if (typeof category === 'number') {
                        category = categoryNames[category];
                    }
                    // If in any legend component the status is not selected.
                    for (var i = 0; i < legendDtos.length; i++) {
                        if (!legendDtos[i].isSelected(category)) {
                            return false;
                        }
                    }
                }
                return true;
            });
        }, this);
    };
});