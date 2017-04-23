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
            "processing": '<i style="z-index:1000" class="fa fa-spinner fa-pulse fa-3x fa-fw"></i><span class="sr-only">Loading...</span>',
            "search": "جستجو:"
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
                sortable: false,
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
                sortable: false,
                searchable: true,
                render: function (data, type, row) {
                    return row.LastName;
                }
            },
            {
                name: "PlanName",
                title: "نام پلن",
                className: "all",
                visible: true,
                sortable: false,
                searchable: true,
                render: function (data, type, row) {
                    return row.PlanName;
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
                    var url = $('#UserdataTables').data('edituser')+"?id="+row.Id;
                    return '<div class="btn btn-sm btn-danger deleteuser" data-id="' + row.Id + '"> حذف</div>' +
                        '<a class="btn btn-sm btn-info edituser" href="' + url + '"> ویرایش</div>';
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
    $('#UserdataTables').on('click', '.deleteuser', function (e) {
        $('#globalLoadingModal').modal('show');
        var url = $('#UserdataTables').data('removeuser');
        $.ajax({
            type: 'POST',
            url: url,
            async: false,
            data: { "Id": $(e.target).data('id') },
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
                userTables.ajax.reload();
                // $("#answers").html(response);
            },
            error: function (errors) {

                $('#globalLoadingModal').modal('hide');
            }
        });
    });



});