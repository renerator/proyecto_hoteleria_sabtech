/**
 * Helper function for axisLabelInterval calculation
 */

define(function(require) {
    'use strict';

    var zrUtil = require('zrender/core/util');
    var axisHelper = require('../axisHelper');

    return function (axis) {
        var axisDto = axis.Dto;
        var labelDto = axisDto.getDto('axisLabel');
        var labelInterval = labelDto.get('interval');
        if (!(axis.type === 'category' && labelInterval === 'auto')) {
            return labelInterval === 'auto' ? 0 : labelInterval;
        }

        return axisHelper.getAxisLabelInterval(
            zrUtil.map(axis.scale.getTicks(), axis.dataToCoord, axis),
            axisDto.getFormattedLabels(),
            labelDto.getDto('textStyle').getFont(),
            axis.isHorizontal()
        );
    };
});