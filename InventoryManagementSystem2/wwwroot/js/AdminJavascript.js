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

// Function to show the Add Item form and hide the table
function showAddItemForm() {
    document.getElementById("itemsTable").style.display = "none"; // Hide the table
    document.getElementById("addItemForm").style.display = "block"; // Show the form
}

// Function to cancel and go back to the stock table
function cancelAddItem() {
    document.getElementById("itemsTable").style.display = "block"; // Show the table
    document.getElementById("addItemForm").style.display = "none"; // Hide the form
}


function hidePopup() {
    const popup = $('#popup-message');
    popup.removeClass('show');
    setTimeout(() => popup.addClass('d-none'), 400);
}

// ======= Ready Function =======
$(document).ready(function () {

    // ======= Employee Inline Editing =======
    $(document).on('click', '.edit-btn', function () {
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

    $(document).on('click', '.finish-btn', function () {
        var row = $(this).closest('tr');
        var userId = row.data('userid');
        var first = row.find('.first-input').val().trim();
        var last = row.find('.last-input').val().trim();
        var status = row.find('.status-select').val();
        var role = row.find('.role-select').val();

        if (!first || !last || !status || !role) {
            showPopup("All fields are required.");
            return;
        }

        $.ajax({
            url: '/Admin/UpdateEmployee',
            type: 'POST',
            data: { userId, first, last, status, role },
            success: function (response) {
                if (response.success) {
                    row.find('.editable-first').text(first);
                    row.find('.editable-last').text(last);
                    row.find('.emp-status').text(status === "1" ? 'Active' : 'Inactive');
                    row.find('.emp-role').text(role);
                    showPopup("Employee updated successfully!", true);
                } else {
                    showPopup(response.message || "Failed to update employee.");
                }

                row.find('.finish-btn, .discard-btn').addClass('d-none');
                row.find('.edit-btn, .delete-btn').removeClass('d-none');
            },
            error: function () {
                showPopup("Error updating employee.");
            }
        });
    });

    $(document).on('click', '.discard-btn', function () {
        location.reload();
    });

    // ======= Category Inline Editing =======
    $(document).on('click', '.edit-category-btn', function () {
        var row = $(this).closest('tr');
        var currentName = row.find('.category-name').text().trim();

        row.find('.category-name').html(`<input type="text" class="form-control category-input" value="${currentName}" />`);
        row.find('.edit-category-btn, .delete-category-btn').addClass('d-none');
        row.find('.finish-category-btn, .discard-category-btn').removeClass('d-none');
    });

    $(document).on('click', '.finish-category-btn', function () {
        var row = $(this).closest('tr');
        var categoryId = row.data('categoryid');
        var newName = row.find('.category-input').val().trim();

        if (!newName) {
            showPopup("Category name cannot be empty.");
            return;
        }

        $.ajax({
            url: '/Admin/UpdateCategory',
            type: 'POST',
            data: { categoryId, categoryName: newName },
            success: function (response) {
                if (response.success) {
                    row.find('.category-name').text(newName);
                    showPopup("Category updated successfully!", true);
                } else {
                    showPopup(response.message || "Failed to update category.");
                }

                row.find('.finish-category-btn, .discard-category-btn').addClass('d-none');
                row.find('.edit-category-btn, .delete-category-btn').removeClass('d-none');
            },
            error: function () {
                showPopup("Error updating category.");
            }
        });
    });

    $(document).on('click', '.discard-category-btn', function () {
        location.reload();
    });

    // ======= Delete Employee =======
    $(document).on('click', '.delete-btn', function () {
        var row = $(this).closest('tr');
        var userId = row.data('userid');

        showPopup("Are you sure you want to delete this employee?", false, function () {
            $.ajax({
                url: '/Admin/DeleteEmployee',
                type: 'POST',
                data: { userId: userId },
                success: function (response) {
                    if (response.success) {
                        row.remove();
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

    // ======= Delete Category =======
    $(document).on('click', '.delete-category-btn', function () {
        var row = $(this).closest('tr');
        var categoryId = row.data('categoryid');

        showPopup("Are you sure you want to delete this category?", false, function () {
            $.ajax({
                url: '/Admin/DeleteCategory',
                type: 'POST',
                data: { categoryId: categoryId },
                success: function (response) {
                    if (response.success) {
                        row.remove();
                        showPopup("Category deleted successfully.", true);
                    } else {
                        showPopup(response.message || "Failed to delete category.");
                    }
                },
                error: function () {
                    showPopup("Error deleting category.");
                }
            });
        });
    });
});
