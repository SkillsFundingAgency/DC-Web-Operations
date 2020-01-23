export let disabledProceedButtons = new Array();

export let jobStatus = {
    ready: 1,
    movedForProcessing: 2,
    processing: 3,
    completed: 4,
    failedRetry: 5,
    failed: 6,
    paused: 7,
    waiting: 8
};

export let jobContinuation = {
    allCompleted: 1,
    someFailed: 2,
    running: 3,
    nothingRunning: 4
};