$(function () {
    var l = abp.localization.getResource('AppManager');
    var dataTable = $('#IisSitesTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            processing: true,
            paging: true,
            ordering: true,
            searching: false,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(
                appManager.iisSites.index.getList
            ),
            columnDefs: [
                {
                    title: l('IisSites:SiteName'),
                    data: 'siteName',
                },
                {
                    title: l('IisSites:PhysicalPath'),
                    data: 'physicalPath',
                },
                {
                    title: l('IisSites:Status'),
                    data: 'status',
                    render: function (data) {
                        var cls = data === 'Running' ? 'badge bg-success' : 'badge bg-secondary';
                        return '<span class="' + cls + '">' + data + '</span>';
                    }
                },
                {
                    title: l('IisSites:Port'),
                    data: 'port',
                },
                {
                    title: l('IisSites:AppPoolName'),
                    data: 'appPoolName',
                },
                {
                    title: l('Actions'),
                    data: 'id',
                    orderable: false,
                    render: function (data, type, row) {
                        return '<div class="btn-group btn-group-sm">'
                            + '<button class="btn btn-sm btn-warning start-btn" data-id="' + data + '" title="' + l('IisSites:Start') + '"><i class="fa fa-play"></i></button>'
                            + '<button class="btn btn-sm btn-secondary stop-btn" data-id="' + data + '" title="' + l('IisSites:Stop') + '"><i class="fa fa-stop"></i></button>'
                            + '<button class="btn btn-sm btn-info edit-btn" data-id="' + data + '" title="' + l('Edit') + '"><i class="fa fa-pencil"></i></button>'
                            + '<button class="btn btn-sm btn-danger delete-btn" data-id="' + data + '" title="' + l('Delete') + '"><i class="fa fa-trash"></i></button>'
                            + '</div>';
                    }
                }
            ]
        })
    );

    $('#FilterInput').on('keyup', function () {
        dataTable.search(this.value).draw();
    });

    $('#CreateSiteButton').on('click', function () {
        // TODO: open create modal
    });

    $('#IisSitesTable').on('click', '.delete-btn', function () {
        var id = $(this).data('id');
        abp.message.confirm(l('DeleteConfirmation'), function (confirmed) {
            if (confirmed) {
                appManager.iisSites.index.delete(id).then(function () {
                    dataTable.ajax.reload();
                    abp.notify.success(l('DeletedSuccessfully'));
                });
            }
        });
    });

    $('#IisSitesTable').on('click', '.start-btn', function () {
        var id = $(this).data('id');
        appManager.iisSites.index.start(id).then(function () {
            dataTable.ajax.reload();
            abp.notify.success(l('IisSites:Started'));
        });
    });

    $('#IisSitesTable').on('click', '.stop-btn', function () {
        var id = $(this).data('id');
        appManager.iisSites.index.stop(id).then(function () {
            dataTable.ajax.reload();
            abp.notify.success(l('IisSites:Stopped'));
        });
    });
});
