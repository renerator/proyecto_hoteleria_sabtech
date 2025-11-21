/**
 * @file History manager.
 */
define(function(require) {

    var zrUtil = require('zrender/core/util');
    var each = zrUtil.each;

    var ATTR = '\0_ec_hist_store';

    var history = {

        /**
         * @public
         * @param {module:echarts/Dto/Global} ecDto
         * @param {Object} newSnapshot {dataZoomId, batch: [payloadInfo, ...]}
         */
        push: function (ecDto, newSnapshot) {
            var store = giveStore(ecDto);

            // If previous dataZoom can not be found,
            // complete an range with current range.
            each(newSnapshot, function (batchItem, dataZoomId) {
                var i = store.length - 1;
                for (; i >= 0; i--) {
                    var snapshot = store[i];
                    if (snapshot[dataZoomId]) {
                        break;
                    }
                }
                if (i < 0) {
                    // No origin range set, create one by current range.
                    var dataZoomDto = ecDto.queryComponents(
                        {mainType: 'dataZoom', subType: 'select', id: dataZoomId}
                    )[0];
                    if (dataZoomDto) {
                        var percentRange = dataZoomDto.getPercentRange();
                        store[0][dataZoomId] = {
                            dataZoomId: dataZoomId,
                            start: percentRange[0],
                            end: percentRange[1]
                        };
                    }
                }
            });

            store.push(newSnapshot);
        },

        /**
         * @public
         * @param {module:echarts/Dto/Global} ecDto
         * @return {Object} snapshot
         */
        pop: function (ecDto) {
            var store = giveStore(ecDto);
            var head = store[store.length - 1];
            store.length > 1 && store.pop();

            // Find top for all dataZoom.
            var snapshot = {};
            each(head, function (batchItem, dataZoomId) {
                for (var i = store.length - 1; i >= 0; i--) {
                    var batchItem = store[i][dataZoomId];
                    if (batchItem) {
                        snapshot[dataZoomId] = batchItem;
                        break;
                    }
                }
            });

            return snapshot;
        },

        /**
         * @public
         */
        clear: function (ecDto) {
            ecDto[ATTR] = null;
        },

        /**
         * @public
         * @param {module:echarts/Dto/Global} ecDto
         * @return {number} records. always >= 1.
         */
        count: function (ecDto) {
            return giveStore(ecDto).length;
        }

    };

    /**
     * [{key: dataZoomId, value: {dataZoomId, range}}, ...]
     * History length of each dataZoom may be different.
     * this._history[0] is used to store origin range.
     * @type {Array.<Object>}
     */
    function giveStore(ecDto) {
        var store = ecDto[ATTR];
        if (!store) {
            store = ecDto[ATTR] = [{}];
        }
        return store;
    }

    return history;

});