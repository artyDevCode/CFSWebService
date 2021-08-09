$(function () {
    $.datepicker.setDefaults($.datepicker.regional[""]);
    $("#DateOfBirth").datepicker({
        changeMonth: true,
        changeYear: true
    });

    $("#StartDate, #EndDate").datepicker({
        changeMonth: true,
        //changeYear: true,
        //showOtherMonths: true,
        //selectOtherMonths: true,
        //dateFormat: 'DD, d MM, yy',
        altField: '#date_due',
        altFormat: 'yy-mm-dd',
        firstDay: 1, // rows starts on Monday 11/7/2014
        dateFormat: "dd-mm-yy",
        timeFormat: "hh:mm tt"
        //numberOfMonths: 3
    }).change(function () {
        if (this.id == 'DN_Start_Date') {

            if ((Date.parse(startDate) >= Date.parse(endDate))) {
                document.getElementById("DN_End_Date").value = startDate;
            }
        }
    });
});

