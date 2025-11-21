define(function (require) {

    var Group = require('zrender/container/Group');
    var componentUtil = require('../util/component');
    var clazzUtil = require('../util/clazz');

    function Chart() {

        /**
         * @type {module:zrender/container/Group}
         * @readOnly
         */
        this.group = new Group();

        /**
         * @type {string}
         * @readOnly
         */
        this.uid = componentUtil.getUID('viewChart');
    }

    Chart.prototype = {

        type: 'chart',

        /**
         * Init the chart
         * @param  {module:echarts/Dto/Global} ecDto
         * @param  {module:echarts/ExtensionAPI} api
         */
        init: function (ecDto, api) {},

        /**
         * Render the chart
         * @param  {module:echarts/Dto/Series} seriesDto
         * @param  {module:echarts/Dto/Global} ecDto
         * @param  {module:echarts/ExtensionAPI} api
         * @param  {Object} payload
         */
        render: function (seriesDto, ecDto, api, payload) {},

        /**
         * Highlight series or specified data item
         * @param  {module:echarts/Dto/Series} seriesDto
         * @param  {module:echarts/Dto/Global} ecDto
         * @param  {module:echarts/ExtensionAPI} api
         * @param  {Object} payload
         */
        highlight: function (seriesDto, ecDto, api, payload) {
            toggleHighlight(seriesDto.getData(), payload, 'emphasis');
        },

        /**
         * Downplay series or specified data item
         * @param  {module:echarts/Dto/Series} seriesDto
         * @param  {module:echarts/Dto/Global} ecDto
         * @param  {module:echarts/ExtensionAPI} api
         * @param  {Object} payload
         */
        downplay: function (seriesDto, ecDto, api, payload) {
            toggleHighlight(seriesDto.getData(), payload, 'normal');
        },

        /**
         * Remove self
         * @param  {module:echarts/Dto/Global} ecDto
         * @param  {module:echarts/ExtensionAPI} api
         */
        remove: function (ecDto, api) {
            this.group.removeAll();
        },

        /**
         * Dispose self
         * @param  {module:echarts/Dto/Global} ecDto
         * @param  {module:echarts/ExtensionAPI} api
         */
        dispose: function () {}
    };

    var chartProto = Chart.prototype;
    chartProto.updateView
        = chartProto.updateLayout
        = chartProto.updateVisual
        = function (seriesDto, ecDto, api, payload) {
            this.render(seriesDto, ecDto, api, payload);
        };

    /**
     * Set state of single element
     * @param  {module:zrender/Element} el
     * @param  {string} state
     */
    function elSetState(el, state) {
        if (el) {
            el.trigger(state);
            if (el.type === 'group') {
                for (var i = 0; i < el.childCount(); i++) {
                    elSetState(el.childAt(i), state);
                }
            }
        }
    }
    /**
     * @param  {module:echarts/data/List} data
     * @param  {Object} payload
     * @param  {string} state 'normal'|'emphasis'
     * @inner
     */
    function toggleHighlight(data, payload, state) {
        if (payload.dataIndex != null) {
            var el = data.getItemGraphicEl(payload.dataIndex);
            elSetState(el, state);
        }
        else if (payload.name) {
            var dataIndex = data.indexOfName(payload.name);
            var el = data.getItemGraphicEl(dataIndex);
            elSetState(el, state);
        }
        else {
            data.eachItemGraphicEl(function (el) {
                elSetState(el, state);
            });
        }
    }

    // Enable Chart.extend.
    clazzUtil.enableClassExtend(Chart);

    // Add capability of registerClass, getClass, hasClass, registerSubTypeDefaulter and so on.
    clazzUtil.enableClassManagement(Chart, {registerWhenExtend: true});

    return Chart;
});