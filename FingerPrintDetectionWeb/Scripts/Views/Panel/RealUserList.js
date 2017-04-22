$(document).ready(function () {
    /*.......datatable..........*/
    var search = '';
    var datatableUrl = $("#UserdataTables").data('url');
    $.fn.dataTable.ext.buttons.reload = {
        text: "بارگذاری مجدد",
        action: function (e, dt) {
            dt.ajax.reload();
            $('#UserdataTables_filter input').val(search);
        }
    };
    var settings = {
        "language": {
            "processing": '<i style="z-index:1000" class="fa fa-spinner fa-pulse fa-3x fa-fw"></i><span class="sr-only">Loading...</span>'
        },
        serverSide: true,
        processing: true,
        responsive: true,
        bSort: true,
        async: true,
        deferRender: true,
        rowId: 'Id',
        select: true,
        dom: 'Bfrtip',
        ajax: {
            url: datatableUrl,
            data: function (d) {
                d.customSearch = search;
            }
        },
        buttons: [
           'reload'
        ],
        "columns": [
            {
                name: "FirstName",
                title: "نام",
                className: "all",
                visible: true,
                sortable: true,
                searchable: true,
                render: function (data, type, row) {
                    return row.FirstName;
                }
            },
            {
                name: "LastName",
                title: "نام خانوادگی",
                className: "all",
                visible: true,
                sortable: true,
                searchable: true,
                render: function (data, type, row) {
                    return row.LastName;
                }
            },
            {
                name: 'Fingers',
                title: "انگشتها",
                className: "all",
                visible: true,
                sortable: false,
                searchable: false,
                render: function (data, type, row) {

                    var res = '';
                    for (var i = 0; i < row.Fingers.length; ++i) {
                        res += '<div class="btn btn-xs '
                            + (row.Fingers[i] ? 'btn-success' : 'btn-default') +
                            ' fingerbtn" data-fingerid="' + i + '" data-userid="'+row.Id+'">انگشت ' + i + '</div>';
                    }
                    return res;
                }
            },
            {
                name: 'Action',
                title: 'عملیات',
                className: 'all',
                data: null,
                sortable: false,
                searchable: false,
                render: function (data, type, row) {

                    return '<div class="btn btn-danger deleteuser" data-id="' + row.Id + '"> حذف</div>';
                }
            }
        ]
    };
    var userTables = $('#UserdataTables').DataTable(settings);
    $('#UserdataTables_filter input').unbind();
    $("#UserdataTables_filter input").keyup(function (e) {
        search = $(this).val();
        if (e.keyCode === 13) {
            userTables.ajax.reload();
        }
    });
    var fingerbtn;
    $(document).on('click', '.fingerbtn', function (e) {
        fingerbtn = $(this);
        $('#globalLoadingModal').modal('show');
        var data = {
            witchFinger: $(this).data('fingerid'),
            userId: $(this).data('userid')
        };
        var url = $('#UserdataTables').data('addfinger');
        $.ajax({
            type: 'POST',
            url: url,
            async: false,
            data: data,
            success: function (data) {
                $('#globalLoadingModal').modal('hide');
                if (data.status === 'success') {
                    if (fingerbtn)
                        fingerbtn.addClass('btn-success').removeClass('btn-default');
                    $('#globalLoadingModal').modal('hide');
                    $('.validation-summary-errors').addClass('hidden').empty();
                } else if (data.status === 'fail') {
                    $('#globalLoadingModal').modal('hide');
                    if (fingerbtn)
                        fingerbtn.addClass('btn-default').removeClass('btn-success');
                    $('.validation-summary-errors').empty().append('<ul class="validation-summary-errors-list"></ul>').removeClass('hidden');
                    for (var cnt = 0; cnt < data.errors.length; cnt++) {
                        $('.validation-summary-errors-list').append('<li>' + data.errors[cnt] + '</li>');
                    }

                }
                $('#UserdataTables').ajax.reload();
                // $("#answers").html(response);
            },
            error: function (errors) {

                $('#globalLoadingModal').modal('hide');
            }
        });
    });

    $('.deleteuser').bind('click', function (e) {
        $('#globalLoadingModal').modal('show');
        var url = $('#UserdataTables').data('removeuser');
        $.ajax({
            type: 'POST',
            url: url,
            async: false,
            data: $(this).data('id'),
            contentType: false,
            processData: false,
            success: function (data) {
                $('#globalLoadingModal').modal('hide');
                if (data.status === 'success') {
                    $('#globalLoadingModal').modal('hide');
                    $('.validation-summary-errors').addClass('hidden').empty();
                } else if (data.status === 'fail') {
                    $('#globalLoadingModal').modal('hide');
                    $('.validation-summary-errors').empty().append('<ul class="validation-summary-errors-list"></ul>').removeClass('hidden');
                    for (var cnt = 0; cnt < data.errors.length; cnt++) {
                        form.find('.validation-summary-errors-list').append('<li>' + data.errors[cnt] + '</li>');
                    }

                }
                $('#UserdataTables').ajax.reload();
                // $("#answers").html(response);
            },
            error: function (errors) {

                $('#globalLoadingModal').modal('hide');
            }
        });
    });
});