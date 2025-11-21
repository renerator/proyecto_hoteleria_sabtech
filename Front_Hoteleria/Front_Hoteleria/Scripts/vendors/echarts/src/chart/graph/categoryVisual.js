define(function (require) {

    return function (ecDto) {
        ecDto.eachSeriesByType('graph', function (seriesDto) {
            var colorList = seriesDto.get('color');
            var categoriesData = seriesDto.getCategoriesData();
            var data = seriesDto.getData();

            var categoryNameIdxMap = {};

            categoriesData.each(function (idx) {
                categoryNameIdxMap[categoriesData.getName(idx)] = idx;

                var itemDto = categoriesData.getItemDto(idx);
                var rawIdx = categoriesData.getRawIndex(idx);
                var color = itemDto.get('itemStyle.normal.color')
                    || colorList[rawIdx % colorList.length];
                categoriesData.setItemVisual(idx, 'color', color);
            });

            // Assign category color to visual
            if (categoriesData.count()) {
                data.each(function (idx) {
                    var Dto = data.getItemDto(idx);
                    var category = Dto.getShallow('category');
                    if (category != null) {
                        if (typeof category === 'string') {
                            category = categoryNameIdxMap[category];
                        }
                        if (!data.getItemVisual(idx, 'color', true)) {
                            data.setItemVisual(
                                idx, 'color',
                                categoriesData.getItemVisual(category, 'color')
                            );
                        }
                    }
                });
            }
        });
    };
});