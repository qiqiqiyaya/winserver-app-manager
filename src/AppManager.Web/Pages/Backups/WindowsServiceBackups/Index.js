$(function () {
    var l = abp.localization.getResource('AppManager');
    var dt = $('#ServiceBackupsTable').DataTable(abp.libs.datatables.normalizeConfiguration({
        serverSide: true, processing: true, paging: true, ordering: true, searching: false, scrollX: true,
        ajax: abp.libs.datatables.createAjax(appManager.application.backups.windowsServiceBackup.getList),
        columnDefs: [
            { title: l('WindowsServices:ServiceName'), data: 'serviceName' },
            { title: l('WindowsServices:DisplayName'), data: 'displayName' },
            { title: l('Backups:Description'), data: 'description' },
            { title: l('Backups:CreatedAt'), data: 'createdAt', render: function (d) { return d ? new Date(d).toLocaleString() : ''; } },
            { title: l('Actions'), data: 'id', orderable: false,
              render: function (d) {
                  return '<div class="btn-group btn-group-sm">'
                      + '<button class="btn btn-sm btn-info preview-btn" data-id="' + d + '"><i class="fa fa-eye"></i></button>'
                      + '<button class="btn btn-sm btn-warning restore-btn" data-id="' + d + '"><i class="fa fa-undo"></i></button>'
                      + '<button class="btn btn-sm btn-danger delete-btn" data-id="' + d + '"><i class="fa fa-trash"></i></button>'
                      + '</div>';
              }}
        ]
    }));
    $('#FilterInput').on('keyup', function () { dt.search(this.value).draw(); });
    $('#ServiceBackupsTable').on('click', '.delete-btn', function () {
        var id = $(this).data('id');
        abp.message.confirm(l('DeleteConfirmation'), function (c) {
            if (c) {
                appManager.application.backups.windowsServiceBackup.delete(id).then(function () {
                    dt.ajax.reload();
                }).catch(function () {
                    dt.ajax.reload();
                });
            }
        });
    });
});