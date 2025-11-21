describe('Component', function() {

    var utHelper = window.utHelper;

    var testCase = utHelper.prepare(['echarts/Dto/Component']);

    describe('topologicalTravel', function () {

        testCase('topologicalTravel_base', function (ComponentDto) {
            ComponentDto.extend({type: 'm1', dependencies: ['a1', 'a2']});
            ComponentDto.extend({type: 'a1'});
            ComponentDto.extend({type: 'a2'});
            var result = [];
            var allList = ComponentDto.getAllClassMainTypes();
            ComponentDto.topologicalTravel(['m1', 'a1', 'a2'], allList, function (componentType, dependencies) {
                result.push([componentType, dependencies]);
            });
            expect(result).toEqual([['a2', []], ['a1', []], ['m1', ['a1', 'a2']]]);
        });

        testCase('topologicalTravel_a1IsAbsent', function (ComponentDto) {
            ComponentDto.extend({type: 'm1', dependencies: ['a1', 'a2']});
            ComponentDto.extend({type: 'a2'});
            var allList = ComponentDto.getAllClassMainTypes();
            var result = [];
            ComponentDto.topologicalTravel(['m1', 'a2'], allList, function (componentType, dependencies) {
                result.push([componentType, dependencies]);
            });
            expect(result).toEqual([['a2', []], ['m1', ['a1', 'a2']]]);
        });

        testCase('topologicalTravel_empty', function (ComponentDto) {
            ComponentDto.extend({type: 'm1', dependencies: ['a1', 'a2']});
            ComponentDto.extend({type: 'a1'});
            ComponentDto.extend({type: 'a2'});
            var allList = ComponentDto.getAllClassMainTypes();
            var result = [];
            ComponentDto.topologicalTravel([], allList, function (componentType, dependencies) {
                result.push([componentType, dependencies]);
            });
            expect(result).toEqual([]);
        });

        testCase('topologicalTravel_isolate', function (ComponentDto) {
            ComponentDto.extend({type: 'a2'});
            ComponentDto.extend({type: 'a1'});
            ComponentDto.extend({type: 'm1', dependencies: ['a2']});
            var allList = ComponentDto.getAllClassMainTypes();
            var result = [];
            ComponentDto.topologicalTravel(['a1', 'a2', 'm1'], allList, function (componentType, dependencies) {
                result.push([componentType, dependencies]);
            });
            expect(result).toEqual([['a1', []], ['a2', []], ['m1', ['a2']]]);
        });

        testCase('topologicalTravel_diamond', function (ComponentDto) {
            ComponentDto.extend({type: 'a1', dependencies: []});
            ComponentDto.extend({type: 'a2', dependencies: ['a1']});
            ComponentDto.extend({type: 'a3', dependencies: ['a1']});
            ComponentDto.extend({type: 'm1', dependencies: ['a2', 'a3']});
            var allList = ComponentDto.getAllClassMainTypes();
            var result = [];
            ComponentDto.topologicalTravel(['m1', 'a1', 'a2', 'a3'], allList, function (componentType, dependencies) {
                result.push([componentType, dependencies]);
            });
            expect(result).toEqual([['a1', []], ['a3', ['a1']], ['a2', ['a1']], ['m1', ['a2', 'a3']]]);
        });

        testCase('topologicalTravel_loop', function (ComponentDto) {
            ComponentDto.extend({type: 'm1', dependencies: ['a1', 'a2']});
            ComponentDto.extend({type: 'm2', dependencies: ['m1', 'a2']});
            ComponentDto.extend({type: 'a1', dependencies: ['m2', 'a2', 'a3']});
            ComponentDto.extend({type: 'a2'});
            ComponentDto.extend({type: 'a3'});
            var allList = ComponentDto.getAllClassMainTypes();
            expect(function () {
                ComponentDto.topologicalTravel(['m1', 'm2', 'a1'], allList);
            }).toThrowError(/Circl/);
        });

        testCase('topologicalTravel_multipleEchartsInstance', function (ComponentDto) {
            ComponentDto.extend({type: 'm1', dependencies: ['a1', 'a2']});
            ComponentDto.extend({type: 'a1'});
            ComponentDto.extend({type: 'a2'});
            var allList = ComponentDto.getAllClassMainTypes();
            var result = [];
            ComponentDto.topologicalTravel(['m1', 'a1', 'a2'], allList, function (componentType, dependencies) {
                result.push([componentType, dependencies]);
            });
            expect(result).toEqual([['a2', []], ['a1', []], ['m1', ['a1', 'a2']]]);

            result = [];
            ComponentDto.extend({type: 'm2', dependencies: ['a1', 'm1']});
            var allList = ComponentDto.getAllClassMainTypes();
            ComponentDto.topologicalTravel(['m2', 'm1', 'a1', 'a2'], allList, function (componentType, dependencies) {
                result.push([componentType, dependencies]);
            });
            expect(result).toEqual([['a2', []], ['a1', []], ['m1', ['a1', 'a2']], ['m2', ['a1', 'm1']]]);
        });

        testCase('topologicalTravel_missingSomeNodeButHasDependencies', function (ComponentDto) {
            ComponentDto.extend({type: 'm1', dependencies: ['a1', 'a2']});
            ComponentDto.extend({type: 'a2', dependencies: ['a3']});
            ComponentDto.extend({type: 'a3'});
            ComponentDto.extend({type: 'a4'});
            var result = [];
            var allList = ComponentDto.getAllClassMainTypes();
            ComponentDto.topologicalTravel(['a3', 'm1'], allList, function (componentType, dependencies) {
                result.push([componentType, dependencies]);
            });
            expect(result).toEqual([['a3', []], ['a2', ['a3']], ['m1', ['a1', 'a2']]]);
            var result = [];
            var allList = ComponentDto.getAllClassMainTypes();
            ComponentDto.topologicalTravel(['m1', 'a3'], allList, function (componentType, dependencies) {
                result.push([componentType, dependencies]);
            });
            expect(result).toEqual([['a3', []], ['a2', ['a3']], ['m1', ['a1', 'a2']]]);
        });

        testCase('topologicalTravel_subType', function (ComponentDto) {
            ComponentDto.extend({type: 'm1', dependencies: ['a1', 'a2']});
            ComponentDto.extend({type: 'a1.aaa', dependencies: ['a2']});
            ComponentDto.extend({type: 'a1.bbb', dependencies: ['a3', 'a4']});
            ComponentDto.extend({type: 'a2'});
            ComponentDto.extend({type: 'a3'});
            ComponentDto.extend({type: 'a4'});
            var result = [];
            var allList = ComponentDto.getAllClassMainTypes();
            ComponentDto.topologicalTravel(['m1', 'a1', 'a2', 'a4'], allList, function (componentType, dependencies) {
                result.push([componentType, dependencies]);
            });
            expect(result).toEqual([['a4', []], ['a2',[]], ['a1', ['a2','a3','a4']], ['m1', ['a1', 'a2']]]);
        });
    });

});