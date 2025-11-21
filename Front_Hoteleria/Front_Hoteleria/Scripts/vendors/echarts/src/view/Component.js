define(function (require) {

    var Group = require('zrender/container/Group');
    var componentUtil = require('../util/component');
    var clazzUtil = require('../util/clazz');

    var Component = function () {
        /**
         * @type {module:zrender/container/Group}
         * @readOnly
         */
        this.group = new Group();

        /**
         * @type {string}
         * @readOnly
         */
        this.uid = componentUtil.getUID('viewComponent');
    };

    Component.prototype = {

        constructor: Component,

        init: function (ecDto, api) {},

        render: function (componentDto, ecDto, api, payload) {},

        dispose: function () {}
    };

    var componentProto = Component.prototype;
    componentProto.updateView
        = componentProto.updateLayout
        = componentProto.updateVisual
        = function (seriesDto, ecDto, api, payload) {
            // Do nothing;
        };
    // Enable Component.extend.
    clazzUtil.enableClassExtend(Component);

    // Enable capability of registerClass, getClass, hasClass, registerSubTypeDefaulter and so on.
    clazzUtil.enableClassManagement(Component, {registerWhenExtend: true});

    return Component;
});