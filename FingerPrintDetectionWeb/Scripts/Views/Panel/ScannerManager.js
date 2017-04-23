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
                sortable: false,
                searchable: false,
                render: function (data, type, row) {
                    return row.ImageQuality;
                }
            },
            {
                name: "Timeout",
                title: "زمان اتمام",
                className: "all",
                visible: true,
                sortable: false,
                searchable: false,
                render: function (data, type, row) {
                    return row.Timeout;
                }
            },
            {
                name: "IsCapturing",
                title: "در حال گرفتن عکس",
                className: "all",
                visible: true,
                sortable: false,
                searchable: false,
                render: function (data, type, row) {
                    if (row.IsCapturing)
                        return '<div class="label label-success">بله</div>';
                    else
                        return '<div class="label label-danger">خیر</div>';
                }
            },
            {
                name: "IsSensorOn",
                title: "فعالیت سنسور",
                className: "all",
                visible: true,
                sortable: false,
                searchable: false,
                render: function (data, type, row) {
                    if (row.IsSensorOn)
                        return '<div class="label label-success">بله</div>';
                    else
                        return '<div class="label label-danger">خیر</div>';
                }
            }
            //,{
            //    name: 'Actions',
            //    title: 'اعمال',
            //    visible: true,
            //    className: all,
            //    sortable: false,
            //    searchable: false,
            //    render:function(data, type, row) {
            //        return '<div class="btn btn-success enableScanner" data-url="' + row.Id + '">فعال کردن</div>' +
            //            '<div class="btn btn-danger disableScanner" data-url="' + row.Id + '">غیر فعال کردن</div>';
            //    }
            //}
        ]
    };
    var tables = $('#ScannerdataTables').DataTable(settings);
    $('#ScannerdataTables_filter input').unbind();
    $("#ScannerdataTables_filter input").keyup(function (e) {
        search = $(this).val();
        if (e.keyCode === 13) {
            tables.ajax.reload();
        }
    });
    $('#toggleScannerManagerForm').bind('submit', function(e) {
        e.preventDefault();
        var form = $(this);
        var url = form.attr('action');
        $('#globalLoadingModal').modal('show');
        $.ajax({
            type: 'POST',
            url: url,
            async: true,
            contentType: false,
            processData: false,
            success: function (data) {
                $('#globalLoadingModal').modal('hide');
                if (data.state === true) {
                    form.parent().addClass('alert-success').removeClass('alert-danger');
                    $(form.children('input')[0]).attr('value', 'توقف');
                } else if (data.state === false) {
                    form.parent().addClass('alert-danger').removeClass('alert-success');
                    $(form.children('input')[0]).attr('value', 'راه اندازی');
                }
                tables.ajax.reload();
            },
            error: function (errors) {
                $('#globalLoadingModal').modal('hide');
            }
        });
    });
    //$('.disableScanner').bind('click', function(e) {
    //    e.preventDefault();
    //    var btn = $(this);
    //    $('#globalLoadingModal').modal('show');
    //    $.ajax({
    //        type: 'POST',
    //        url: url,
    //        async: true,
    //        contentType: false,
    //        processData: false,
    //        success: function (data) {
    //            $('#globalLoadingModal').modal('hide');
    //            if (data.state === true) {
    //                form.parent().addClass('alert-success').removeClass('alert-danger');
    //                $(form.children('input')[0]).attr('value', 'توقف');
    //            } else if (data.state === false) {
    //                form.parent().addClass('alert-danger').removeClass('alert-success');
    //                $(form.children('input')[0]).attr('value', 'راه اندازی');
    //            }
    //            tables.ajax.reload();
    //        },
    //        error: function (errors) {
    //            $('#globalLoadingModal').modal('hide');
    //        }
    //    });
    //});

    //$('.enableScanner').bind('click', function(e) {
        
    //});
});