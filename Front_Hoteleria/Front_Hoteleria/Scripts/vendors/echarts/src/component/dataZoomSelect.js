/**
 * DataZoom component entry
 */
define(function (require) {

    require('./dataZoom/typeDefaulter');

    require('./dataZoom/DataZoomDto');
    require('./dataZoom/DataZoomView');

    require('./dataZoom/SelectZoomDto');
    require('./dataZoom/SelectZoomView');

    require('./dataZoom/dataZoomProcessor');
    require('./dataZoom/dataZoomAction');

});