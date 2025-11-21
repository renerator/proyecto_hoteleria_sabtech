define(function (require) {

    var VisualMapping = require('../../visual/VisualMapping');
    var zrColor = require('zrender/tool/color');
    var zrUtil = require('zrender/core/util');
    var isArray = zrUtil.isArray;

    var ITEM_STYLE_NORMAL = 'itemStyle.normal';

    return function (ecDto, payload) {

        var condition = {mainType: 'series', subType: 'treemap', query: payload};
        ecDto.eachComponent(condition, function (seriesDto) {

            var tree = seriesDto.getData().tree;
            var root = tree.root;
            var seriesItemStyleDto = seriesDto.getDto(ITEM_STYLE_NORMAL);

            if (root.isRemoved()) {
                return;
            }

            var levelItemStyles = zrUtil.map(tree.levelDtos, function (levelDto) {
                return levelDto ? levelDto.get(ITEM_STYLE_NORMAL) : null;
            });

            travelTree(
                root, // Visual should calculate from tree root but not view root.
                {},
                levelItemStyles,
                seriesItemStyleDto,
                seriesDto.getViewRoot().getAncestors(),
                seriesDto
            );
        });
    };

    function travelTree(
        node, designatedVisual, levelItemStyles, seriesItemStyleDto,
        viewRootAncestors, seriesDto
    ) {
        var nodeDto = node.getDto();
        var nodeLayout = node.getLayout();

        // Optimize
        if (!nodeLayout || nodeLayout.invisible || !nodeLayout.isInView) {
            return;
        }

        var nodeItemStyleDto = node.getDto(ITEM_STYLE_NORMAL);
        var levelItemStyle = levelItemStyles[node.depth];
        var visuals = buildVisuals(
            nodeItemStyleDto, designatedVisual, levelItemStyle, seriesItemStyleDto
        );

        // calculate border color
        var borderColor = nodeItemStyleDto.get('borderColor');
        var borderColorSaturation = nodeItemStyleDto.get('borderColorSaturation');
        var thisNodeColor;
        if (borderColorSaturation != null) {
            // For performance, do not always execute 'calculateColor'.
            thisNodeColor = calculateColor(visuals, node);
            borderColor = calculateBorderColor(borderColorSaturation, thisNodeColor);
        }
        node.setVisual('borderColor', borderColor);

        var viewChildren = node.viewChildren;
        if (!viewChildren || !viewChildren.length) {
            thisNodeColor = calculateColor(visuals, node);
            // Apply visual to this node.
            node.setVisual('color', thisNodeColor);
        }
        else {
            var mapping = buildVisualMapping(
                node, nodeDto, nodeLayout, nodeItemStyleDto, visuals, viewChildren
            );
            // Designate visual to children.
            zrUtil.each(viewChildren, function (child, index) {
                // If higher than viewRoot, only ancestors of viewRoot is needed to visit.
                if (child.depth >= viewRootAncestors.length
                    || child === viewRootAncestors[child.depth]
                ) {
                    var childVisual = mapVisual(
                        nodeDto, visuals, child, index, mapping, seriesDto
                    );
                    travelTree(
                        child, childVisual, levelItemStyles, seriesItemStyleDto,
                        viewRootAncestors, seriesDto
                    );
                }
            });
        }
    }

    function buildVisuals(
        nodeItemStyleDto, designatedVisual, levelItemStyle, seriesItemStyleDto
    ) {
        var visuals = zrUtil.extend({}, designatedVisual);

        zrUtil.each(['color', 'colorAlpha', 'colorSaturation'], function (visualName) {
            // Priority: thisNode > thisLevel > parentNodeDesignated > seriesDto
            var val = nodeItemStyleDto.get(visualName, true); // Ignore parent
            val == null && levelItemStyle && (val = levelItemStyle[visualName]);
            val == null && (val = designatedVisual[visualName]);
            val == null && (val = seriesItemStyleDto.get(visualName));

            val != null && (visuals[visualName] = val);
        });

        return visuals;
    }

    function calculateColor(visuals) {
        var color = getValueVisualDefine(visuals, 'color');

        if (color) {
            var colorAlpha = getValueVisualDefine(visuals, 'colorAlpha');
            var colorSaturation = getValueVisualDefine(visuals, 'colorSaturation');
            if (colorSaturation) {
                color = zrColor.modifyHSL(color, null, null, colorSaturation);
            }
            if (colorAlpha) {
                color = zrColor.modifyAlpha(color, colorAlpha);
            }

            return color;
        }
    }

    function calculateBorderColor(borderColorSaturation, thisNodeColor) {
        return thisNodeColor != null
             ? zrColor.modifyHSL(thisNodeColor, null, null, borderColorSaturation)
             : null;
    }

    function getValueVisualDefine(visuals, name) {
        var value = visuals[name];
        if (value != null && value !== 'none') {
            return value;
        }
    }

    function buildVisualMapping(
        node, nodeDto, nodeLayout, nodeItemStyleDto, visuals, viewChildren
    ) {
        if (!viewChildren || !viewChildren.length) {
            return;
        }

        var rangeVisual = getRangeVisual(nodeDto, 'color')
            || (
                visuals.color != null
                && visuals.color !== 'none'
                && (
                    getRangeVisual(nodeDto, 'colorAlpha')
                    || getRangeVisual(nodeDto, 'colorSaturation')
                )
            );

        if (!rangeVisual) {
            return;
        }

        var colorMappingBy = nodeDto.get('colorMappingBy');
        var opt = {
            type: rangeVisual.name,
            dataExtent: nodeLayout.dataExtent,
            visual: rangeVisual.range
        };
        if (opt.type === 'color'
            && (colorMappingBy === 'index' || colorMappingBy === 'id')
        ) {
            opt.mappingMethod = 'category';
            opt.loop = true;
            // categories is ordinal, so do not set opt.categories.
        }
        else {
            opt.mappingMethod = 'linear';
        }

        var mapping = new VisualMapping(opt);
        mapping.__drColorMappingBy = colorMappingBy;

        return mapping;
    }

    // Notice: If we dont have the attribute 'colorRange', but only use
    // attribute 'color' to represent both concepts of 'colorRange' and 'color',
    // (It means 'colorRange' when 'color' is Array, means 'color' when not array),
    // this problem will be encountered:
    // If a level-1 node dont have children, and its siblings has children,
    // and colorRange is set on level-1, then the node can not be colored.
    // So we separate 'colorRange' and 'color' to different attributes.
    function getRangeVisual(nodeDto, name) {
        // 'colorRange', 'colorARange', 'colorSRange'.
        // If not exsits on this node, fetch from levels and series.
        var range = nodeDto.get(name);
        return (isArray(range) && range.length) ? {name: name, range: range} : null;
    }

    function mapVisual(nodeDto, visuals, child, index, mapping, seriesDto) {
        var childVisuals = zrUtil.extend({}, visuals);

        if (mapping) {
            var mappingType = mapping.type;
            var colorMappingBy = mappingType === 'color' && mapping.__drColorMappingBy;
            var value =
                colorMappingBy === 'index'
                ? index
                : colorMappingBy === 'id'
                ? seriesDto.mapIdToIndex(child.getId())
                : child.getValue(nodeDto.get('visualDimension'));

            childVisuals[mappingType] = mapping.mapValueToVisual(value);
        }

        return childVisuals;
    }

});