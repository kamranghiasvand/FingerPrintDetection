$(document).ready(function () {
    /*.......datatable..........*/
    var search = '';
    var datatableUrl = $("#LogdataTables").data('url');
    $.fn.dataTable.ext.buttons.reload = {
        text: "بارگذاری مجدد",
        action: function (e, dt) {
            dt.ajax.reload();
            $('#LogdataTables_filter input').val(search);
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
        dom: 'Brtip',
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
                name: "Income",
                title: "وضعیت",
                className: "all",
                visible: true,
                sortable: false,
                searchable: false,
                render: function (data, type, row) {
                    
                    if (row.Income) return '<div class="alert alert-success"><span class="glyphicon glyphicon-arrow-down" aria-hidden="true"></span></div>';
                    else return '<div class="alert alert-danger"><span class="glyphicon glyphicon-arrow-up" aria-hidden="true"></span></div>';
                }
            },
            {
                name: "RealUser",
                title: "کاربر واقعی",
                className: "all",
                visible: true,
                sortable: false,
                searchable: false,
                render: function (data, type, row) {
                    return row.RealUser;
                }
            },
            {
                name: "LogicalUser",
                title: "کاربر منطقی",
                className: "all",
                visible: true,
                sortable: false,
                searchable:false,
                render: function (data, type, row) {
                    return row.LogicalUser;
                }
            },
            {
                name: "Plan",
                title: "نام پلن",
                className: "all",
                visible: true,
                sortable: false,
                searchable: false,
                render: function (data, type, row) {
                    return row.Plan;
                }
            },
            {
                name: "Time",
                title: "زمان",
                className: "all",
                visible: true,
                sortable: false,
                searchable: false,
                render: function (data, type, row) {
                    return row.Time;
                }
            }
        ]
    };
    var tables = $('#LogdataTables').DataTable(settings);
    $('#LogdataTables_filter input').unbind();
    $("#LogdataTables_filter input").keyup(function (e) {
        search = $(this).val();
        if (e.keyCode === 13) {
            tables.ajax.reload();
        }
    });
    $("#calendar").persianDatepicker({
        onSelect: function (e) {
            var t = new Date( e );
            search = t.toUTCString();
            tables.ajax.reload();
        }
    });
});



