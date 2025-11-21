/**
 * DataZoom component entry
 */
define(function (require) {

    require('../echarts').registerPreprocessor(
        require('./visualMap/preprocessor')
    );

    require('./visualMap/typeDefaulter');
    require('./visualMap/visualCoding');
    require('./visualMap/ContinuousDto');
    require('./visualMap/ContinuousView');
    require('./visualMap/visualMapAction');

});