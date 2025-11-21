define(function (require) {

    var featureManager = require('./featureManager');
    var zrUtil = require('zrender/core/util');
    var graphic = require('../../util/graphic');
    var Dto = require('../../Dto/Dto');
    var DataDiffer = require('../../data/DataDiffer');
    var listComponentHelper = require('../helper/listComponent');
    var textContain = require('zrender/contain/text');

    return require('../../echarts').extendComponentView({

        type: 'toolbox',

        render: function (toolboxDto, ecDto, api) {
            var group = this.group;
            group.removeAll();

            if (!toolboxDto.get('show')) {
                return;
            }

            var itemSize = +toolboxDto.get('itemSize');
            var featureOpts = toolboxDto.get('feature') || {};
            var features = this._features || (this._features = {});

            var featureNames = [];
            zrUtil.each(featureOpts, function (opt, name) {
                featureNames.push(name);
            });

            (new DataDiffer(this._featureNames || [], featureNames))
                .add(process)
                .update(process)
                .remove(zrUtil.curry(process, null))
                .execute();

            // Keep for diff.
            this._featureNames = featureNames;

            function process(newIndex, oldIndex) {
                var featureName = featureNames[newIndex];
                var oldName = featureNames[oldIndex];
                var featureOpt = featureOpts[featureName];
                var featureDto = new Dto(featureOpt, toolboxDto, toolboxDto.ecDto);
                var feature;

                if (featureName && !oldName) { // Create
                    if (isUserFeatureName(featureName)) {
                        feature = {
                            Dto: featureDto,
                            onclick: featureDto.option.onclick,
                            featureName: featureName
                        };
                    }
                    else {
                        var Feature = featureManager.get(featureName);
                        if (!Feature) {
                            return;
                        }
                        feature = new Feature(featureDto);
                    }
                    features[featureName] = feature;
                }
                else {
                    feature = features[oldName];
                    // If feature does not exsit.
                    if (!feature) {
                        return;
                    }
                    feature.Dto = featureDto;
                }

                if (!featureName && oldName) {
                    feature.dispose && feature.dispose(ecDto, api);
                    return;
                }

                if (!featureDto.get('show') || feature.unusable) {
                    feature.remove && feature.remove(ecDto, api);
                    return;
                }

                createIconPaths(featureDto, feature, featureName);

                featureDto.setIconStatus = function (iconName, status) {
                    var option = this.option;
                    var iconPaths = this.iconPaths;
                    option.iconStatus = option.iconStatus || {};
                    option.iconStatus[iconName] = status;
                    // FIXME
                    iconPaths[iconName] && iconPaths[iconName].trigger(status);
                };

                if (feature.render) {
                    feature.render(featureDto, ecDto, api);
                }
            }

            function createIconPaths(featureDto, feature, featureName) {
                var iconStyleDto = featureDto.getDto('iconStyle');

                // If one feature has mutiple icon. they are orginaized as
                // {
                //     icon: {
                //         foo: '',
                //         bar: ''
                //     },
                //     title: {
                //         foo: '',
                //         bar: ''
                //     }
                // }
                var icons = feature.getIcons ? feature.getIcons() : featureDto.get('icon');
                var titles = featureDto.get('title') || {};
                if (typeof icons === 'string') {
                    var icon = icons;
                    var title = titles;
                    icons = {};
                    titles = {};
                    icons[featureName] = icon;
                    titles[featureName] = title;
                }
                var iconPaths = featureDto.iconPaths = {};
                zrUtil.each(icons, function (icon, iconName) {
                    var normalStyle = iconStyleDto.getDto('normal').getItemStyle();
                    var hoverStyle = iconStyleDto.getDto('emphasis').getItemStyle();

                    var style = {
                        x: -itemSize / 2,
                        y: -itemSize / 2,
                        width: itemSize,
                        height: itemSize
                    };
                    var path = icon.indexOf('image://') === 0
                        ? (
                            style.image = icon.slice(8),
                            new graphic.Image({style: style})
                        )
                        : graphic.makePath(
                            icon.replace('path://', ''),
                            {
                                style: normalStyle,
                                hoverStyle: hoverStyle,
                                rectHover: true
                            },
                            style,
                            'center'
                        );

                    graphic.setHoverStyle(path);

                    if (toolboxDto.get('showTitle')) {
                        path.__title = titles[iconName];
                        path.on('mouseover', function () {
                                path.setStyle({
                                    text: titles[iconName],
                                    textPosition: hoverStyle.textPosition || 'bottom',
                                    textFill: hoverStyle.fill || hoverStyle.stroke || '#000',
                                    textAlign: hoverStyle.textAlign || 'center'
                                });
                            })
                            .on('mouseout', function () {
                                path.setStyle({
                                    textFill: null
                                });
                            });
                    }
                    path.trigger(featureDto.get('iconStatus.' + iconName) || 'normal');

                    group.add(path);
                    path.on('click', zrUtil.bind(
                        feature.onclick, feature, ecDto, api, iconName
                    ));

                    iconPaths[iconName] = path;
                });
            }

            listComponentHelper.layout(group, toolboxDto, api);
            // Render background after group is layout
            // FIXME
            listComponentHelper.addBackground(group, toolboxDto);

            // Adjust icon title positions to avoid them out of screen
            group.eachChild(function (icon) {
                var titleText = icon.__title;
                var hoverStyle = icon.hoverStyle;
                // May be background element
                if (hoverStyle && titleText) {
                    var rect = textContain.getBoundingRect(
                        titleText, hoverStyle.font
                    );
                    var offsetX = icon.position[0] + group.position[0];
                    var offsetY = icon.position[1] + group.position[1] + itemSize;

                    var needPutOnTop = false;
                    if (offsetY + rect.height > api.getHeight()) {
                        hoverStyle.textPosition = 'top';
                        needPutOnTop = true;
                    }
                    var topOffset = needPutOnTop ? (-5 - rect.height) : (itemSize + 8);
                    if (offsetX + rect.width /  2 > api.getWidth()) {
                        hoverStyle.textPosition = ['100%', topOffset];
                        hoverStyle.textAlign = 'right';
                    }
                    else if (offsetX - rect.width / 2 < 0) {
                        hoverStyle.textPosition = [0, topOffset];
                        hoverStyle.textAlign = 'left';
                    }
                }
            });
        },

        remove: function (ecDto, api) {
            zrUtil.each(this._features, function (feature) {
                feature.remove && feature.remove(ecDto, api);
            });
            this.group.removeAll();
        },

        dispose: function (ecDto, api) {
            zrUtil.each(this._features, function (feature) {
                feature.dispose && feature.dispose(ecDto, api);
            });
        }
    });

    function isUserFeatureName(featureName) {
        return featureName.indexOf('my') === 0;
    }

});