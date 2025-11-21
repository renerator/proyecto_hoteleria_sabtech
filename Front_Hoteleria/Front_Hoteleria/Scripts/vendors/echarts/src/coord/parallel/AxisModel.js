define(function(require) {

    var ComponentDto = require('../../Dto/Component');
    var zrUtil = require('zrender/core/util');
    var makeStyleMapper = require('../../Dto/mixin/makeStyleMapper');
    var axisDtoCreator = require('../axisDtoCreator');
    var numberUtil = require('../../util/number');

    var AxisDto = ComponentDto.extend({

        type: 'baseParallelAxis',

        /**
         * @type {module:echarts/coord/parallel/Axis}
         */
        axis: null,

        /**
         * @type {Array.<Array.<number>}
         * @readOnly
         */
        activeIntervals: [],

        /**
         * @return {Object}
         */
        getAreaSelectStyle: function () {
            return makeStyleMapper(
                [
                    ['fill', 'color'],
                    ['lineWidth', 'borderWidth'],
                    ['stroke', 'borderColor'],
                    ['width', 'width'],
                    ['opacity', 'opacity']
                ]
            ).call(this.getDto('areaSelectStyle'));
        },

        /**
         * The code of this feature is put on AxisDto but not ParallelAxis,
         * because axisDto can be alive after echarts updating but instance of
         * ParallelAxis having been disposed. this._activeInterval should be kept
         * when action dispatched (i.e. legend click).
         *
         * @param {Array.<Array<number>>} intervals interval.length === 0
         *                                          means set all active.
         * @public
         */
        setActiveIntervals: function (intervals) {
            var activeIntervals = this.activeIntervals = zrUtil.clone(intervals);

            // Normalize
            if (activeIntervals) {
                for (var i = activeIntervals.length - 1; i >= 0; i--) {
                    numberUtil.asc(activeIntervals[i]);
                }
            }
        },

        /**
         * @param {number|string} [value] When attempting to detect 'no activeIntervals set',
         *                         value can not be input.
         * @return {string} 'normal': no activeIntervals set,
         *                  'active',
         *                  'inactive'.
         * @public
         */
        getActiveState: function (value) {
            var activeIntervals = this.activeIntervals;

            if (!activeIntervals.length) {
                return 'normal';
            }

            if (value == null) {
                return 'inactive';
            }

            for (var i = 0, len = activeIntervals.length; i < len; i++) {
                if (activeIntervals[i][0] <= value && value <= activeIntervals[i][1]) {
                    return 'active';
                }
            }
            return 'inactive';
        }

    });

    var defaultOption = {

        type: 'value',

        /**
         * @type {Array.<number>}
         */
        dim: null, // 0, 1, 2, ...

        parallelIndex: null,

        areaSelectStyle: {
            width: 20,
            borderWidth: 1,
            borderColor: 'rgba(160,197,232)',
            color: 'rgba(160,197,232)',
            opacity: 0.3
        },

        z: 10
    };

    zrUtil.merge(AxisDto.prototype, require('../axisDtoCommonMixin'));

    function getAxisType(axisName, option) {
        return option.type || (option.data ? 'category' : 'value');
    }

    axisDtoCreator('parallel', AxisDto, getAxisType, defaultOption);

    return AxisDto;
});