import { getPathNameBySubPathId, isNextItemSubPath, getJobContinuationStatus } from '/assets/js/periodEnd/periodEndUtil.js';
import { jobContinuation, jobStatus } from '/assets/js/periodEnd/state.js';

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

            // Assert
            expect(result).toBe(true);
        });

        test('getPathNameBySubPathId returns false when next item is not subpath ', () => {
            //Arrange
            const pathItems = [{ pathItemId: 1 }, { pathItemId: 2 }, { pathItemId: 3, subPaths: [1] }];

            // Act
            const result = isNextItemSubPath(pathItems, 0);

            // Assert
            expect(result).toBe(false);
        });
    });

    describe('getJobContinuationStatus', () => {
        describe('with no job returns nothingRunning', () => {
            test('null jobs', () => {
                 // Act
                const result = getJobContinuationStatus(null);

                // Assert
                expect(result).toBe(jobContinuation.nothingRunning);
            });

            test('empty jobs', () => {
                // Act
                const result = getJobContinuationStatus([]);

                // Assert
                expect(result).toBe(jobContinuation.nothingRunning);
            });

            test('undefined', () => {
                // Act
                const result = getJobContinuationStatus();

                // Assert
                expect(result).toBe(jobContinuation.nothingRunning);
            });
        });

        describe('with failed jobs returns someFailed', () => {
            test('failed and completed job', () => {
                //arrange
                const pathItems = [{ status: jobStatus.completed }, { status: jobStatus.failed }, ]

                // Act
                const result = getJobContinuationStatus(pathItems);

                // Assert
                expect(result).toBe(jobContinuation.someFailed);
            });

            test('retry failed and completed job', () => {
                //arrange
                const pathItems = [{ status: jobStatus.failedRetry }, { status: jobStatus.failed }]

                // Act
                const result = getJobContinuationStatus(pathItems);

                // Assert
                expect(result).toBe(jobContinuation.someFailed);
            });
        });

        test('all completed returns allCompleted', () => {
            //arrange
            const pathItems = [{ status: jobStatus.completed }, { status: jobStatus.completed }]

            // Act
            const result = getJobContinuationStatus(pathItems);

            // Assert
            expect(result).toBe(jobContinuation.allCompleted);
        });

        describe('returns is running if job still running irrespective of other results', () => {

            test('when job processing', () => {
                //arrange
                const pathItems = [{ status: jobStatus.completed }, { status: jobStatus.failed }, { status: jobStatus.processing }]

                // Act
                const result = getJobContinuationStatus(pathItems);

                // Assert
                expect(result).toBe(jobContinuation.running);
            });

            test('when job ready', () => {
                //arrange
                const pathItems = [{ status: jobStatus.completed }, { status: jobStatus.failed }, { status: jobStatus.ready }]

                // Act
                const result = getJobContinuationStatus(pathItems);

                // Assert
                expect(result).toBe(jobContinuation.running);
            });

            test('when job movedForProcessing', () => {
                //arrange
                const pathItems = [{ status: jobStatus.completed }, { status: jobStatus.failed }, { status: jobStatus.movedForProcessing }]

                // Act
                const result = getJobContinuationStatus(pathItems);

                // Assert
                expect(result).toBe(jobContinuation.running);
            });

            test('when job paused', () => {
                //arrange
                const pathItems = [{ status: jobStatus.completed }, { status: jobStatus.failed }, { status: jobStatus.paused }]

                // Act
                const result = getJobContinuationStatus(pathItems);

                // Assert
                expect(result).toBe(jobContinuation.running);
            });

            test('when job waiting', () => {
                //arrange
                const pathItems = [{ status: jobStatus.completed }, { status: jobStatus.failed }, { status: jobStatus.waiting }]

                // Act
                const result = getJobContinuationStatus(pathItems);

                // Assert
                expect(result).toBe(jobContinuation.running);
            });
        });
    });

});

