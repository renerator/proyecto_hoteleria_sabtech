define(function (require) {

    require('../../Dto/Component').registerSubTypeDefaulter('timeline', function () {
        // Only slider now.
        return 'slider';
    });

});