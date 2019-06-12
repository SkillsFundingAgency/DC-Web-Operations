/* global $ */

// Warn about using the kit in production
if (window.console && window.console.info) {
    window.console.info('GOV.UK Prototype Kit - do not use for production');
}

$(document).ready(function() {
    window.GOVUKFrontend.initAll();
});

$("#backlink").click(function(event) {
    event.preventDefault();
    history.back(1);
});

$("#typeFilter :checkbox").click(function() {
    $(".cards").hide();
    $("#typeFilter :checkbox:checked").each(function() {
        $("." + $(this).val()).show();
    });

    if (!$("#typeFilter :checkbox").is(':checked')) {
      $(".cards").show();
    }
});

$("#yearFilter :checkbox").click(function() {
    $(".year-groups").hide();
    $("#yearFilter :checkbox:checked").each(function() {
        $("." + $(this).val()).show();
    });

    if (!$("#yearFilter :checkbox").is(':checked')) {
      $(".year-groups").show();
    }
});


// to add a URL to the button when any radio button is select - ADD NEW PROVIDER PAGE //
$('#bulkOrSingle').click(function() {
   if($('#changed-name-1').is(':checked')) { location.href='new-provider-details'; }
	 else if ($('#changed-name-2').is(':checked')) { location.href='upload-provider-list'; }
});


$('#manageOrNew').click(function() {
   if($('#newNotification').is(':checked')) { location.href='notifications-type'; }
	 else if ($('#manageNotification').is(':checked')) { location.href='manage-notifications'; }
	 else if ($('#ilrNotification').is(':checked')) { location.href='notifications-layout-options'; }
});


// autocomlete option select function //

$(".autocomplete__menu").click(function() {
  $("#searchProviders").trigger("click");
});

// dashboard number count up //

// $('.counter').each(function() {
//   var $this = $(this),
//       countTo = $this.attr('data-count');
//
//   $({ countNum: $this.text()}).animate({
//     countNum: countTo
//   },
//
//   {
//     // delay: 2000,
//     duration: 1000,
//     easing:'linear',
//     step: function() {
//       $this.text(Math.floor(this.countNum));
//     },
//     complete: function() {
//       $this.text(this.countNum);
//       //alert('finished');
//     }
//
//   });
//
// });


// add assignments to provider //

$('#addAssignment').click(function(){

  if($('.assignment').is(':checked') && !$('.assignment-value').val() == '') {

    $('.govuk-error-message').addClass('hidden');

    $('.assigned').addClass('hidden');
    $('.assigned2').addClass('hidden');

    var datevalue = $(".assignment-value").val();
    var assignmnentEnd = $(".assignment-end").val();
    $('.assigned').removeClass('hidden');
    $('.start-date').empty().append( 'Start date: ' + datevalue );

    if($('.assignment-end').val() == '') {
      $('.end-date').empty().append( 'End date: ' + 'N/A' );
    } else if (!$('.assignment-end').val() == '') {
      $('.end-date').empty().append( 'End date: ' + assignmnentEnd );
    }

    $('#noActive').addClass('hidden');
    $('#buttonGroup').show();

    } else if ($('.assignment').is(':checked') && $('.assignment-value').val() == '') {
        $('.error1').removeClass('hidden');
      }


  if($('.assignment2').is(':checked') && !$('.assignment-value2').val() == '') {

    $('.assignment-value').removeAttr("disabled");

    $('.govuk-error-message').addClass('hidden');

    $('.assigned').addClass('hidden');
    $('.assigned2').addClass('hidden');

    var datevalue2 = $(".assignment-value2").val();
    var assignmnentEnd2 = $(".assignment-end2").val();
    $('.assigned2').removeClass('hidden');
    $('.start-date2').empty().append( 'Start date: ' + datevalue2 );

    if($('.assignment-end2').val() == '') {
      $('.end-date2').empty().append( 'End date: ' + 'N/A' );
    } else if (!$('.assignment-end2').val() == '') {
      $('.end-date2').empty().append( 'End date: ' + assignmnentEnd2 );
    }

    $('#noActive').addClass('hidden');
    $('#buttonGroup').show();

  } else if ($('.assignment2').is(':checked') && $('.assignment-value2').val() == '') {
      $('.error2').removeClass('hidden');
    }



  if($('.assignment3').is(':checked') && !$('.assignment-value3').val() == '') {

    $('.govuk-error-message').addClass('hidden');

    var datevalue3 = $(".assignment-value3").val();
    var assignmnentEnd3 = $(".assignment-end3").val();
    $('.assigned3').removeClass('hidden');
    $('.start-date3').empty().append( 'Start date: ' + datevalue3 );

    if($('.assignment-end3').val() == '') {
      $('.end-date3').empty().append( 'End date: ' + 'N/A' );
    } else if (!$('.assignment-end3').val() == '') {
      $('.end-date3').empty().append( 'End date: ' + assignmnentEnd3 );
    }

    $('#noActive').addClass('hidden');
    $('#buttonGroup').show();

  } else if ($('.assignment3').is(':checked') && $('.assignment-value3').val() == '') {
      $('.error3').removeClass('hidden');
    }


  if($('.assignment4').is(':checked') && !$('.assignment-value4').val() == '') {

    $('.govuk-error-message').addClass('hidden');

    var datevalue4 = $(".assignment-value4").val();
    var assignmnentEnd4 = $(".assignment-end4").val();
    $('.assigned4').removeClass('hidden');
    $('.start-date4').empty().append( 'Start date: ' + datevalue4 );

    if($('.assignment-end4').val() == '') {
      $('.end-date4').empty().append( 'End date: ' + 'N/A' );
    } else if (!$('.assignment-end4').val() == '') {
      $('.end-date4').empty().append( 'End date: ' + assignmnentEnd4 );
    }

    $('#noActive').addClass('hidden');
    $('#buttonGroup').show();

  } else if ($('.assignment4').is(':checked') && $('.assignment-value4').val() == '') {
      $('.error4').removeClass('hidden');
    }


    if($('.assignment5').is(':checked')) {

      $('.govuk-error-message').addClass('hidden');

      $('.assigned5').removeClass('hidden');

      $('#noActive').addClass('hidden');
      $('#buttonGroup').show();

    }


    if($('.active-assignments section').is(':visible')) {
      $('#addAssignment').text('Update assignments');
    }

  // $(".govuk-input").val("");

});



$('.remove').click(function(){
  $('.assigned').addClass('hidden');
  if(!$('.active-assignments section').is(':visible')) {
    $('#noActive').removeClass('hidden');
  }
  $('#buttonGroup').show();
});

$('.remove2').click(function(){
  $('.assigned2').addClass('hidden');
  if(!$('.active-assignments section').is(':visible')) {
    $('#noActive').removeClass('hidden');
  }
  $('#buttonGroup').show();
});

$('.remove3').click(function(){
  $('.assigned3').addClass('hidden');
  if(!$('.active-assignments section').is(':visible')) {
    $('#noActive').removeClass('hidden');
  }
  $('#buttonGroup').show();
});

$('.remove4').click(function(){
  $('.assigned4').addClass('hidden');
  if(!$('.active-assignments section').is(':visible')) {
    $('#noActive').removeClass('hidden');
  }
  $('#buttonGroup').show();
});

$('.remove5').click(function(){
  $('.assigned5').addClass('hidden');
  if(!$('.active-assignments section').is(':visible')) {
    $('#noActive').removeClass('hidden');
  }
  $('#buttonGroup').show();
});

$('#1 section p a').click(function(e){
  e.preventDefault();
});


$(document).ready(function () {
    if(window.location.href.indexOf("dashboard") > -1) {
       $('.main-nav #1').addClass('active');
    }

    if(window.location.href.indexOf("manage-collections") > -1) {
       $('.main-nav #2').addClass('active');
    }

    if(window.location.href.indexOf("notifications") > -1) {
       $('.main-nav #3').addClass('active');
    }

    if(window.location.href.indexOf("period-end") > -1) {
       $('.main-nav #4').addClass('active');
    }
});


$('#startRound3').click(function(e){
  $(this).text('Pause');
  $('.awaiting-1').text('in progress...');
  $('.time-started').show();
  setTimeout(function()
  {
    $('.awaiting-1').hide();
    $('.hide-complete-1').show();
    $('.awaiting-2').html('in progress...');
  }, 1000);
  setTimeout(function()
  {
    $('.awaiting-2').hide();
    $('.hide-complete-2').show();
    $('.awaiting-3').html('in progress...');
  }, 2000);
  setTimeout(function()
  {
    $('.awaiting-3').hide();
    $('.hide-complete-3').show();
    $('.awaiting-4').html('in progress...');

  }, 5000);
  setTimeout(function()
  {
    $('.awaiting-4').hide();
    $('.hide-complete-4').show();
    $('.period-end-complete').css('display', 'inline-block');
    $('#startRound3').hide();
  }, 7000);

  e.preventDefault();
});


$("#firstStep").change(function () {
    if ($(this).val() == "collection-assignments") {
        $("#grey1").addClass('show');
    } else {
        $("#grey1").removeClass('show');
    }
});

$('#closeBox').click(function(){
  $('#firstStep').val('empty').trigger('change');
  $('#secondStep').val('empty').trigger('change');
  $("#grey1").removeClass('show');
  $('.button-container').removeClass('show');
});

$("#secondStep").change(function () {
  $('.button-container').addClass('show');
});

$('#addMore').click(function(e){
  $("#grey2").addClass('show');
  $("#andOr").addClass('show');
  e.preventDefault();
});

$('#closeBox2').click(function(){
  $("#grey2").removeClass('show');
  $("#andOr").removeClass('show');
});

$('#allTab').click(function(){
  $('#first').attr('stroke-dasharray', '40, 100');
  $('#firstDonut').text('40');
  $('#jobText').text('All jobs');
});

$('.ilr').change(function(){
  $('#first').attr('stroke-dasharray', '22, 100');
  $('#firstDonut').text('22');
  $('#jobText').text('ILR');
});

$('.eas').change(function(){
  $('#first').attr('stroke-dasharray', '10, 100');
  $('#firstDonut').text('10');
  $('#jobText').text('EAS');
});

$('.esf').change(function(){
  $('#first').attr('stroke-dasharray', '8, 100');
  $('#firstDonut').text('8');
  $('#jobText').text('ESF');
});


$("#ILR").change(function() {
  if ($(this).is(':checked')) {
    $('#first').attr('stroke-dasharray', '22, 100');
    $('#firstDonut').text('22');
    $('#jobText').text('ILR jobs');
  } else {
    $('#first').attr('stroke-dasharray', '40, 100');
    $('#firstDonut').text('40');
    $('#jobText').text('All jobs');
  }
});

$("#EAS").change(function() {
  if ($(this).is(':checked')) {
    $('#first').attr('stroke-dasharray', '10, 100');
    $('#firstDonut').text('10');
    $('#jobText').text('EAS jobs');
  } else {
    $('#first').attr('stroke-dasharray', '40, 100');
    $('#firstDonut').text('40');
    $('#jobText').text('All jobs');
  }
});

$("#ESF").change(function() {
  if ($(this).is(':checked')) {
    $('#first').attr('stroke-dasharray', '8, 100');
    $('#firstDonut').text('8');
    $('#jobText').text('ESF jobs');
  } else {
    $('#first').attr('stroke-dasharray', '40, 100');
    $('#firstDonut').text('40');
    $('#jobText').text('All jobs');
  }
});


$("#typeFilter :checkbox").click(function() {
    $("#submissions div").hide();
    $("#typeFilter :checkbox:checked").each(function() {
        $("." + $(this).val()).show();
    });

    if (!$("#typeFilter :checkbox").is(':checked')) {
      $("#submissions div").show();
    }
});

$('#clearFromList').click(function(e){
  $('.card-1').hide();

  e.preventDefault();
});

$('.pause').click(function(e){
  $('#Status').text('Paused');
  $(this).text('Continue');
  e.preventDefault();
});

$('.cancel-process').click(function(e){
  $('#Status').text('Cancelled');
  $(this).text('Submit again');
  $('.pause').hide();

  e.preventDefault();
});


$('#intExt').click(function() {
   if($('.open-to-providers').is(':checked')) { location.href='review-ILR-changes-ext'; }
	 else if ($('.open-internally').is(':checked')) { location.href='review-ILR-changes'; }
});

$("#typeFilter :checkbox").click(function() {
    $("#submissions tr").hide();
    $("#typeFilter :checkbox:checked").each(function() {
        $("." + $(this).val()).show();
    });

    if (!$("#typeFilter :checkbox").is(':checked')) {
      $("#submissions tr").show();
    }
});

$("#yearFilter :checkbox").click(function() {
    $(".year-groups").hide();
    $("#yearFilter :checkbox:checked").each(function() {
        $("." + $(this).val()).show();
    });

    if (!$("#yearFilter :checkbox").is(':checked')) {
      $(".year-groups").show();
    }
});


$("#first-radio").change(function() {
  $('.right-side-image img').attr('src','/public/images/01-first.png');
});
$("#second-radio").change(function() {
  $('.right-side-image img').attr('src','/public/images/02-second.png');
});
$("#third-radio").change(function() {
  $('.right-side-image img').attr('src','/public/images/03-third.png');
});
$("#forth-radio").change(function() {
  $('.right-side-image img').attr('src','/public/images/right-hand.png');
});
