// TODO Axis scale
define(function (require) {

    var Polar = require('./Polar');
    var numberUtil = require('../../util/number');

    var axisHelper = require('../../coord/axisHelper');
    var niceScaleExtent = axisHelper.niceScaleExtent;

    // 依赖 PolarDto 做预处理
    require('./PolarDto');

    /**
     * Resize method bound to the polar
     * @param {module:echarts/coord/polar/PolarDto} polarDto
     * @param {module:echarts/ExtensionAPI} api
     */
    function resizePolar(polarDto, api) {
        var center = polarDto.get('center');
        var radius = polarDto.get('radius');
        var width = api.getWidth();
        var height = api.getHeight();
        var parsePercent = numberUtil.parsePercent;

        this.cx = parsePercent(center[0], width);
        this.cy = parsePercent(center[1], height);

        var radiusAxis = this.getRadiusAxis();
        var size = Math.min(width, height) / 2;
        // var idx = radiusAxis.inverse ? 1 : 0;
        radiusAxis.setExtent(0, parsePercent(radius, size));
    }

    /**
     * Update polar
     */
    function updatePolarScale(ecDto, api) {
        var polar = this;
        var angleAxis = polar.getAngleAxis();
        var radiusAxis = polar.getRadiusAxis();
        // Reset scale
        angleAxis.scale.setExtent(Infinity, -Infinity);
        radiusAxis.scale.setExtent(Infinity, -Infinity);

        ecDto.eachSeries(function (seriesDto) {
            if (seriesDto.coordinateSystem === polar) {
                var data = seriesDto.getData();
                radiusAxis.scale.unionExtent(
                    data.getDataExtent('radius', radiusAxis.type !== 'category')
                );
                angleAxis.scale.unionExtent(
                    data.getDataExtent('angle', angleAxis.type !== 'category')
                );
            }
        });

        niceScaleExtent(angleAxis, angleAxis.Dto);
        niceScaleExtent(radiusAxis, radiusAxis.Dto);

        // Fix extent of category angle axis
        if (angleAxis.type === 'category' && !angleAxis.onBand) {
            var extent = angleAxis.getExtent();
            var diff = 360 / angleAxis.scale.count();
            angleAxis.inverse ? (extent[1] += diff) : (extent[1] -= diff);
            angleAxis.setExtent(extent[0], extent[1]);
        }
    }

    /**
     * Set common axis properties
     * @param {module:echarts/coord/polar/AngleAxis|module:echarts/coord/polar/RadiusAxis}
     * @param {module:echarts/coord/polar/AxisDto}
     * @inner
     */
    function setAxis(axis, axisDto) {
        axis.type = axisDto.get('type');
        axis.scale = axisHelper.createScaleByDto(axisDto);
        axis.onBand = axisDto.get('boundaryGap') && axis.type === 'category';

        // FIXME Radius axis not support inverse axis
        if (axisDto.mainType === 'angleAxis') {
            var startAngle = axisDto.get('startAngle');
            axis.inverse = axisDto.get('inverse') ^ axisDto.get('clockwise');
            axis.setExtent(startAngle, startAngle + (axis.inverse ? -360 : 360));
        }

        // Inject axis instance
        axisDto.axis = axis;
        axis.Dto = axisDto;
    }


    var polarCreator = {

        dimensions: Polar.prototype.dimensions,

        create: function (ecDto, api) {
            var polarList = [];
            ecDto.eachComponent('polar', function (polarDto, idx) {
                var polar = new Polar(idx);
                // Inject resize and update method
                polar.resize = resizePolar;
                polar.update = updatePolarScale;

                var radiusAxis = polar.getRadiusAxis();
                var angleAxis = polar.getAngleAxis();

                var radiusAxisDto = polarDto.findAxisDto('radiusAxis');
                var angleAxisDto = polarDto.findAxisDto('angleAxis');

                setAxis(radiusAxis, radiusAxisDto);
                setAxis(angleAxis, angleAxisDto);

                polar.resize(polarDto, api);
                polarList.push(polar);

                polarDto.coordinateSystem = polar;
            });
            // Inject coordinateSystem to series
            ecDto.eachSeries(function (seriesDto) {
                if (seriesDto.get('coordinateSystem') === 'polar') {
                    seriesDto.coordinateSystem = polarList[seriesDto.get('polarIndex')];
                }
            });

            return polarList;
        }
    };

    require('../../CoordinateSystem').register('polar', polarCreator);
});