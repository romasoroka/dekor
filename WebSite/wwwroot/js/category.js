
var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {

    dataTable = $('#dataTable').DataTable({
        "ajax": {
            "url": "/Admin/Category/GetAll",
            "type": "GET",
            "dataType": "json"
        },
        "columns": [
            { "data": "name", "width": "50%" },
            {
                "data": "id",
                "render": function (data) {
                    return `<a href="/Admin/Category/Upsert?id=${data}" class="btn btn-primary"><i class="bi bi-pencil-square"></i> Edit</a>
                            <a onclick="Delete('/Admin/Category/Delete/${data}')" class="btn btn-danger">
                                <i class="fas fa-trash"></i> Delete
                            </a>`

                },
                "width": "50%"
            }
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    } else {
                        toastr.error(data.message);
                    }
                },
                error: function () {
                    toastr.error("Something went wrong while deleting.");
                }
            });
        }
    });
}
