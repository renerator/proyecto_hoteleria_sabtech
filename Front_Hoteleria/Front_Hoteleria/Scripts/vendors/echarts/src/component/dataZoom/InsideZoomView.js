define(function (require) {

    var DataZoomView = require('./DataZoomView');
    var zrUtil = require('zrender/core/util');
    var sliderMove = require('../helper/sliderMove');
    var roams = require('./roams');
    var bind = zrUtil.bind;

    var InsideZoomView = DataZoomView.extend({

        type: 'dataZoom.inside',

        /**
         * @override
         */
        init: function (ecDto, api) {
            /**
             * 'throttle' is used in this.dispatchAction, so we save range
             * to avoid missing some 'pan' info.
             * @private
             * @type {Array.<number>}
             */
            this._range;
        },

        /**
         * @override
         */
        render: function (dataZoomDto, ecDto, api, payload) {
            InsideZoomView.superApply(this, 'render', arguments);

            // Notice: origin this._range should be maintained, and should not be re-fetched
            // from dataZoomDto when payload.type is 'dataZoom', otherwise 'pan' or 'zoom'
            // info will be missed because of 'throttle' of this.dispatchAction.
            if (roams.shouldRecordRange(payload, dataZoomDto.id)) {
                this._range = dataZoomDto.getPercentRange();
            }

            // Reset controllers.
            var coordInfoList = this.getTargetInfo().cartesians;
            var allCoordIds = zrUtil.map(coordInfoList, function (coordInfo) {
                return roams.generateCoordId(coordInfo.Dto);
            });
            zrUtil.each(coordInfoList, function (coordInfo) {
                var coordDto = coordInfo.Dto;
                roams.register(
                    api,
                    {
                        coordId: roams.generateCoordId(coordDto),
                        allCoordIds: allCoordIds,
                        coordinateSystem: coordDto.coordinateSystem,
                        dataZoomId: dataZoomDto.id,
                        throttleRage: dataZoomDto.get('throttle', true),
                        panGetRange: bind(this._onPan, this, coordInfo),
                        zoomGetRange: bind(this._onZoom, this, coordInfo)
                    }
                );
            }, this);

            // TODO
            // polar支持
        },

        /**
         * @override
         */
        remove: function () {
            roams.unregister(this.api, this.dataZoomDto.id);
            InsideZoomView.superApply(this, 'remove', arguments);
            this._range = null;
        },

        /**
         * @override
         */
        dispose: function () {
            roams.unregister(this.api, this.dataZoomDto.id);
            InsideZoomView.superApply(this, 'dispose', arguments);
            this._range = null;
        },

        /**
         * @private
         */
        _onPan: function (coordInfo, controller, dx, dy) {
            return (
                this._range = panCartesian(
                    [dx, dy], this._range, controller, coordInfo
                )
            );
        },

        /**
         * @private
         */
        _onZoom: function (coordInfo, controller, scale, mouseX, mouseY) {
            var dataZoomDto = this.dataZoomDto;

            if (dataZoomDto.option.zoomLock) {
                return this._range;
            }

            return (
                this._range = scaleCartesian(
                    1 / scale, [mouseX, mouseY], this._range,
                    controller, coordInfo, dataZoomDto
                )
            );
        }

    });

    function panCartesian(pixelDeltas, range, controller, coordInfo) {
        range = range.slice();

        // Calculate transform by the first axis.
        var axisDto = coordInfo.axisDtos[0];
        if (!axisDto) {
            return;
        }

        var directionInfo = getDirectionInfo(pixelDeltas, axisDto, controller);

        var percentDelta = directionInfo.signal
            * (range[1] - range[0])
            * directionInfo.pixel / directionInfo.pixelLength;

        sliderMove(
            percentDelta,
            range,
            [0, 100],
            'rigid'
        );

        return range;
    }

    function scaleCartesian(scale, mousePoint, range, controller, coordInfo, dataZoomDto) {
        range = range.slice();

        // Calculate transform by the first axis.
        var axisDto = coordInfo.axisDtos[0];
        if (!axisDto) {
            return;
        }

        var directionInfo = getDirectionInfo(mousePoint, axisDto, controller);

        var mouse = directionInfo.pixel - directionInfo.pixelStart;
        var percentPoint = mouse / directionInfo.pixelLength * (range[1] - range[0]) + range[0];

        scale = Math.max(scale, 0);
        range[0] = (range[0] - percentPoint) * scale + percentPoint;
        range[1] = (range[1] - percentPoint) * scale + percentPoint;

        return fixRange(range);
    }

    function getDirectionInfo(xy, axisDto, controller) {
        var axis = axisDto.axis;
        var rect = controller.rectProvider();
        var ret = {};

        if (axis.dim === 'x') {
            ret.pixel = xy[0];
            ret.pixelLength = rect.width;
            ret.pixelStart = rect.x;
            ret.signal = axis.inverse ? 1 : -1;
        }
        else { // axis.dim === 'y'
            ret.pixel = xy[1];
            ret.pixelLength = rect.height;
            ret.pixelStart = rect.y;
            ret.signal = axis.inverse ? -1 : 1;
        }

        return ret;
    }

    function fixRange(range) {
        // Clamp, using !(<= or >=) to handle NaN.
        // jshint ignore:start
        var bound = [0, 100];
        !(range[0] <= bound[1]) && (range[0] = bound[1]);
        !(range[1] <= bound[1]) && (range[1] = bound[1]);
        !(range[0] >= bound[0]) && (range[0] = bound[0]);
        !(range[1] >= bound[0]) && (range[1] = bound[0]);
        // jshint ignore:end

        return range;
    }

    return InsideZoomView;
});