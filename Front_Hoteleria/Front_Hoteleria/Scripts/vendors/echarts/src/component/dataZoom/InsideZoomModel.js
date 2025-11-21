/**
 * @file Data zoom Dto
 */
define(function(require) {

    return require('./DataZoomDto').extend({

        type: 'dataZoom.inside',

        /**
         * @protected
         */
        defaultOption: {
            zoomLock: false // Whether disable zoom but only pan.
        }
    });
});