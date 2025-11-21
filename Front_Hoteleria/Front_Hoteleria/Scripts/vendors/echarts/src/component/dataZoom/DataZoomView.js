define(function (require) {

    var ComponentView = require('../../view/Component');

    return ComponentView.extend({

        type: 'dataZoom',

        render: function (dataZoomDto, ecDto, api, payload) {
            this.dataZoomDto = dataZoomDto;
            this.ecDto = ecDto;
            this.api = api;
        },

        /**
         * Find the first target coordinate system.
         *
         * @protected
         * @return {Object} {
         *                   cartesians: [
         *                       {Dto: coord0, axisDtos: [axis1, axis3], coordIndex: 1},
         *                       {Dto: coord1, axisDtos: [axis0, axis2], coordIndex: 0},
         *                       ...
         *                   ],  // cartesians must not be null/undefined.
         *                   polars: [
         *                       {Dto: coord0, axisDtos: [axis4], coordIndex: 0},
         *                       ...
         *                   ],  // polars must not be null/undefined.
         *                   axisDtos: [axis0, axis1, axis2, axis3, axis4]
         *                       // axisDtos must not be null/undefined.
         *                  }
         */
        getTargetInfo: function () {
            var dataZoomDto = this.dataZoomDto;
            var ecDto = this.ecDto;
            var cartesians = [];
            var polars = [];
            var axisDtos = [];

            dataZoomDto.eachTargetAxis(function (dimNames, axisIndex) {
                var axisDto = ecDto.getComponent(dimNames.axis, axisIndex);
                if (axisDto) {
                    axisDtos.push(axisDto);

                    var gridIndex = axisDto.get('gridIndex');
                    var polarIndex = axisDto.get('polarIndex');

                    if (gridIndex != null) {
                        var coordDto = ecDto.getComponent('grid', gridIndex);
                        save(coordDto, axisDto, cartesians, gridIndex);
                    }
                    else if (polarIndex != null) {
                        var coordDto = ecDto.getComponent('polar', polarIndex);
                        save(coordDto, axisDto, polars, polarIndex);
                    }
                }
            }, this);

            function save(coordDto, axisDto, store, coordIndex) {
                var item;
                for (var i = 0; i < store.length; i++) {
                    if (store[i].Dto === coordDto) {
                        item = store[i];
                        break;
                    }
                }
                if (!item) {
                    store.push(item = {
                        Dto: coordDto, axisDtos: [], coordIndex: coordIndex
                    });
                }
                item.axisDtos.push(axisDto);
            }

            return {
                cartesians: cartesians,
                polars: polars,
                axisDtos: axisDtos
            };
        }

    });

});