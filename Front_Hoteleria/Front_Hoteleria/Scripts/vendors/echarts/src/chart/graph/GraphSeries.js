define(function (require) {

    'use strict';

    var List = require('../../data/List');
    var zrUtil = require('zrender/core/util');
    var DtoUtil = require('../../util/Dto');
    var Dto = require('../../Dto/Dto');

    var createGraphFromNodeEdge = require('../helper/createGraphFromNodeEdge');

    var GraphSeries = require('../../echarts').extendSeriesDto({

        type: 'series.graph',

        init: function (option) {
            GraphSeries.superApply(this, 'init', arguments);

            // Provide data for legend select
            this.legendDataProvider = function () {
                return this._categoriesData;
            };

            this.fillDataTextStyle(option.edges || option.links);

            this._updateCategoriesData();
        },

        mergeOption: function (option) {
            GraphSeries.superApply(this, 'mergeOption', arguments);

            this.fillDataTextStyle(option.edges || option.links);

            this._updateCategoriesData();
        },

        mergeDefaultAndTheme: function (option) {
            GraphSeries.superApply(this, 'mergeDefaultAndTheme', arguments);
            DtoUtil.defaultEmphasis(option.edgeLabel, DtoUtil.LABEL_OPTIONS);
        },

        getInitialData: function (option, ecDto) {
            var edges = option.edges || option.links || [];
            var nodes = option.data || option.nodes || [];
            var self = this;

            if (nodes && edges) {
                return createGraphFromNodeEdge(nodes, edges, this, true, beforeLink).data;
            }

            function beforeLink(nodeData, edgeData) {
                // Overwrite nodeData.getItemDto to
                nodeData.wrapMethod('getItemDto', function (Dto) {
                    var categoriesDtos = self._categoriesDtos;
                    var categoryIdx = Dto.getShallow('category');
                    var categoryDto = categoriesDtos[categoryIdx];
                    if (categoryDto) {
                        categoryDto.parentDto = Dto.parentDto;
                        Dto.parentDto = categoryDto;
                    }
                    return Dto;
                });

                var edgeLabelDto = self.getDto('edgeLabel');
                var wrappedGetEdgeDto = function (path, parentDto) {
                    var pathArr = (path || '').split('.');
                    if (pathArr[0] === 'label') {
                        parentDto = parentDto
                            || edgeLabelDto.getDto(pathArr.slice(1));
                    }
                    var Dto = Dto.prototype.getDto.call(this, pathArr, parentDto);
                    Dto.getDto = wrappedGetEdgeDto;
                    return Dto;
                };
                edgeData.wrapMethod('getItemDto', function (Dto) {
                    // FIXME Wrap get method ?
                    Dto.getDto = wrappedGetEdgeDto;
                    return Dto;
                });
            }
        },

        /**
         * @return {module:echarts/data/Graph}
         */
        getGraph: function () {
            return this.getData().graph;
        },

        /**
         * @return {module:echarts/data/List}
         */
        getEdgeData: function () {
            return this.getGraph().edgeData;
        },

        /**
         * @return {module:echarts/data/List}
         */
        getCategoriesData: function () {
            return this._categoriesData;
        },

        /**
         * @override
         */
        formatTooltip: function (dataIndex, multipleSeries, dataType) {
            if (dataType === 'edge') {
                var nodeData = this.getData();
                var params = this.getDataParams(dataIndex, dataType);
                var edge = nodeData.graph.getEdgeByIndex(dataIndex);
                var sourceName = nodeData.getName(edge.node1.dataIndex);
                var targetName = nodeData.getName(edge.node2.dataIndex);
                var html = sourceName + ' > ' + targetName;
                if (params.value) {
                    html += ' : ' + params.value;
                }
                return html;
            }
            else { // dataType === 'node' or empty
                return GraphSeries.superApply(this, 'formatTooltip', arguments);
            }
        },

        _updateCategoriesData: function () {
            var categories = zrUtil.map(this.option.categories || [], function (category) {
                // Data must has value
                return category.value != null ? category : zrUtil.extend({
                    value: 0
                }, category);
            });
            var categoriesData = new List(['value'], this);
            categoriesData.initData(categories);

            this._categoriesData = categoriesData;

            this._categoriesDtos = categoriesData.mapArray(function (idx) {
                return categoriesData.getItemDto(idx, true);
            });
        },

        setZoom: function (zoom) {
            this.option.zoom = zoom;
        },

        setCenter: function (center) {
            this.option.center = center;
        },

        defaultOption: {
            zlevel: 0,
            z: 2,

            color: ['#61a0a8', '#d14a61', '#fd9c35', '#675bba', '#fec42c',
                    '#dd4444', '#fd9c35', '#cd4870'],

            coordinateSystem: 'view',

            // Default option for all coordinate systems
            xAxisIndex: 0,
            yAxisIndex: 0,
            polarIndex: 0,
            geoIndex: 0,

            legendHoverLink: true,

            hoverAnimation: true,

            layout: null,

            // Configuration of force
            force: {
                initLayout: null,
                repulsion: 50,
                gravity: 0.1,
                edgeLength: 30,

                layoutAnimation: true
            },

            left: 'center',
            top: 'center',
            // right: null,
            // bottom: null,
            // width: '80%',
            // height: '80%',

            symbol: 'circle',
            symbolSize: 10,

            edgeSymbol: ['none', 'none'],
            edgeSymbolSize: 10,
            edgeLabel: {
                normal: {
                    position: 'middle'
                },
                emphasis: {}
            },

            draggable: false,

            roam: false,

            // Default on center of graph
            center: null,

            zoom: 1,
            // Symbol size scale ratio in roam
            nodeScaleRatio: 0.6,

            // categories: [],

            // data: []
            // Or
            // nodes: []
            //
            // links: []
            // Or
            // edges: []

            label: {
                normal: {
                    show: false,
                    formatter: '{b}'
                },
                emphasis: {
                    show: true
                }
            },

            itemStyle: {
                normal: {},
                emphasis: {}
            },

            lineStyle: {
                normal: {
                    color: '#aaa',
                    width: 1,
                    curveness: 0,
                    opacity: 0.5
                },
                emphasis: {}
            }
        }
    });

    return GraphSeries;
});