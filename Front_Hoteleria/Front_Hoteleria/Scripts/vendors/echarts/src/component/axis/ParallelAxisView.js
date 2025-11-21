define(function (require) {

    var zrUtil = require('zrender/core/util');
    var AxisBuilder = require('./AxisBuilder');
    var SelectController = require('../helper/SelectController');

    var elementList = ['axisLine', 'axisLabel', 'axisTick', 'axisName'];

    var AxisView = require('../../echarts').extendComponentView({

        type: 'parallelAxis',

        /**
         * @type {module:echarts/component/helper/SelectController}
         */
        _selectController: null,

        /**
         * @override
         */
        render: function (axisDto, ecDto, api, payload) {
            if (fromAxisAreaSelect(axisDto, ecDto, payload)) {
                return;
            }

            this.axisDto = axisDto;
            this.api = api;

            this.group.removeAll();

            if (!axisDto.get('show')) {
                return;
            }

            var coordSys = ecDto.getComponent(
                'parallel', axisDto.get('parallelIndex')
            ).coordinateSystem;

            var areaSelectStyle = axisDto.getAreaSelectStyle();
            var areaWidth = areaSelectStyle.width;

            var axisLayout = coordSys.getAxisLayout(axisDto.axis.dim);
            var builderOpt = zrUtil.extend(
                {
                    strokeContainThreshold: areaWidth,
                    // lineWidth === 0 or no value.
                    axisLineSilent: !(areaWidth > 0) // jshint ignore:line
                },
                axisLayout
            );

            var axisBuilder = new AxisBuilder(axisDto, builderOpt);

            zrUtil.each(elementList, axisBuilder.add, axisBuilder);

            var axisGroup = axisBuilder.getGroup();

            this.group.add(axisGroup);

            this._buildSelectController(
                axisGroup, areaSelectStyle, axisDto, api
            );
        },

        _buildSelectController: function (axisGroup, areaSelectStyle, axisDto, api) {

            var axis = axisDto.axis;
            var selectController = this._selectController;

            if (!selectController) {
                selectController = this._selectController = new SelectController(
                    'line',
                    api.getZr(),
                    areaSelectStyle
                );

                selectController.on('selected', zrUtil.bind(this._onSelected, this));
            }

            selectController.enable(axisGroup);

            // After filtering, axis may change, select area needs to be update.
            var ranges = zrUtil.map(axisDto.activeIntervals, function (interval) {
                return [
                    axis.dataToCoord(interval[0], true),
                    axis.dataToCoord(interval[1], true)
                ];
            });
            selectController.update(ranges);
        },

        _onSelected: function (ranges) {
            // Do not cache these object, because the mey be changed.
            var axisDto = this.axisDto;
            var axis = axisDto.axis;

            var intervals = zrUtil.map(ranges, function (range) {
                return [
                    axis.coordToData(range[0], true),
                    axis.coordToData(range[1], true)
                ];
            });
            this.api.dispatchAction({
                type: 'axisAreaSelect',
                parallelAxisId: axisDto.id,
                intervals: intervals
            });
        },

        /**
         * @override
         */
        remove: function () {
            this._selectController && this._selectController.disable();
        },

        /**
         * @override
         */
        dispose: function () {
            if (this._selectController) {
                this._selectController.dispose();
                this._selectController = null;
            }
        }
    });

    function fromAxisAreaSelect(axisDto, ecDto, payload) {
        return payload
            && payload.type === 'axisAreaSelect'
            && ecDto.findComponents(
                {mainType: 'parallelAxis', query: payload}
            )[0] === axisDto;
    }

    return AxisView;
});