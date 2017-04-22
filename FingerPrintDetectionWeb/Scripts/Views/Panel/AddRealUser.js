$(document).ready(function () {


    $("#addUserForm").validate({
        errorClass: "my-error-class",
        validClass: "my-valid-class",
        rules: {
            FirstName: {
                required: true
            },
            LastName: {
                required: true
            },
            LogicalUserId: {
                required: true
            }
        },
        messages: {

            FirstName: {
                required: "نام کاربر را وارد کنید"
            },
            LastName: {
                required: "نام خانوادگی کاربر را وارد کنید"
            },
            LogicalUserId: { required: " کاربر منطقی را مشخص کتید" }

        }

    });
    $('#addUserForm').bind('submit', function (e) {
        e.preventDefault(); //prevent the default action
        var form = $(this);
        var url = form.data('url');
        form.validate();
        var isValid = form.valid();
        if (!isValid) {
            return;
        }

        $('#globalLoadingModal').modal('show');
        $.ajax({
            type: 'POST',
            url: url,
            async: true,
            data: form.serialize(),
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
            },
            error: function (errors) {
                $('#globalLoadingModal').modal('hide');
            }
        });
    });
    $("#birthdayTimepicker").pDatepicker({
        format: "YYYY/MM/DD"
    });
    $("#datepicker1btn").click(function(event) {
        event.preventDefault();
        $("#birthdayTimepicker").focus();
    });
});