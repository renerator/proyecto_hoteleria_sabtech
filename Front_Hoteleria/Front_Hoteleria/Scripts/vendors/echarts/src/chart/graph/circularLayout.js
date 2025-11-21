define(function (require) {
    var circularLayoutHelper = require('./circularLayoutHelper');
    return function (ecDto, api) {
        ecDto.eachSeriesByType('graph', function (seriesDto) {
            if (seriesDto.get('layout') === 'circular') {
                circularLayoutHelper(seriesDto);
            }
        });
    };
});