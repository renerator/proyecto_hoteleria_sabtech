define(function (require) {

    var DtoUtil = require('../../util/Dto');
    var zrUtil = require('zrender/core/util');

    function fillLabel(opt) {
        DtoUtil.defaultEmphasis(
            opt.label,
            DtoUtil.LABEL_OPTIONS
        );
    }

    var MarkLineDto = require('../../echarts').extendComponentDto({

        type: 'markLine',

        dependencies: ['series', 'grid', 'polar', 'geo'],
        /**
         * @overrite
         */
        init: function (option, parentDto, ecDto, extraOpt) {
            this.mergeDefaultAndTheme(option, ecDto);
            this.mergeOption(option, ecDto, extraOpt.createdBySelf, true);
        },

        mergeOption: function (newOpt, ecDto, createdBySelf, isInit) {
            if (!createdBySelf) {
                ecDto.eachSeries(function (seriesDto) {
                    var markLineOpt = seriesDto.get('markLine');
                    var mlDto = seriesDto.markLineDto;
                    if (!markLineOpt || !markLineOpt.data) {
                        seriesDto.markLineDto = null;
                        return;
                    }
                    if (!mlDto) {
                        if (isInit) {
                            // Default label emphasis `position` and `show`
                            fillLabel(markLineOpt);
                        }
                        zrUtil.each(markLineOpt.data, function (item) {
                            if (item instanceof Array) {
                                fillLabel(item[0]);
                                fillLabel(item[1]);
                            }
                            else {
                                fillLabel(item);
                            }
                        });
                        var opt = {
                            mainType: 'markLine',
                            // Use the same series index and name
                            seriesIndex: seriesDto.seriesIndex,
                            name: seriesDto.name,
                            createdBySelf: true
                        };
                        mlDto = new MarkLineDto(
                            markLineOpt, this, ecDto, opt
                        );
                    }
                    else {
                        mlDto.mergeOption(markLineOpt, ecDto, true);
                    }
                    seriesDto.markLineDto = mlDto;
                }, this);
            }
        },

        defaultOption: {
            zlevel: 0,
            z: 5,

            symbol: ['circle', 'arrow'],
            symbolSize: [8, 16],

            //symbolRotate: 0,

            precision: 2,
            tooltip: {
                trigger: 'item'
            },
            label: {
                normal: {
                    show: true,
                    position: 'end'
                },
                emphasis: {
                    show: true
                }
            },
            lineStyle: {
                normal: {
                    type: 'dashed'
                },
                emphasis: {
                    width: 3
                }
            },
            animationEasing: 'linear'
        }
    });

    return MarkLineDto;
});