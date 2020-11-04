//unhide submit unpublish reports
function showsubmit() {
    document.getElementById("submit").classList.add('govuk-button');
    document.getElementById("submit").hidden = false;
    document.getElementById("confirmtext").hidden = false;
}
//enable continue button to be selected
function enablecontinue() {
    if (
        document.getElementById("PublishedPeriods").value == "selectone") {
        document.getElementById("continue").disabled = true;
        document.getElementById("submit").disabled = true;
    }
    else {
        document.getElementById("continue").disabled = false;
        document.getElementById("submit").disabled = false;

    }
};      

function submit() {
    this.form.submit();
}