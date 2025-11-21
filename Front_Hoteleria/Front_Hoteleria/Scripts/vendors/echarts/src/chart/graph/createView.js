define(function (require) {
    // FIXME Where to create the simple view coordinate system
    var View = require('../../coord/View');
    var layout = require('../../util/layout');
    var bbox = require('zrender/core/bbox');

    function getViewRect(seriesDto, api, aspect) {
        var option = seriesDto.getBoxLayoutParams();
        option.aspect = aspect;
        return layout.getLayoutRect(option, {
            width: api.getWidth(),
            height: api.getHeight()
        });
    }

    return function (ecDto, api) {
        var viewList = [];
        ecDto.eachSeriesByType('graph', function (seriesDto) {
            var coordSysType = seriesDto.get('coordinateSystem');
            if (!coordSysType || coordSysType === 'view') {
                var viewCoordSys = new View();
                viewList.push(viewCoordSys);

                var data = seriesDto.getData();
                var positions = data.mapArray(function (idx) {
                    var itemDto = data.getItemDto(idx);
                    return [+itemDto.get('x'), +itemDto.get('y')];
                });

                var min = [];
                var max = [];

                bbox.fromPoints(positions, min, max);

                // If width or height is 0
                if (max[0] - min[0] === 0) {
                    max[0] += 1;
                    min[0] -= 1;
                }
                if (max[1] - min[1] === 0) {
                    max[1] += 1;
                    min[1] -= 1;
                }
                var aspect = (max[0] - min[0]) / (max[1] - min[1]);
                // FIXME If get view rect after data processed?
                var viewRect = getViewRect(seriesDto, api, aspect);
                // Position may be NaN, use view rect instead
                if (isNaN(aspect)) {
                    min = [viewRect.x, viewRect.y];
                    max = [viewRect.x + viewRect.width, viewRect.y + viewRect.height];
                }

                var bbWidth = max[0] - min[0];
                var bbHeight = max[1] - min[1];

                var viewWidth = viewRect.width;
                var viewHeight = viewRect.height;

                viewCoordSys = seriesDto.coordinateSystem = new View();
                viewCoordSys.zoomLimit = seriesDto.get('scaleLimit');

                viewCoordSys.setBoundingRect(
                    min[0], min[1], bbWidth, bbHeight
                );
                viewCoordSys.setViewRect(
                    viewRect.x, viewRect.y, viewWidth, viewHeight
                );

                // Update roam info
                viewCoordSys.setCenter(seriesDto.get('center'));
                viewCoordSys.setZoom(seriesDto.get('zoom'));
            }
        });
        return viewList;
    };
});