// Toast Notification Function
function showToast(message, type) {
    type = type || 'success';
    const bgClass = type === 'success' ? 'bg-success' :
        type === 'danger' ? 'bg-danger' :
            type === 'warning' ? 'bg-warning' : 'bg-info';

    const toastHtml = `
        <div class="position-fixed bottom-0 end-0 p-3" style="z-index: 11">
            <div class="toast align-items-center text-white ${bgClass} border-0" role="alert">
                <div class="d-flex">
                    <div class="toast-body">${message}</div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
                </div>
            </div>
        </div>
    `;

    document.body.insertAdjacentHTML('beforeend', toastHtml);
    const toastElement = document.body.lastElementChild.querySelector('.toast');
    const toast = new bootstrap.Toast(toastElement, { delay: 3000 });
    toast.show();

    toastElement.addEventListener('hidden.bs.toast', function() {
        this.parentElement.remove();
    });
}

// Add to Cart Function
function addToCart(productId, quantity = 1) {
    $.ajax({
        url: '/Cart/AddToCart',
        method: 'POST',
        data: { productId: productId, quantity: quantity },
        success: function(response) {
            if (response.success) {
                showToast(response.message, 'success');
                updateCartCount();
            } else {
                showToast(response.message || 'Помилка додавання до кошика', 'danger');
            }
        },
        error: function() {
            showToast('Помилка з\'єднання з сервером', 'danger');
        }
    });
}

// Update Cart Quantity
function updateCartQuantity(cartItemId, quantity) {
    $.ajax({
        url: '/Cart/UpdateQuantity',
        method: 'POST',
        data: { cartItemId: cartItemId, quantity: quantity },
        success: function(response) {
            if (response.success) {
                location.reload();
            } else {
                showToast(response.message || 'Помилка оновлення кількості', 'danger');
            }
        },
        error: function() {
            showToast('Помилка з\'єднання з сервером', 'danger');
        }
    });
}

// Remove from Cart
function removeFromCart(cartItemId) {
    if (!confirm('Видалити товар з кошика?')) return;

    $.ajax({
        url: '/Cart/RemoveFromCart',
        method: 'POST',
        data: { cartItemId: cartItemId },
        success: function(response) {
            if (response.success) {
                showToast('Товар видалено з кошика', 'success');
                setTimeout(() => location.reload(), 1000);
            } else {
                showToast(response.message || 'Помилка видалення', 'danger');
            }
        },
        error: function() {
            showToast('Помилка з\'єднання з сервером', 'danger');
        }
    });
}

// Update Cart Count
function updateCartCount() {
    $.ajax({
        url: '/Cart/GetCartCount',
        method: 'GET',
        success: function(data) {
            $('#cartCount').text(data.count);
        }
    });
}

// Admin Functions
const adminFunctions = {
    deleteReview: function(reviewId) {
        if (!confirm('Видалити відгук?')) return;

        $.ajax({
            url: '/Review/Delete',
            method: 'POST',
            data: { id: reviewId },
            success: function(response) {
                if (response.success) {
                    showToast(response.message, 'success');
                    setTimeout(() => location.reload(), 1000);
                } else {
                    showToast(response.message, 'danger');
                }
            },
            error: function() {
                showToast('Помилка з\'єднання з сервером', 'danger');
            }
        });
    },

    addResponse: function(reviewId) {
        const response = prompt('Введіть відповідь на відгук:');
        if (!response) return;

        $.ajax({
            url: '/Review/AddResponse',
            method: 'POST',
            data: { reviewId: reviewId, response: response },
            success: function(result) {
                if (result.success) {
                    showToast(result.message, 'success');
                    setTimeout(() => location.reload(), 1000);
                } else {
                    showToast(result.message, 'danger');
                }
            },
            error: function() {
                showToast('Помилка з\'єднання з сервером', 'danger');
            }
        });
    }
};

// Document Ready
$(document).ready(function() {
    // Auto-hide alerts after 5 seconds
    setTimeout(function() {
        $('.alert').fadeOut('slow', function() {
            $(this).remove();
        });
    }, 5000);

    // Add to cart buttons
    $('.add-to-cart').on('click', function() {
        const productId = $(this).data('product-id');
        const quantity = $(this).data('quantity') || 1;
        addToCart(productId, quantity);
    });

    // Remove from cart buttons
    $('.remove-from-cart').on('click', function() {
        const cartItemId = $(this).data('cart-item-id');
        removeFromCart(cartItemId);
    });

    // Update cart count on page load
    if ($('#cartCount').length) {
        updateCartCount();
    }

    // Form validation
    $('form').on('submit', function(e) {
        const form = this;
        if (form.checkValidity() === false) {
            e.preventDefault();
            e.stopPropagation();
        }
        $(form).addClass('was-validated');
    });

    // Confirmation dialogs
    $('[data-confirm]').on('click', function(e) {
        const message = $(this).data('confirm');
        if (!confirm(message)) {
            e.preventDefault();
            return false;
        }
    });

    // Number input validation
    $('input[type="number"]').on('input', function() {
        const min = parseFloat($(this).attr('min'));
        const max = parseFloat($(this).attr('max'));
        const value = parseFloat($(this).val());

        if (!isNaN(min) && value < min) {
            $(this).val(min);
        }
        if (!isNaN(max) && value > max) {
            $(this).val(max);
        }
    });

    // Image preview
    $('input[type="file"][accept*="image"]').on('change', function(e) {
        const file = e.target.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = function(event) {
                const preview = $(e.target).siblings('.image-preview');
                if (preview.length) {
                    preview.attr('src', event.target.result).show();
                }
            };
            reader.readAsDataURL(file);
        }
    });

    // Highlight active navigation
    const currentPath = window.location.pathname;
    $('.nav-link').each(function() {
        const href = $(this).attr('href');
        if (href && currentPath.indexOf(href) !== -1 && href !== '/') {
            $(this).addClass('active');
        }
    });
});

// Get CSRF Token
function getAntiForgeryToken() {
    return $('input[name="__RequestVerificationToken"]').val();
}

// AJAX Setup for CSRF Token
$.ajaxSetup({
    beforeSend: function(xhr, settings) {
        if (settings.type !== 'GET') {
            const token = getAntiForgeryToken();
            if (token) {
                xhr.setRequestHeader('RequestVerificationToken', token);
            }
        }
    }
});

// Prevent double form submission
$('form').on('submit', function() {
    const $form = $(this);
    const $submitBtn = $form.find('button[type="submit"]');

    if ($form.data('submitted') === true) {
        return false;
    }

    $form.data('submitted', true);
    $submitBtn.prop('disabled', true);

    // Re-enable after 3 seconds (in case of validation errors)
    setTimeout(function() {
        $form.data('submitted', false);
        $submitBtn.prop('disabled', false);
    }, 3000);
});

// Format numbers
function formatNumber(number, decimals = 0) {
    return number.toLocaleString('uk-UA', {
        minimumFractionDigits: decimals,
        maximumFractionDigits: decimals
    });
}

// Format currency
function formatCurrency(amount) {
    return formatNumber(amount, 0) + ' грн';
}

// Debounce function
function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

// Export functions to window
window.showToast = showToast;
window.addToCart = addToCart;
window.updateCartQuantity = updateCartQuantity;
window.removeFromCart = removeFromCart;
window.updateCartCount = updateCartCount;
window.adminFunctions = adminFunctions;
window.formatNumber = formatNumber;
window.formatCurrency = formatCurrency;
window.debounce = debounce;