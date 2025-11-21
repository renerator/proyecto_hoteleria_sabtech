define(function () {
    return function (seriesType, ecDto) {
        var legendDtos = ecDto.findComponents({
            mainType: 'legend'
        });
        if (!legendDtos || !legendDtos.length) {
            return;
        }
        ecDto.eachSeriesByType(seriesType, function (series) {
            var data = series.getData();
            data.filterSelf(function (idx) {
                var name = data.getName(idx);
                // If in any legend component the status is not selected.
                for (var i = 0; i < legendDtos.length; i++) {
                    if (!legendDtos[i].isSelected(name)) {
                        return false;
                    }
                }
                return true;
            }, this);
        }, this);
    };
});