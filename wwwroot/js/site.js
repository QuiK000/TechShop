// Site-wide JavaScript functions

// CSRF Token helper
function getAntiForgeryToken() {
    return $('input[name="__RequestVerificationToken"]').val();
}

// Add CSRF token to all AJAX requests
$.ajaxSetup({
    beforeSend: function(xhr, settings) {
        if (settings.type === 'POST' || settings.type === 'PUT' || settings.type === 'DELETE') {
            var token = getAntiForgeryToken();
            if (token) {
                xhr.setRequestHeader("RequestVerificationToken", token);
            }
        }
    }
});

// Format number as currency
function formatCurrency(amount) {
    return new Intl.NumberFormat('uk-UA', {
        style: 'currency',
        currency: 'UAH',
        minimumFractionDigits: 0
    }).format(amount);
}

// Show loading spinner
function showLoading(element) {
    const originalHtml = element.html();
    element.data('original-html', originalHtml);
    element.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Завантаження...');
    element.prop('disabled', true);
}

// Hide loading spinner
function hideLoading(element) {
    const originalHtml = element.data('original-html');
    element.html(originalHtml);
    element.prop('disabled', false);
}

// Show toast notification
function showToast(message, type = 'success') {
    const bgClass = type === 'success' ? 'bg-success' :
        type === 'danger' ? 'bg-danger' :
            type === 'warning' ? 'bg-warning' : 'bg-info';

    const toastHtml = `
        <div class="toast align-items-center text-white ${bgClass} border-0" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="d-flex">
                <div class="toast-body">
                    ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        </div>
    `;

    let container = document.getElementById('toast-container');
    if (!container) {
        container = document.createElement('div');
        container.id = 'toast-container';
        container.className = 'position-fixed bottom-0 end-0 p-3';
        container.style.zIndex = '11';
        document.body.appendChild(container);
    }

    container.insertAdjacentHTML('beforeend', toastHtml);
    const toastElement = container.lastElementChild;
    const toast = new bootstrap.Toast(toastElement);
    toast.show();

    toastElement.addEventListener('hidden.bs.toast', function () {
        toastElement.remove();
    });
}

// Confirm dialog
function confirmAction(message, callback) {
    if (confirm(message)) {
        callback();
    }
}

// Add to cart function
window.addToCart = function(productId, quantity = 1) {
    return $.ajax({
        url: '/Cart/AddToCart',
        method: 'POST',
        data: { productId: productId, quantity: quantity },
        success: function(response) {
            if (response.success) {
                $('#cartCount').text(response.cartCount);
                showToast('Товар додано до кошика!', 'success');
            } else {
                showToast(response.message || 'Помилка при додаванні товару', 'danger');
            }
        },
        error: function(xhr) {
            console.error('Cart error:', xhr);
            showToast('Помилка з\'єднання з сервером', 'danger');
        }
    });
};

// Update cart item quantity
window.updateCartQuantity = function(cartItemId, quantity) {
    return $.ajax({
        url: '/Cart/UpdateQuantity',
        method: 'POST',
        data: { cartItemId: cartItemId, quantity: quantity },
        success: function(response) {
            if (response.success) {
                location.reload();
            } else {
                showToast(response.message || 'Помилка при оновленні кількості', 'danger');
            }
        },
        error: function(xhr) {
            console.error('Update error:', xhr);
            showToast('Помилка з\'єднання з сервером', 'danger');
        }
    });
};

// Remove from cart
window.removeFromCart = function(cartItemId) {
    confirmAction('Видалити товар з кошика?', function() {
        $.ajax({
            url: '/Cart/RemoveFromCart',
            method: 'POST',
            data: { cartItemId: cartItemId },
            success: function(response) {
                if (response.success) {
                    $('#cartCount').text(response.cartCount);
                    location.reload();
                } else {
                    showToast(response.message || 'Помилка при видаленні товару', 'danger');
                }
            },
            error: function(xhr) {
                console.error('Remove error:', xhr);
                showToast('Помилка з\'єднання з сервером', 'danger');
            }
        });
    });
};

// Image preview for file upload
function previewImage(input, previewElement) {
    if (input.files && input.files[0]) {
        const reader = new FileReader();
        reader.onload = function(e) {
            $(previewElement).attr('src', e.target.result);
            $(previewElement).removeClass('d-none');
        };
        reader.readAsDataURL(input.files[0]);
    }
}

// Admin functions namespace
window.adminFunctions = {
    // Update order status
    updateOrderStatus: function(orderId, status) {
        if (!confirm('Змінити статус замовлення?')) {
            return;
        }

        $.ajax({
            url: '/Admin/Admin/UpdateOrderStatus',
            method: 'POST',
            data: {
                id: orderId,
                status: status
            },
            success: function(response) {
                if (response.success) {
                    showToast('Статус оновлено!', 'success');
                    setTimeout(function() {
                        location.reload();
                    }, 1000);
                } else {
                    showToast(response.message || 'Помилка при оновленні статусу', 'danger');
                }
            },
            error: function(xhr) {
                console.error('Status update error:', xhr);
                showToast('Помилка з\'єднання з сервером', 'danger');
            }
        });
    },

    // Delete review
    deleteReview: function(reviewId) {
        confirmAction('Видалити відгук?', function() {
            $.ajax({
                url: '/Review/Delete',
                method: 'POST',
                data: { id: reviewId },
                success: function(response) {
                    if (response.success) {
                        showToast('Відгук видалено!', 'success');
                        setTimeout(function() {
                            location.reload();
                        }, 1000);
                    } else {
                        showToast(response.message || 'Помилка при видаленні', 'danger');
                    }
                },
                error: function(xhr) {
                    console.error('Delete review error:', xhr);
                    showToast('Помилка з\'єднання з сервером', 'danger');
                }
            });
        });
    },

    // Lock/Unlock user
    toggleUserLock: function(userId, lock) {
        const action = lock ? 'заблокувати' : 'розблокувати';
        const url = lock ? '/Admin/Admin/LockUser' : '/Admin/Admin/UnlockUser';

        confirmAction(`Дійсно ${action} користувача?`, function() {
            $.ajax({
                url: url,
                method: 'POST',
                data: { id: userId },
                success: function(response) {
                    if (response.success) {
                        showToast(response.message, 'success');
                        setTimeout(function() {
                            location.reload();
                        }, 1000);
                    } else {
                        showToast(response.message || 'Помилка операції', 'danger');
                    }
                },
                error: function(xhr) {
                    console.error('Lock/unlock error:', xhr);
                    showToast('Помилка з\'єднання з сервером', 'danger');
                }
            });
        });
    },

    // Delete product
    deleteProduct: function(productId) {
        confirmAction('Видалити товар?', function() {
            $.ajax({
                url: '/Admin/Admin/DeleteProduct',
                method: 'POST',
                data: { id: productId },
                success: function(response) {
                    if (response.success) {
                        showToast('Товар видалено!', 'success');
                        setTimeout(function() {
                            location.reload();
                        }, 1000);
                    } else {
                        showToast(response.message || 'Помилка при видаленні', 'danger');
                    }
                },
                error: function(xhr) {
                    console.error('Delete product error:', xhr);
                    showToast('Помилка з\'єднання з сервером', 'danger');
                }
            });
        });
    },

    // Delete category
    deleteCategory: function(categoryId) {
        confirmAction('Видалити категорію?', function() {
            $.ajax({
                url: '/Admin/Categories/Delete',
                method: 'POST',
                data: { id: categoryId },
                success: function(response) {
                    if (response.success) {
                        showToast(response.message, 'success');
                        setTimeout(function() {
                            location.reload();
                        }, 1000);
                    } else {
                        showToast(response.message || 'Помилка при видаленні', 'danger');
                    }
                },
                error: function(xhr) {
                    console.error('Delete category error:', xhr);
                    showToast('Помилка з\'єднання з сервером', 'danger');
                }
            });
        });
    },

    // Bulk actions
    bulkAction: function(action, ids) {
        if (!ids || ids.length === 0) {
            showToast('Не обрано товарів', 'warning');
            return;
        }

        const actionText = action === 'delete' ? 'видалити' :
            action === 'activate' ? 'активувати' : 'деактивувати';

        confirmAction(`${actionText} ${ids.length} товарів?`, function() {
            $.ajax({
                url: '/Admin/Admin/BulkAction',
                method: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({ action: action, ids: ids }),
                success: function(response) {
                    if (response.success) {
                        showToast(response.message, 'success');
                        setTimeout(function() {
                            location.reload();
                        }, 1000);
                    } else {
                        showToast(response.message || 'Помилка операції', 'danger');
                    }
                },
                error: function(xhr) {
                    console.error('Bulk action error:', xhr);
                    showToast('Помилка з\'єднання з сервером', 'danger');
                }
            });
        });
    }
};

// Initialize on document ready
$(document).ready(function() {
    // Bootstrap tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Auto-hide alerts after 5 seconds
    setTimeout(function() {
        $('.alert:not(.alert-permanent)').fadeOut('slow');
    }, 5000);

    // Smooth scroll for anchor links
    $('a[href^="#"]').on('click', function(event) {
        const target = $(this.getAttribute('href'));
        if (target.length) {
            event.preventDefault();
            $('html, body').stop().animate({
                scrollTop: target.offset().top - 70
            }, 1000);
        }
    });

    // Add to cart buttons
    $(document).on('click', '.add-to-cart', function(e) {
        e.preventDefault();
        const btn = $(this);
        const productId = btn.data('product-id');
        const quantity = btn.data('quantity') || 1;

        showLoading(btn);
        addToCart(productId, quantity).always(function() {
            hideLoading(btn);
        });
    });

    // Cart quantity update
    $(document).on('change', '.cart-quantity', function() {
        const cartItemId = $(this).data('cart-item-id');
        const quantity = $(this).val();
        updateCartQuantity(cartItemId, quantity);
    });

    // Remove from cart
    $(document).on('click', '.remove-from-cart', function(e) {
        e.preventDefault();
        const cartItemId = $(this).data('cart-item-id');
        removeFromCart(cartItemId);
    });

    // File input preview
    $(document).on('change', 'input[type="file"].image-input', function() {
        const preview = $(this).data('preview');
        if (preview) {
            previewImage(this, preview);
        }
    });

    // Product rating display
    $('.product-rating').each(function() {
        const rating = parseFloat($(this).data('rating'));
        const stars = $(this);
        stars.empty();

        for (let i = 1; i <= 5; i++) {
            if (i <= rating) {
                stars.append('<i class="fas fa-star text-warning"></i>');
            } else if (i - 0.5 <= rating) {
                stars.append('<i class="fas fa-star-half-alt text-warning"></i>');
            } else {
                stars.append('<i class="far fa-star text-warning"></i>');
            }
        }
    });

    // Debug: Log AJAX errors
    $(document).ajaxError(function(event, xhr, settings, error) {
        console.error('AJAX Error:', {
            url: settings.url,
            status: xhr.status,
            statusText: xhr.statusText,
            responseText: xhr.responseText,
            error: error
        });
    });
});

// Global functions for inline onclick handlers
window.deleteProduct = function(id) {
    adminFunctions.deleteProduct(id);
};

window.deleteCategory = function(id) {
    adminFunctions.deleteCategory(id);
};

window.bulkAction = function(action) {
    const ids = [];
    $('.product-checkbox:checked').each(function() {
        ids.push(parseInt($(this).val()));
    });
    adminFunctions.bulkAction(action, ids);
};

window.clearCart = function() {
    confirmAction('Очистити кошик?', function() {
        $.ajax({
            url: '/Cart/ClearCart',
            method: 'POST',
            success: function(response) {
                if (response.success) {
                    location.reload();
                } else {
                    showToast(response.message || 'Помилка', 'danger');
                }
            },
            error: function(xhr) {
                console.error('Clear cart error:', xhr);
                showToast('Помилка з\'єднання з сервером', 'danger');
            }
        });
    });
};