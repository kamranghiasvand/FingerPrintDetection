$(document).ready(function () {
    /*.......datatable..........*/
    var search = '';
    var datatableUrl = $("#ScannerdataTables").data('url');
    $.fn.dataTable.ext.buttons.reload = {
        text: "بارگذاری مجدد",
        action: function (e, dt) {
            dt.ajax.reload();
            $('#ScannerdataTables_filter input').val(search);
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
                name: "Id",
                title: "شناسه",
                className: "all",
                visible: true,
                sortable: true,
                searchable: true,
                render: function (data, type, row) {
                    return row.Id;
                }
            },
            {
                name: "ImageQuality",
                title: "کیفیت عکس",
                className: "all",
                visible: true,
                sortable: true,
                searchable: true,
                render: function (data, type, row) {
                    return row.ImageQuality;
                }
            },,
            {
                name: "Timeout",
                title: "زمان اتمام",
                className: "all",
                visible: true,
                sortable: true,
                searchable: true,
                render: function (data, type, row) {
                    return row.Timeout;
                }
            },
            {
                name: "IsCapturing",
                title: "در حال گرفتن عکس",
                className: "all",
                visible: true,
                sortable: true,
                searchable: true,
                render: function (data, type, row) {
                    if (row.IsCapturing)
                        return '<div class="label label-success>بله</div>';
                    else
                        return '<div class="label label-danger>خیر</div>';
                }
            },
            {
                name: "IsSensorOn",
                title: "فعالیت سنسور",
                className: "all",
                visible: true,
                sortable: true,
                searchable: true,
                render: function (data, type, row) {
                    if (row.IsSensorOn)
                        return '<div class="label label-success>بله</div>';
                    else
                        return '<div class="label label-danger>خیر</div>';
                }
            }
        ]
    };
    var userTables = $('#ScannerdataTables').DataTable(settings);
    $('#ScannerdataTables_filter input').unbind();
    $("#ScannerdataTables_filter input").keyup(function (e) {
        search = $(this).val();
        if (e.keyCode === 13) {
            userTables.ajax.reload();
        }
    });
});