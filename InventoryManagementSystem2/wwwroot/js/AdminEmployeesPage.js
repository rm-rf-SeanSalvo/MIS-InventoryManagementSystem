// Define showPopup globally
function showPopup(message, isSuccess = false, onConfirm = null, autoClose = false, duration = 4000) {
    const popup = $('#popup-message');
    const popupText = $('#popup-text');

    popupText.text(message);

    // Style
    popup.removeClass('success error d-none');
    popup.addClass(isSuccess ? 'success' : 'error');
    popup.addClass('show');

    // Handle mode
    if (autoClose || onConfirm === null) {
        $('.confirmation-buttons').hide(); // Hide Yes/No
        setTimeout(() => {
            hidePopup();
        }, duration);
    } else {
        $('.confirmation-buttons').show(); // Show Yes/No
    }

    // Confirm button
    $('#confirm-action').off('click').on('click', function () {
        if (onConfirm) onConfirm();
        hidePopup();
    });

    // Cancel button
    $('#cancel-action').off('click').on('click', function () {
        hidePopup();
    });
}


// Function to hide the popup
function hidePopup() {
    const popup = $('#popup-message');
    popup.removeClass('show');
    setTimeout(() => popup.addClass('d-none'), 400); // Delay hiding for smooth transition
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

    $(document).on('click', '.delete-btn', function () {
        var row = $(this).closest('tr');
        var userId = row.data('userid');

        // Show the custom popup with confirmation
        showPopup("Are you sure you want to delete this employee?", false, function () {
            // Proceed with delete action
            $.ajax({
                url: '/Admin/DeleteEmployee', // Ensure this route exists and points to the correct action
                type: 'POST',
                data: { userId: userId },
                success: function (response) {
                    if (response.success) {
                        row.remove(); // Remove the row from the table
                        showPopup("Employee deleted successfully.", true);
                    } else {
                        showPopup(response.message || "Failed to delete employee.");
                    }
                },
                error: function () {
                    showPopup("Failed to delete employee.");
                }
            });
        });
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
});
