describe('util/Dto', function() {

    var utHelper = window.utHelper;
    var DtoUtil;

    beforeAll(function (done) { // jshint ignore:line
        utHelper.resetPackageLoader(function () {
            window.require(['echarts/util/Dto'], function (h) {
                DtoUtil = h;
                done();
            });
        });
    });

    function makeRecords(result) {
        var o = {};
        DtoUtil.eachAxisDim(function (dimNames) {
            o[dimNames.name] = {};
            var r = result[dimNames.name] || [];
            for (var i = 0; i < r.length; i++) {
                o[dimNames.name][r[i]] = true;
            }
        });
        return o;
    }

    describe('findLinkedNodes', function () {

        function forEachDto(Dtos, callback) {
            for (var i = 0; i < Dtos.length; i++) {
                callback(Dtos[i]);
            }
        }

        function axisIndicesGetter(Dto, dimNames) {
            return Dto[dimNames.axisIndex];
        }

        it('findLinkedNodes_base', function (done) {
            var Dtos = [
                {xAxisIndex: [1, 2], yAxisIndex: [0]},
                {xAxisIndex: [3], yAxisIndex: [1]},
                {xAxisIndex: [5], yAxisIndex: []},
                {xAxisIndex: [2, 5], yAxisIndex: []}
            ];
            var result = DtoUtil.createLinkedNodesFinder(
                utHelper.curry(forEachDto, Dtos),
                DtoUtil.eachAxisDim,
                axisIndicesGetter
            )(Dtos[0]);
            expect(result).toEqual({
                nodes: [Dtos[0], Dtos[3], Dtos[2]],
                records: makeRecords({x: [1, 2, 5], y: [0]})
            });
            done();
        });

        it('findLinkedNodes_crossXY', function (done) {
            var Dtos = [
                {xAxisIndex: [1, 2], yAxisIndex: [0]},
                {xAxisIndex: [3], yAxisIndex: [3, 0]},
                {xAxisIndex: [6, 3], yAxisIndex: [9]},
                {xAxisIndex: [5, 3], yAxisIndex: []},
                {xAxisIndex: [8], yAxisIndex: [4]}
            ];
            var result = DtoUtil.createLinkedNodesFinder(
                utHelper.curry(forEachDto, Dtos),
                DtoUtil.eachAxisDim,
                axisIndicesGetter
            )(Dtos[0]);
            expect(result).toEqual({
                nodes: [Dtos[0], Dtos[1], Dtos[2], Dtos[3]],
                records: makeRecords({x: [1, 2, 3, 5, 6], y: [0, 3, 9]})
            });
            done();
        });

        it('findLinkedNodes_emptySourceDto', function (done) {
            var Dtos = [
                {xAxisIndex: [1, 2], yAxisIndex: [0]},
                {xAxisIndex: [3], yAxisIndex: [3, 0]},
                {xAxisIndex: [6, 3], yAxisIndex: [9]},
                {xAxisIndex: [5, 3], yAxisIndex: []},
                {xAxisIndex: [8], yAxisIndex: [4]}
            ];
            var result = DtoUtil.createLinkedNodesFinder(
                utHelper.curry(forEachDto, Dtos),
                DtoUtil.eachAxisDim,
                axisIndicesGetter
            )();
            expect(result).toEqual({
                nodes: [],
                records: makeRecords({x: [], y: []})
            });
            done();
        });

    });

});