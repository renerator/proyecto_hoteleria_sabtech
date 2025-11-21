define(function (require) {

    return require('echarts').extendComponentView({
        type: 'bmap',

        render: function (bMapDto, ecDto, api) {
            var rendering = true;

            var bmap = bMapDto.getBMap();
            var viewportRoot = api.getZr().painter.getViewportRoot();
            var coordSys = bMapDto.coordinateSystem;
            var moveHandler = function (type, target) {
                if (rendering) {
                    return;
                }
                var offsetEl = viewportRoot.parentNode.parentNode.parentNode;
                var mapOffset = [
                    -parseInt(offsetEl.style.left, 10) || 0,
                    -parseInt(offsetEl.style.top, 10) || 0
                ];
                viewportRoot.style.left = mapOffset[0] + 'px';
                viewportRoot.style.top = mapOffset[1] + 'px';

                coordSys.setMapOffset(mapOffset);
                bMapDto.__mapOffset = mapOffset;

                api.dispatchAction({
                    type: 'bmapRoam'
                });
            };

            function zoomEndHandler() {
                if (rendering) {
                    return;
                }
                api.dispatchAction({
                    type: 'bmapRoam'
                });
            }

            bmap.removeEventListener('moving', this._oldMoveHandler);
            // FIXME
            // Moveend may be triggered by centerAndZoom method when creating coordSys next time
            // bmap.removeEventListener('moveend', this._oldMoveHandler);
            bmap.removeEventListener('zoomend', this._oldZoomEndHandler);
            bmap.addEventListener('moving', moveHandler);
            // bmap.addEventListener('moveend', moveHandler);
            bmap.addEventListener('zoomend', zoomEndHandler);

            this._oldMoveHandler = moveHandler;
            this._oldZoomEndHandler = zoomEndHandler;

            var roam = bMapDto.get('roam');
            if (roam && roam !== 'scale') {
                bmap.enableDragging();
            }
            else {
                bmap.disableDragging();
            }
            if (roam && roam !== 'move') {
                bmap.enableScrollWheelZoom();
                bmap.enableDoubleClickZoom();
                bmap.enablePinchToZoom();
            }
            else {
                bmap.disableScrollWheelZoom();
                bmap.disableDoubleClickZoom();
                bmap.disablePinchToZoom();
            }

            var originalStyle = bMapDto.__mapStyle;

            var newMapStyle = bMapDto.get('mapStyle') || {};
            // FIXME, Not use JSON methods
            var mapStyleStr = JSON.stringify(newMapStyle);
            if (JSON.stringify(originalStyle) !== mapStyleStr) {
                bmap.setMapStyle(newMapStyle);
                bMapDto.__mapStyle = JSON.parse(mapStyleStr);
            }

            rendering = false;
        }
    });
});