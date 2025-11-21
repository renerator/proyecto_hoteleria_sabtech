/**
 * DataZoom component entry
 */
define(function (require) {

    require('./dataZoom/typeDefaulter');

    require('./dataZoom/DataZoomDto');
    require('./dataZoom/DataZoomView');

    require('./dataZoom/SliderZoomDto');
    require('./dataZoom/SliderZoomView');

    require('./dataZoom/InsideZoomDto');
    require('./dataZoom/InsideZoomView');

    require('./dataZoom/dataZoomProcessor');
    require('./dataZoom/dataZoomAction');

});