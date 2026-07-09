$(function () {
    var l = abp.localization.getResource('AppManager');
    var dt = $('#SystemLogsTable').DataTable(abp.libs.datatables.normalizeConfiguration({
        serverSide: true, processing: true, paging: true, ordering: true, searching: false, scrollX: true,
        ajax: abp.libs.datatables.createAjax(appManager.systemLogs.index.getList),
        columnDefs: [
            { title: l('SystemLogs:Timestamp'), data: 'timeStamp', render: function (d) { return d ? new Date(d).toLocaleString() : ''; } },
            { title: l('SystemLogs:Level'), data: 'level',
              render: function (d) {
                  var c = d === 'Error' ? 'bg-danger' : (d === 'Warning' ? 'bg-warning' : (d === 'Information' ? 'bg-info' : 'bg-secondary'));
                  return '<span class="badge ' + c + '">' + d + '</span>';
              }},
            { title: l('SystemLogs:Message'), data: 'message' },
            { title: l('SystemLogs:SourceContext'), data: 'sourceContext' }
        ]
    }));
    $('#KeywordFilter').on('keyup', function () { dt.search(this.value).draw(); });
});
