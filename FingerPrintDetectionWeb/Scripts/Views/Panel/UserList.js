$(document).ready(function () {
    /*.......datatable..........*/
    var search = '';
    var datatableUrl = $("#UserdataTables").data('url');
    $.fn.dataTable.ext.buttons.reload = {
        text:"بارگذاری مجدد",
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
                render: function(data, type, row) {
                    return row.LastName;
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

});