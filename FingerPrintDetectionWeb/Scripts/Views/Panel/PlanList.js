$(document).ready(function () {
    /*.......datatable..........*/
    var search = '';
    var datatableUrl = $("#PlandataTables").data('url');
    $.fn.dataTable.ext.buttons.reload = {
        text: "بارگذاری مجدد",
        action: function (e, dt) {
            dt.ajax.reload();
            $('#PlandataTables_filter input').val(search);
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
                name: "Name",
                title: "نام",
                className: "all",
                visible: true,
                sortable: true,
                searchable: true,
                render: function (data, type, row) {
                    return row.Name;
                }
            },
            {
                name: "Description",
                title: "توضیحات",
                className: "all",
                visible: true,
                sortable: true,
                searchable: true,
                render: function (data, type, row) {
                    return row.Description;
                }
            },
            {
                name: "RepeatNumber",
                title: "تعداد تکرار",
                className: "all",
                visible: true,
                sortable: true,
                searchable: true,
                render: function (data, type, row) {
                    return row.RepeatNumber;
                }
            },
            {
                name: "MaxNumberOfUse",
                title: "حداکثر تعداد استفاده",
                className: "all",
                visible: true,
                sortable: false,
                searchable: false,
                render: function (data, type, row) {
                    return row.MaxNumberOfUse;
                }
            },
            {
                name: "StartTime",
                title: "زمان شروع",
                className: "all",
                visible: true,
                sortable: false,
                searchable: false,
                render: function (data, type, row) {
                    return row.StartTime;
                }
            },
            {
                name: "EndTime",
                title: "زمان خاتمه",
                className: "all",
                visible: true,
                sortable: false,
                searchable: false,
                render: function (data, type, row) {
                    return row.EndTime;
                }
            },
            {
                name: "Users",
                title: "تعداد کاربران انتصاب داده شده",
                className: "all",
                visible: true,
                sortable: true,
                searchable: true,
                render: function (data, type, row) {
                    return row.Users.length;
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
                    var url = $('#PlandataTables').data('editplan') + "?id=" + row.Id;
                    return '<div class="btn btn-sm btn-danger deleteplan" data-id="' + row.Id + '"> حذف</div>'+
                        '<a class="btn btn-sm btn-info" href="' + url + '"> ویرایش</div>';
                }
            }
        ]
    };
    var userTables = $('#PlandataTables').DataTable(settings);
    $('#PlandataTables_filter input').unbind();
    $("#PlandataTables_filter input").keyup(function (e) {
        search = $(this).val();
        if (e.keyCode === 13) {
            userTables.ajax.reload();
        }
    });
    $('#PlandataTables').on('click', '.deleteplan', function (e) {
        $('#globalLoadingModal').modal('show');
        var url = $('#PlandataTables').data('removeplan');
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