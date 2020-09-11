import { getPathNameBySubPathId, isNextItemSubPath } from '/assets/js/periodEnd/periodEndUtil.js';

describe('period end util', () => {

    describe('getPathNameBySubPathId', () => {

        const stateModel = {
            "paths": [{ pathItems: [{ name: 'One', subPaths: [1] }, { name: 'Two', subPaths: [2] }, { name: 'Three', subPaths: [3] }] }]
        };

        test('getPathNameBySubPathId gets correct name by id', () => {
            //Act
            const result = getPathNameBySubPathId(stateModel, 2);

            // Assert
            expect(result).toBe('Two');
        });

        test('getPathNameBySubPathId defaults to path0 when no match', () => {
            //Act
            const result = getPathNameBySubPathId(stateModel, 5);

            // Assert
            expect(result).toBe('Path0');
        });
    });

    describe('getPathNameBySubPathId', () => {

        const pathItems = [{ pathItemId: 1 }, { pathItemId: 2 }, { pathItemId: 3, subPaths: [1] }];

        test('getPathNameBySubPathId returns true when next item is subpath', () => {
            // Act
            const result = isNextItemSubPath(pathItems, 1);

            // Arrange
            expect(result).toBe(true);
        });

        test('getPathNameBySubPathId returns false when next item is not subpath ', () => {
            //Arrange
            const pathItems = [{ pathItemId: 1 }, { pathItemId: 2 }, { pathItemId: 3, subPaths: [1] }];

            // Act
            const result = isNextItemSubPath(pathItems, 0);

            // Arrange
            expect(result).toBe(false);
        });
    });

});

