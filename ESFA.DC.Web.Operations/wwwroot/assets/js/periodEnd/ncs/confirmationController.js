import JobController from '/assets/js/periodEnd/ncs/jobController.js';

class confirmationController {

    initialiseConfirmation(periodClosed, collectionClosedEmailSent, periodEndFinished) {
        const finished = periodEndFinished === "False" ? false : true;
        const closed = periodClosed === "False" ? false : true;
        const emailSent = collectionClosedEmailSent === "False" ? false : true;

        const jobController = new JobController();

        if (finished === true) {
            jobController.setContinueButtonState(true);
            return;
        }
        
        jobController.setCollectionClosedEmailButtonState(closed && !emailSent);
        jobController.setContinueButtonState(closed && emailSent);
    }

}

export default confirmationController;