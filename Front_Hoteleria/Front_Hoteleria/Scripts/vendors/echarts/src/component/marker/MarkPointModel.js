define(function (require) {

    var DtoUtil = require('../../util/Dto');
    var zrUtil = require('zrender/core/util');

    function fillLabel(opt) {
        DtoUtil.defaultEmphasis(
            opt.label,
            DtoUtil.LABEL_OPTIONS
        );
    }
    var MarkPointDto = require('../../echarts').extendComponentDto({

        type: 'markPoint',

        dependencies: ['series', 'grid', 'polar'],
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
                    var markPointOpt = seriesDto.get('markPoint');
                    var mpDto = seriesDto.markPointDto;
                    if (!markPointOpt || !markPointOpt.data) {
                        seriesDto.markPointDto = null;
                        return;
                    }
                    if (!mpDto) {
                        if (isInit) {
                            // Default label emphasis `position` and `show`
                            fillLabel(markPointOpt);
                        }
                        zrUtil.each(markPointOpt.data, fillLabel);
                        var opt = {
                            mainType: 'markPoint',
                            // Use the same series index and name
                            seriesIndex: seriesDto.seriesIndex,
                            name: seriesDto.name,
                            createdBySelf: true
                        };
                        mpDto = new MarkPointDto(
                            markPointOpt, this, ecDto, opt
                        );
                    }
                    else {
                        mpDto.mergeOption(markPointOpt, ecDto, true);
                    }
                    seriesDto.markPointDto = mpDto;
                }, this);
            }
        },

        defaultOption: {
            zlevel: 0,
            z: 5,
            symbol: 'pin',
            symbolSize: 50,
            //symbolRotate: 0,
            //symbolOffset: [0, 0]
            tooltip: {
                trigger: 'item'
            },
            label: {
                normal: {
                    show: true,
                    position: 'inside'
                },
                emphasis: {
                    show: true
                }
            },
            itemStyle: {
                normal: {
                    borderWidth: 2
                }
            }
        }
    });

    return MarkPointDto;
});