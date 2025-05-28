unction showPopup(message, isSuccess = false, onConfirm = null, autoClose = false, duration = 4000) {
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
    popup.removeClass('show').addClass('d-none');
}

// Only runs after DOM is fully loaded
$(document).ready(function () {
    // Edit button clicked
    $(document).on('click', '.edit-category-btn', function () {
        var row = $(this).closest('tr');
        var currentName = row.find('.category-name').text().trim();

        row.find('.category-name').html(`<input type="text" class="form-control category-input" value="${currentName}" />`);
        row.find('.edit-category-btn, .delete-category-btn').addClass('d-none');
        row.find('.finish-category-btn, .discard-category-btn').removeClass('d-none');
    });

    // Discard button clicked
    $(document).on('click', '.discard-category-btn', function () {
        location.reload(); // Reload the page to reset the UI
    });

    // Finish button clicked
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
            data: { categoryId: categoryId, newCategoryName: newName },
            success: function (response) {
                if (response.success) {
                    row.find('.category-name').text(newName);
                    row.find('.edit-category-btn, .delete-category-btn').removeClass('d-none');
                    row.find('.finish-category-btn, .discard-category-btn').addClass('d-none');
                    showPopup("Category updated successfully.", true, null, true);
                } else {
                    showPopup(response.message || "Failed to update category.");
                }
            },
            error: function () {
                showPopup("Error updating category.");
            }
        });
    });

    // Delete button clicked
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
                        showPopup("Category deleted successfully.", true, null, true);
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
