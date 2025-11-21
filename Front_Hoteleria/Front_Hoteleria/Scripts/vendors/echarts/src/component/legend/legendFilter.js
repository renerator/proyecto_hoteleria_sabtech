define(function () {
   return function (ecDto) {
        var legendDtos = ecDto.findComponents({
            mainType: 'legend'
        });
        if (legendDtos && legendDtos.length) {
            ecDto.filterSeries(function (series) {
                // If in any legend component the status is not selected.
                // Because in legend series is assumed selected when it is not in the legend data.
                for (var i = 0; i < legendDtos.length; i++) {
                    if (!legendDtos[i].isSelected(series.name)) {
                        return false;
                    }
                }
                return true;
            });
        }
    };
});