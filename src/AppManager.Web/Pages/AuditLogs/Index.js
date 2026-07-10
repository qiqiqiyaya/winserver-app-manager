$(function () {
    var l = abp.localization.getResource('AppManager');
    var dt = $('#AuditLogsTable').DataTable(abp.libs.datatables.normalizeConfiguration({
        serverSide: true, processing: true, paging: true, ordering: true, searching: false, scrollX: true,
        ajax: abp.libs.datatables.createAjax(appManager.application.auditLogs.auditLog.getList),
        columnDefs: [
            { title: l('AuditLogs:Time'), data: 'executionTime', render: function (d) { return d ? new Date(d).toLocaleString() : ''; } },
            { title: l('AuditLogs:User'), data: 'userName' },
            { title: l('AuditLogs:HttpMethod'), data: 'httpMethod',
              render: function (d) {
                  var c = d === 'GET' ? 'bg-success' : (d === 'POST' ? 'bg-primary' : (d === 'DELETE' ? 'bg-danger' : 'bg-info'));
                  return '<span class="badge ' + c + '">' + d + '</span>';
              }},
            { title: l('AuditLogs:Url'), data: 'url' },
            { title: l('AuditLogs:StatusCode'), data: 'httpStatusCode' },
            { title: l('AuditLogs:Duration'), data: 'executionDuration', render: function (d) { return d + ' ms'; } },
            { title: l('Actions'), data: 'id', orderable: false,
              render: function (d) { return '<button class="btn btn-sm btn-info detail-btn" data-id="' + d + '"><i class="fa fa-eye"></i></button>'; }}
        ]
    }));
    $('#UserFilter').on('keyup', function () { dt.columns(1).search(this.value).draw(); });
    $('#UrlFilter').on('keyup', function () { dt.columns(3).search(this.value).draw(); });
});
