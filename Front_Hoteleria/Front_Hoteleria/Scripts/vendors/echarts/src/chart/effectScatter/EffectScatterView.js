define(function (require) {

    var SymbolDraw = require('../helper/SymbolDraw');
    var EffectSymbol = require('../helper/EffectSymbol');

    require('../../echarts').extendChartView({

        type: 'effectScatter',

        init: function () {
            this._symbolDraw = new SymbolDraw(EffectSymbol);
        },

        render: function (seriesDto, ecDto, api) {
            var data = seriesDto.getData();
            var effectSymbolDraw = this._symbolDraw;
            effectSymbolDraw.updateData(data);
            this.group.add(effectSymbolDraw.group);
        },

        updateLayout: function () {
            this._symbolDraw.updateLayout();
        },

        remove: function (ecDto, api) {
            this._symbolDraw && this._symbolDraw.remove(api);
        }
    });
});