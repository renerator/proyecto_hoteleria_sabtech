define(function (require) {

    var positiveBorderColorQuery = ['itemStyle', 'normal', 'borderColor'];
    var negativeBorderColorQuery = ['itemStyle', 'normal', 'borderColor0'];
    var positiveColorQuery = ['itemStyle', 'normal', 'color'];
    var negativeColorQuery = ['itemStyle', 'normal', 'color0'];

    return function (ecDto, api) {

        ecDto.eachRawSeriesByType('candlestick', function (seriesDto) {

            var data = seriesDto.getData();

            data.setVisual({
                legendSymbol: 'roundRect'
            });

            // Only visible series has each data be visual encoded
            if (!ecDto.isSeriesFiltered(seriesDto)) {
                data.each(function (idx) {
                    var itemDto = data.getItemDto(idx);
                    var sign = data.getItemLayout(idx).sign;

                    data.setItemVisual(
                        idx,
                        {
                            color: itemDto.get(
                                sign > 0 ? positiveColorQuery : negativeColorQuery
                            ),
                            borderColor: itemDto.get(
                                sign > 0 ? positiveBorderColorQuery : negativeBorderColorQuery
                            )
                        }
                    );
                });
            }
        });

    };
});