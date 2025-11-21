// Test the core element functionality
describe('Core element tests', function() {
	it ('should transition Dto properties', function() {
		var element = new Chart.Element({
			_Dto: {
				numberProp: 0,
				numberProp2: 100,
				_underscoreProp: 0,
				stringProp: 'abc',
				objectProp: {
					myObject: true
				},
				colorProp: 'rgb(0, 0, 0)'
			}
		});

		// First transition clones Dto into view
		element.transition(0.25);
		expect(element._view).toEqual(element._Dto);
		expect(element._start).toEqual(element._Dto); // also cloned

		expect(element._view.objectProp).toBe(element._Dto.objectProp); // not cloned
		expect(element._start.objectProp).toEqual(element._Dto.objectProp); // not cloned

		element._Dto.numberProp = 100;
		element._Dto.numberProp2 = 250;
		element._Dto._underscoreProp = 200;
		element._Dto.stringProp = 'def'
		element._Dto.newStringProp = 'newString';
		element._Dto.colorProp = 'rgb(255, 255, 0)'

		element.transition(0.25);
		expect(element._view).toEqual({
			numberProp: 25,
			numberProp2: 137.5,
			_underscoreProp: 0, // underscore props are not transition to a new value
			stringProp: 'def',
			newStringProp: 'newString',
			objectProp: {
				myObject: true
			},
			colorProp: 'rgb(64, 64, 0)',
		});
	});
});