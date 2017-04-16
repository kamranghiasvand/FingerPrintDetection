$(document).ready(function () {


    $("#addPlanForm").validate({
        errorClass: "my-error-class",
        validClass: "my-valid-class",
        rules: {
            Name: {
                required: true
            },
            RepeatNumber: {
                required: true
            },
            MaxUserCount: {
                required: true
            }
        },
        messages: {

            Name: {
                required: "نام پلن را وارد کنید"
            },
            RepeatNumber: {
                required: "تعداد تکرار را مشخص کنید"
            },
            MaxUserCount: { required: "حداکثر  تعداد کاربران را مشخص کنید" }

        }

    });
    $('#addPlanForm').bind('submit', function (e) {
        e.preventDefault();
        var form = $(this);
        var url = form.data('url');
        form.validate();
        var isValid = form.valid();
        if (!isValid) {
            return;
        }
        var formData = form.serialize();
        $('#globalLoadingModal').modal('show');
        $.ajax({
            type: 'POST',
            url: url,
            async: false,
            data: formData,
            processData: false,
            success: function (data) {
                $('#globalLoadingModal').modal('hide');
                if (data.status === 'success') {
                    form.find('.validation-summary-errors').addClass('hidden').empty();
                    window.location.href = data.address;
                } else if (data.status === 'fail') {
                    form.find('.validation-summary-errors').empty().append('<ul class="validation-summary-errors-list"></ul>').removeClass('hidden');
                    for (var cnt = 0; cnt < data.errors.length; cnt++) {
                        form.find('.validation-summary-errors-list').append('<li>' + data.errors[cnt] + '</li>');
                    }

                }
                // $("#answers").html(response);
            },
            error: function (errors) {

                $('#globalLoadingModal').modal('hide');
            }
        });
    });
    var date = new Date($.now());
    var time = date.getHours() + ':' + date.getMinutes();
    $('#schedulerDailyStartTimepicker').timepicker({
        showInputs: false,
        showMeridian: false,
        defaultTime: time,
        minuteStep: 1

    });

    time = date.getHours() + 1 + ':' + date.getMinutes();
    $('#schedulerDailyEndTimepicker').timepicker({
        showInputs: false,
        showMeridian: false,
        defaultTime: time,
        minuteStep: 1
    });
});