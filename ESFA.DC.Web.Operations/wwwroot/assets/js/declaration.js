$(function () {

    $("#confirm").change(function() {
        if (this.checked) {
            $('#errorSummary').hide();
            $('#errorMessage').removeClass("govuk-form-group--error");
        } else {
            $('#errorSummary').show();
            $('#errorMessage').addClass("govuk-form-group--error");
        }
        
    
    });

        $("#submit").click(function (e) {

        if ($('#confirm').is(':checked') == false) {
            $('#errorSummary').show();
            $('#errorMessage').addClass("govuk-form-group--error");
            e.preventDefault();
        }
    });
});

   