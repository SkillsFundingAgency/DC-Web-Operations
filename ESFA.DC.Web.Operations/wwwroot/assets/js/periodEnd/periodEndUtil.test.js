import { getPathNameBySubPathId, isNextItemSubPath, getJobContinuationStatus, canContinue, isAutoCompleted } from '/assets/js/periodEnd/periodEndUtil.js';
import { jobContinuation, jobStatus } from '/assets/js/periodEnd/state.js';

describe('period end util', () => {

    describe('getPathNameBySubPathId', () => {

        const stateModel = {
            "paths": [{ pathItems: [{ name: 'One', subPaths: [1] }, { name: 'Two', subPaths: [2] }, { name: 'Three', subPaths: [3] }] }]
        };

        test('returns correct name by id', () => {
            //Act
            const result = getPathNameBySubPathId(stateModel, 2);

            // Assert
            expect(result).toBe('Two');
        });

        test('returns Path0 when no match', () => {
            //Act
            const result = getPathNameBySubPathId(stateModel, 5);

            // Assert
            expect(result).toBe('Path0');
        });
    });

    describe('isNextItemSubPath', () => {

        const pathItems = [{ pathItemId: 1 }, { pathItemId: 2 }, { pathItemId: 3, subPaths: [1] }];

        test('returns true when next item is subpath', () => {
            // Act
            const result = isNextItemSubPath(pathItems, 1);

            // Assert
            expect(result).toBe(true);
        });

        test('returns false when next item is not subpath ', () => {
            //Arrange
            const pathItems = [{ pathItemId: 1 }, { pathItemId: 2 }, { pathItemId: 3, subPaths: [1] }];

            // Act
            const result = isNextItemSubPath(pathItems, 0);

            // Assert
            expect(result).toBe(false);
        });
    });

    describe('getJobContinuationStatus', () => {
        describe('returns nothingRunning', () => {
            test('when jobs are null', () => {
                 // Act
                const result = getJobContinuationStatus(null);

                // Assert
                expect(result).toBe(jobContinuation.nothingRunning);
            });

            test('when jobs are empty', () => {
                // Act
                const result = getJobContinuationStatus([]);

                // Assert
                expect(result).toBe(jobContinuation.nothingRunning);
            });

            test('when jobs are undefined', () => {
                // Act
                const result = getJobContinuationStatus();

                // Assert
                expect(result).toBe(jobContinuation.nothingRunning);
            });
        });

        describe('returns someFailed', () => {
            test('when has failed and completed job', () => {
                //arrange
                const pathItems = [{ status: jobStatus.completed }, { status: jobStatus.failed }, ]

                // Act
                const result = getJobContinuationStatus(pathItems);

                // Assert
                expect(result).toBe(jobContinuation.someFailed);
            });

            test('when has retry failed and completed job', () => {
                //arrange
                const pathItems = [{ status: jobStatus.failedRetry }, { status: jobStatus.failed }]

                // Act
                const result = getJobContinuationStatus(pathItems);

                // Assert
                expect(result).toBe(jobContinuation.someFailed);
            });
        });

        test('returns allCompleted', () => {
            //arrange
            const pathItems = [{ status: jobStatus.completed }, { status: jobStatus.completed }]

            // Act
            const result = getJobContinuationStatus(pathItems);

            // Assert
            expect(result).toBe(jobContinuation.allCompleted);
        });

        describe('returns is running', () => {

            test('when at least one job processing', () => {
                //arrange
                const pathItems = [{ status: jobStatus.completed }, { status: jobStatus.failed }, { status: jobStatus.processing }]

                // Act
                const result = getJobContinuationStatus(pathItems);

                // Assert
                expect(result).toBe(jobContinuation.running);
            });

            test('when at least one job ready', () => {
                //arrange
                const pathItems = [{ status: jobStatus.completed }, { status: jobStatus.failed }, { status: jobStatus.ready }]

                // Act
                const result = getJobContinuationStatus(pathItems);

                // Assert
                expect(result).toBe(jobContinuation.running);
            });

            test('when at least one job movedForProcessing', () => {
                //arrange
                const pathItems = [{ status: jobStatus.completed }, { status: jobStatus.failed }, { status: jobStatus.movedForProcessing }]

                // Act
                const result = getJobContinuationStatus(pathItems);

                // Assert
                expect(result).toBe(jobContinuation.running);
            });

            test('when at least job paused', () => {
                //arrange
                const pathItems = [{ status: jobStatus.completed }, { status: jobStatus.failed }, { status: jobStatus.paused }]

                // Act
                const result = getJobContinuationStatus(pathItems);

                // Assert
                expect(result).toBe(jobContinuation.running);
            });

            test('when at least job waiting', () => {
                //arrange
                const pathItems = [{ status: jobStatus.completed }, { status: jobStatus.failed }, { status: jobStatus.waiting }]

                // Act
                const result = getJobContinuationStatus(pathItems);

                // Assert
                expect(result).toBe(jobContinuation.running);
            });
        });
    });

    describe('canContinue', () => {
        test('when isBusy returns false', () => {
            //arrange
            const pathItem = {};

            // Act
            const result = canContinue(pathItem, true);

            // Assert
            expect(result).toBe(false);
        });

        test('when all jobs completed returns true ', () => {
            //arrange
            const pathItem = { pathItemJobs: [{ status: jobStatus.completed }, { status: jobStatus.completed }]};

            // Act
            const result = canContinue(pathItem, false);

            // Assert
            expect(result).toBe(true);
        });

        test('when has no jobs returns true', () => {
            //arrange
            const pathItem = { hasJobs: false };

            // Act
            const result = canContinue(pathItem, false);

            // Assert
            expect(result).toBe(true);
        });

        test('when has no jobs but is busy returns false', () => {
            //arrange
            const pathItem = { hasJobs: false };

            // Act
            const result = canContinue(pathItem, true);

            // Assert
            expect(result).toBe(false);
        });

    });

    describe('isAutoCompleted', () => {

        test('when no jobs and pathItem in past should be true', () => {
            //arrange
            const pathItem = { ordinal:1 };
            const path = { position: 3 };

            // Act
            const result = isAutoCompleted(pathItem, path);

            // Assert
            expect(result).toBe(true);
        });

        test('when has jobs and pathItem in past should be false', () => {
            //arrange
            const pathItem = { ordinal: 1, pathItemJobs: [{}, {}] };
            const path = { position: 3 };

            // Act
            const result = isAutoCompleted(pathItem, path);

            // Assert
            expect(result).toBe(false);
        });

        test('when no jobs and pathItem is last in list should be true', () => {
            //arrange
            const pathItem = { ordinal: 0 };
            const path = { position: 1, pathItems: [{}] };

            // Act
            const result = isAutoCompleted(pathItem, path);

            // Assert
            expect(result).toBe(true);
        });


        test('when has jobs and pathItem is last should be false', () => {
            //arrange
            const pathItem = { ordinal: 0, pathItemJobs: [{}, {}] };
            const path = { position: 1, pathItems: [{}] };

            // Act
            const result = isAutoCompleted(pathItem, path);

            // Assert
            expect(result).toBe(false);
        });

    });

});

