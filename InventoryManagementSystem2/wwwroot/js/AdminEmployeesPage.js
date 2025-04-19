// Define showPopup globally
function showPopup(message, isSuccess = false) {
    const popup = $('#popup-message');
    popup.text(message);

    popup.removeClass('success error d-none');
    popup.addClass(isSuccess ? 'success' : 'error');

    popup.addClass('show');

    setTimeout(() => {
        popup.removeClass('show');
        setTimeout(() => popup.addClass('d-none'), 400);
    }, 2500);
}

$(document).ready(function () {
    $('.edit-btn').on('click', function () {
        var row = $(this).closest('tr');

        var first = row.find('.editable-first').text().trim();
        var last = row.find('.editable-last').text().trim();
        var status = row.find('.emp-status').text().trim();
        var role = row.find('.emp-role').text().trim();

        row.find('.editable-first').html(`<input type="text" class="form-control first-input" value="${first}" />`);
        row.find('.editable-last').html(`<input type="text" class="form-control last-input" value="${last}" />`);
        row.find('.emp-status').html(`
            <select class="form-select status-select">
                <option value="1" ${status === 'Active' ? 'selected' : ''}>Active</option>
                <option value="0" ${status === 'Inactive' ? 'selected' : ''}>Inactive</option>
            </select>`);
        row.find('.emp-role').html(`
            <select class="form-select role-select">
                <option value="Employee" ${role === 'Employee' ? 'selected' : ''}>Employee</option>
                <option value="Manager" ${role === 'Manager' ? 'selected' : ''}>Manager</option>
            </select>`);

        row.find('.edit-btn, .delete-btn').addClass('d-none');
        row.find('.finish-btn, .discard-btn').removeClass('d-none');
    });

    $(document).on('click', '.discard-btn', function () {
        location.reload();
    });

    $(document).on('click', '.finish-btn', function () {
        var row = $(this).closest('tr');
        var userId = row.data('userid');
        var firstName = row.find('.first-input').val();
        var lastName = row.find('.last-input').val();
        var status = row.find('.status-select').val();
        var role = row.find('.role-select').val();

        $.ajax({
            url: '/Admin/UpdateEmployee',
            type: 'POST',
            data: {
                userId: userId,
                firstName: firstName,
                lastName: lastName,
                status: status,
                role: role
            },
            success: function () {
                showPopup("Employee updated successfully!", true);
                setTimeout(() => location.reload(), 1500);
            },
            error: function () {
                showPopup("Failed to update employee.");
            }
        });
    });

    $(document).on('click', '.delete-btn', function () {
        var row = $(this).closest('tr');
        var userId = row.data('userid');

        // Show the custom modal
        $('#delete-modal').removeClass('d-none');

        // Handle confirm delete
        $('#confirm-delete').on('click', function () {
            $.ajax({
                url: '/Admin/DeleteEmployee',
                type: 'POST',
                data: { userId: userId },
                success: function (response) {
                    if (response.success) {
                        row.remove();
                        showPopup("Employee deleted successfully.", true);
                    } else {
                        showPopup(response.message);
                    }
                    // Close the modal after action
                    $('#delete-modal').addClass('d-none');
                },
                error: function () {
                    showPopup("Failed to delete employee.");
                    $('#delete-modal').addClass('d-none');
                }
            });
        });

        // Handle cancel delete
        $('#cancel-delete').on('click', function () {
            $('#delete-modal').addClass('d-none');
        });
    });
});
