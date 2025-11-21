/**
 * DataZoom component entry
 */
define(function (require) {

    require('./dataZoom/typeDefaulter');

    require('./dataZoom/DataZoomDto');
    require('./dataZoom/DataZoomView');

    require('./dataZoom/InsideZoomDto');
    require('./dataZoom/InsideZoomView');

    require('./dataZoom/dataZoomProcessor');
    require('./dataZoom/dataZoomAction');

});