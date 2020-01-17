import JobController from '/assets/js/periodEnd/jobController.js';

class confirmationController {

    constructor() {
        const classScope = this;
        this._confirmPauseContainer = document.getElementById("confirm-pause");
        this._confirmedContainer = document.getElementById("pause-confirmation");
        this._pauseButton = document.getElementById("pause-all-jobs");
        this._confirmPauseButton = document.getElementById("pauseReferenceData");
        this._cancelButton = document.getElementById("cancel-pause");

        this._pauseButton.addEventListener("click", classScope.showConfirmation.bind(classScope));
        this._confirmPauseButton.addEventListener("click", classScope.confirmPause.bind(classScope));
        this._cancelButton.addEventListener("click", classScope.cancelPause.bind(classScope));
    }

    initialiseConfirmation(referenceDataJobs, periodClosed, collectionClosedEmailSent, periodEndFinished) {
        const finished = periodEndFinished === "False" ? false : true;

        const jobController = new JobController();

        if (finished === true) {
            jobController.setContinueButtonState(true);
            return;
        }

        let paused = true;
        const jobs = JSON.parse(referenceDataJobs);

        jobs.forEach(function(job) {
            if (job.status !== "Paused") {
                paused = false;
            }
        });
        
        if (paused === true) {
            const closed = periodClosed === "False" ? false : true;
            const emailSent = collectionClosedEmailSent === "False" ? false : true;

            jobController.setCollectionClosedEmailButtonState(closed && !emailSent);
            jobController.setContinueButtonState(closed && emailSent);

            this._pauseButton.style.display = "none";
            this._confirmedContainer.style.display = "block";
        } else {
            jobController.setPauseRefJobsButtonState(true);
        }
    }

    showConfirmation() {
        this._pauseButton.style.display = "none";
        this._confirmPauseContainer.style.display = "block";
    }

    confirmPause() {
        this._confirmPauseContainer.style.display = "none";
        this._confirmedContainer.style.display = "block";
    }

    cancelPause() {
        this._confirmPauseContainer.style.display = "none";
        this._pauseButton.style.display = "block";
    }
}

export default confirmationController;