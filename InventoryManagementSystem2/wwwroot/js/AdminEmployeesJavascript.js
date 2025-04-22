// ======= Global Popup Handler =======
function showPopup(message, isSuccess = false, onConfirm = null, autoClose = false, duration = 4000) {
    const popup = $('#popup-message');
    const popupText = $('#popup-text');

    popupText.text(message);
    popup.removeClass('success error d-none').addClass(isSuccess ? 'success' : 'error').addClass('show');

    if (autoClose || onConfirm === null) {
        $('.confirmation-buttons').hide();
        setTimeout(() => hidePopup(), duration);
    } else {
        $('.confirmation-buttons').show();
    }

    $('#confirm-action').off('click').on('click', function () {
        if (onConfirm) onConfirm();
        hidePopup();
    });

    $('#cancel-action').off('click').on('click', function () {
        hidePopup();
    });
}

function hidePopup() {
    const popup = $('#popup-message');
    popup.removeClass('show');
    setTimeout(() => popup.addClass('d-none'), 400);
}

$(document).ready(function () {
    $('.edit-btn').on('click', function () {
        const $row = $(this).closest('tr');
        const originalData = {
            firstName: $row.find('.editable-first').text().trim(),
            lastName: $row.find('.editable-last').text().trim(),
            status: $row.find('.emp-status').text().trim(),
            role: $row.find('.emp-role').text().trim()
        };
        $row.data('original', originalData);
            
        $row.find('.editable-first').html(`<input type="text" class="form-control form-control-sm" value="${originalData.firstName}">`);
        $row.find('.editable-last').html(`<input type="text" class="form-control form-control-sm" value="${originalData.lastName}">`);

        const statusValue = originalData.status.toLowerCase() === 'active' || originalData.status === 'True';
        const statusDropdown = `
            <select class="form-select form-select-sm">
                <option value="true" ${statusValue ? 'selected' : ''}>Active</option>
                <option value="false" ${!statusValue ? 'selected' : ''}>Inactive</option>
            </select>`;
        $row.find('.emp-status').html(statusDropdown);

        const roleDropdown = `
            <select class="form-select form-select-sm">
                <option value="Admin" ${originalData.role === 'Manager' ? 'selected' : ''}>Manager</option>
                <option value="User" ${originalData.role === 'User' ? 'selected' : ''}>User</option>
            </select>`;
        $row.find('.emp-role').html(roleDropdown);

        $row.find('.edit-btn, .delete-btn').addClass('d-none');
        $row.find('.finish-btn, .discard-btn').removeClass('d-none');
    });

    $('.discard-btn').on('click', function () {
        const $row = $(this).closest('tr');
        const original = $row.data('original');

        // Revert values
        $row.find('.editable-first').text(original.firstName);
        $row.find('.editable-last').text(original.lastName);
        $row.find('.emp-status').text(original.status);
        $row.find('.emp-role').text(original.role);

        $row.find('.edit-btn, .delete-btn').removeClass('d-none');
        $row.find('.finish-btn, .discard-btn').addClass('d-none');
    });

    $('.finish-btn').on('click', function () {
        const $row = $(this).closest('tr');
        const userId = $row.data('userid');

        const firstName = $row.find('.editable-first input').val().trim();
        const lastName = $row.find('.editable-last input').val().trim();
        const status = $row.find('.emp-status select').val() === 'true';
        const role = $row.find('.emp-role select').val();

        $.ajax({
            type: 'POST',
            url: '/Admin/UpdateEmployee',
            data: { userId, firstName, lastName, status, role },
            success: function (response) {
                if (response.success) {
                    // Update UI
                    $row.find('.editable-first').text(firstName);
                    $row.find('.editable-last').text(lastName);
                    $row.find('.emp-status').text(status ? 'Active' : 'Inactive');
                    $row.find('.emp-role').text(role);

                    showPopup('Employee updated successfully!', true, null, true);
                } else {
                    showPopup(response.message || 'Failed to update employee.', false, null, true);
                }
            },
            error: function () {
                showPopup('Server error while updating employee.', false, null, true);
            },
            complete: function () {
                // Reset buttons
                $row.find('.edit-btn, .delete-btn').removeClass('d-none');
                $row.find('.finish-btn, .discard-btn').addClass('d-none');
            }
        });
    });

    $('.delete-btn').on('click', function () {
        const $row = $(this).closest('tr');
        const userId = $row.data('userid');

        showPopup("Are you sure you want to delete this employee?", false, function () {
            $.post('/Admin/DeleteEmployee', { userId }, function (response) {
                if (response.success) {
                    $row.remove();
                    showPopup("Employee deleted successfully!", true, null, true);
                } else {
                    showPopup(response.message || "Failed to delete employee.", false, null, true);
                }
            }).fail(function () {
                showPopup("Server error during deletion.", false, null, true);
            });
        });
    });
});
