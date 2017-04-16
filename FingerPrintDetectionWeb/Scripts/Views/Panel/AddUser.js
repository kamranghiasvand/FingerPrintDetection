$(document).ready(function () {


    $("#addUserForm").validate({
        errorClass: "my-error-class",
        validClass: "my-valid-class",
        rules: {
            FirstName: {
                required: true
            },
            LastName: {
                required:true
            },
            PlanId: {
                required: true
            }
        },
        messages: {
          
            FirstName: {
                required: "نام کاربر را وارد کنید"
            },
            LastName: {
                required:"نام خانوادگی کاربر را وارد کنید"
            },
            PlanId: { required: " پلن پخش صدا را مشخص کتید" }

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
        var formData = new FormData($(this)[0]);
        $('#globalLoadingModal').modal('show');
        $.ajax({
            type: 'POST',
            url: url,
            async: false,
            data: formData,
            contentType: false,
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

    $('#soundFile').change(function (e) {
        var target = e.currentTarget;
        var file = target.files[0];
        if (target.files && file) {
            var reader = new FileReader();
            reader.onload = function (e) {
                var player = document.getElementById('player');
                var sourceMp3 = document.getElementById('sourceMp3');

                sourceMp3.src = e.target.result;
                player.load(); //just start buffering (preload)
                player.play(); //start playing
            }
            reader.readAsDataURL(file);
        }

    });
});