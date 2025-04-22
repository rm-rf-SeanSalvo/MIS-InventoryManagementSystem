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


    // ======= Ready Function =======
    $(document).ready(function () {

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

            $(document).on('click', '.discard-category-btn', function () {
                location.reload();
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
