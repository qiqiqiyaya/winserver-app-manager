$(function () {
    var l = abp.localization.getResource('AppManager');
    var dataTable = $('#IisInstancesTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            processing: true,
            paging: true,
            ordering: true,
            searching: false,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(
                appManager.application.iisSites.iisInstance.getList
            ),
            columnDefs: [
                {
                    title: l('IisInstances:Name'),
                    data: 'name',
                },
                {
                    title: l('IisInstances:ConfigPath'),
                    data: 'configPath',
                    render: function (data) {
                        return data || l('IisInstances:SystemDefault');
                    }
                },
                {
                    title: l('IisInstances:Status'),
                    data: 'status',
                    render: function (data) {
                        var cls = data === 'Connected' ? 'badge bg-success' : 'badge bg-secondary';
                        return '<span class="' + cls + '">' + data + '</span>';
                    }
                },
                {
                    title: l('Actions'),
                    data: 'id',
                    orderable: false,
                    render: function (data) {
                        return '<div class="btn-group btn-group-sm">'
                            + '<button class="btn btn-sm btn-danger delete-btn" data-id="' + data + '" title="' + l('Delete') + '"><i class="fa fa-trash"></i></button>'
                            + '</div>';
                    }
                }
            ]
        })
    );

    $('#CreateInstanceButton').on('click', function () {
        var createModal = new abp.ModalManager({
            viewUrl: abp.appPath + 'IisInstances/CreateModal'
        });
        createModal.onResult(function () {
            dataTable.ajax.reload();
        });
        createModal.open();
    });

    $('#IisInstancesTable').on('click', '.delete-btn', function () {
        var id = $(this).data('id');
        abp.message.confirm(l('DeleteConfirmation'), function (confirmed) {
            if (confirmed) {
                appManager.application.iisSites.iisInstance.delete(id).then(function () {
                    dataTable.ajax.reload();
                    abp.notify.success(l('DeletedSuccessfully'));
                }).catch(function () {
                    dataTable.ajax.reload();
                });
            }
        });
    });
});