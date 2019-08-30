"use strict";

let confirmPauseContainer = document.getElementById("confirm-pause");
let confirmedContainer = document.getElementById("pause-confirmation");

let pauseButton = document.getElementById("pause-all-jobs");
let confirmPauseButton = document.getElementById("pauseReferenceData");
let cancelButton = document.getElementById("cancel-pause");

pauseButton.addEventListener("click", showConfirmation);
confirmPauseButton.addEventListener("click", confirmPause);
cancelButton.addEventListener("click", cancelPause);

function initialiseConfirmation(referenceDataJobs) {
    let paused = true;
    let jobs = JSON.parse(referenceDataJobs);

    jobs.forEach(function(job) {
        if (job.status !== "Paused") {
            paused = false;
        }
    });

    if (paused === true) {
        pauseButton.style.display = "none";
        confirmedContainer.style.display = "block";
    }
}

function showConfirmation() {
    pauseButton.style.display = "none";
    confirmPauseContainer.style.display = "block";
}

function confirmPause() {
    confirmPauseContainer.style.display = "none";
    confirmedContainer.style.display = "block";
}

function cancelPause() {
    confirmPauseContainer.style.display = "none";
    pauseButton.style.display = "block";
}