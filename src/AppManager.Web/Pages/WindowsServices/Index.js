$(function () {
    var l = abp.localization.getResource('AppManager');
    var dt = $('#WindowsServicesTable').DataTable(abp.libs.datatables.normalizeConfiguration({
        serverSide: true, processing: true, paging: true, ordering: true, searching: false, scrollX: true,
        ajax: abp.libs.datatables.createAjax(appManager.windowsServices.index.getList),
        columnDefs: [
            { title: l('WindowsServices:ServiceName'), data: 'serviceName' },
            { title: l('WindowsServices:DisplayName'), data: 'displayName' },
            { title: l('WindowsServices:Status'), data: 'status',
              render: function (d) { var c = d === 'Running' ? 'bg-success' : (d === 'Stopped' ? 'bg-danger' : 'bg-secondary'); return '<span class="badge ' + c + '">' + d + '</span>'; }},
            { title: l('WindowsServices:StartType'), data: 'startType' },
            { title: l('WindowsServices:Account'), data: 'account' },
            { title: l('Actions'), data: 'id', orderable: false,
              render: function (d) {
                  return '<div class="btn-group btn-group-sm">'
                      + '<button class="btn btn-sm btn-success start-btn" data-id="' + d + '"><i class="fa fa-play"></i></button>'
                      + '<button class="btn btn-sm btn-warning stop-btn" data-id="' + d + '"><i class="fa fa-stop"></i></button>'
                      + '<button class="btn btn-sm btn-info restart-btn" data-id="' + d + '"><i class="fa fa-refresh"></i></button>'
                      + '<button class="btn btn-sm btn-primary edit-btn" data-id="' + d + '"><i class="fa fa-pencil"></i></button>'
                      + '<button class="btn btn-sm btn-danger delete-btn" data-id="' + d + '"><i class="fa fa-trash"></i></button>'
                      + '</div>';
              }}
        ]
    }));
    $('#FilterInput').on('keyup', function () { dt.search(this.value).draw(); });
    $('#WindowsServicesTable').on('click', '.delete-btn', function () {
        var id = $(this).data('id');
        abp.message.confirm(l('DeleteConfirmation'), function (c) { if (c) appManager.windowsServices.index.delete(id).then(function () { dt.ajax.reload(); }); });
    });
    $('#WindowsServicesTable').on('click', '.start-btn', function () { appManager.windowsServices.index.start($(this).data('id')).then(function () { dt.ajax.reload(); }); });
    $('#WindowsServicesTable').on('click', '.stop-btn', function () { appManager.windowsServices.index.stop($(this).data('id')).then(function () { dt.ajax.reload(); }); });
    $('#WindowsServicesTable').on('click', '.restart-btn', function () { appManager.windowsServices.index.restart($(this).data('id')).then(function () { dt.ajax.reload(); }); });
});
