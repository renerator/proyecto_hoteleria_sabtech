/**
 * @file Treemap action
 */
define(function(require) {

    var echarts = require('../../echarts');
    var helper = require('./helper');

    var noop = function () {};

    var actionTypes = [
        'treemapZoomToNode',
        'treemapRender',
        'treemapMove'
    ];

    for (var i = 0; i < actionTypes.length; i++) {
        echarts.registerAction({type: actionTypes[i], update: 'updateView'}, noop);
    }

    echarts.registerAction(
        {type: 'treemapRootToNode', update: 'updateView'},
        function (payload, ecDto) {

            ecDto.eachComponent(
                {mainType: 'series', subType: 'treemap', query: payload},
                handleRootToNode
            );

            function handleRootToNode(Dto, index) {
                var targetInfo = helper.retrieveTargetInfo(payload, Dto);

                if (targetInfo) {
                    var originViewRoot = Dto.getViewRoot();
                    if (originViewRoot) {
                        payload.direction = helper.aboveViewRoot(originViewRoot, targetInfo.node)
                            ? 'rollUp' : 'drillDown';
                    }
                    Dto.resetViewRoot(targetInfo.node);
                }
            }
        }
    );

});