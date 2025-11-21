define(function (require) {

    var SymbolDraw = require('../helper/SymbolDraw');
    var LargeSymbolDraw = require('../helper/LargeSymbolDraw');

    require('../../echarts').extendChartView({

        type: 'scatter',

        init: function () {
            this._normalSymbolDraw = new SymbolDraw();
            this._largeSymbolDraw = new LargeSymbolDraw();
        },

        render: function (seriesDto, ecDto, api) {
            var data = seriesDto.getData();
            var largeSymbolDraw = this._largeSymbolDraw;
            var normalSymbolDraw = this._normalSymbolDraw;
            var group = this.group;

            var symbolDraw = seriesDto.get('large') && data.count() > seriesDto.get('largeThreshold')
                ? largeSymbolDraw : normalSymbolDraw;

            this._symbolDraw = symbolDraw;
            symbolDraw.updateData(data);
            group.add(symbolDraw.group);

            group.remove(
                symbolDraw === largeSymbolDraw
                ? normalSymbolDraw.group : largeSymbolDraw.group
            );
        },

        updateLayout: function (seriesDto) {
            this._symbolDraw.updateLayout(seriesDto);
        },

        remove: function (ecDto, api) {
            this._symbolDraw && this._symbolDraw.remove(api, true);
        }
    });
});