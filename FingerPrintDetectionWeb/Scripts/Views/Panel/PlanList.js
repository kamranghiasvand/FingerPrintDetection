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
                sortable: true,
                searchable: true,
                render: function (data, type, row) {
                    return row.MaxNumberOfUse;
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
});