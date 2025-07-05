// Site-wide JavaScript for EShop Web.UI

// Persian number formatting
function formatPersianNumber(number) {
    const persianDigits = '۰۱۲۳۴۵۶۷۸۹';
    return number.toString().replace(/\d/g, function(digit) {
        return persianDigits[parseInt(digit)];
    });
}

// Format currency for Persian locale
function formatCurrency(amount) {
    return new Intl.NumberFormat('fa-IR', {
        style: 'currency',
        currency: 'IRR',
        minimumFractionDigits: 0,
        maximumFractionDigits: 0
    }).format(amount);
}

// Format date for Persian locale
function formatPersianDate(date) {
    return new Intl.DateTimeFormat('fa-IR', {
        year: 'numeric',
        month: 'long',
        day: 'numeric'
    }).format(new Date(date));
}

// Shopping cart functionality
class ShoppingCart {
    constructor() {
        this.items = JSON.parse(localStorage.getItem('cart') || '[]');
        this.updateCartUI();
    }

    addItem(productId, quantity = 1) {
        const existingItem = this.items.find(item => item.productId === productId);
        
        if (existingItem) {
            existingItem.quantity += quantity;
        } else {
            this.items.push({
                productId: productId,
                quantity: quantity,
                addedAt: new Date().toISOString()
            });
        }
        
        this.saveCart();
        this.updateCartUI();
        this.showNotification('محصول به سبد خرید اضافه شد', 'success');
    }

    removeItem(productId) {
        this.items = this.items.filter(item => item.productId !== productId);
        this.saveCart();
        this.updateCartUI();
        this.showNotification('محصول از سبد خرید حذف شد', 'info');
    }

    updateQuantity(productId, quantity) {
        const item = this.items.find(item => item.productId === productId);
        if (item) {
            if (quantity <= 0) {
                this.removeItem(productId);
            } else {
                item.quantity = quantity;
                this.saveCart();
                this.updateCartUI();
            }
        }
    }

    getItemCount() {
        return this.items.reduce((total, item) => total + item.quantity, 0);
    }

    clear() {
        this.items = [];
        this.saveCart();
        this.updateCartUI();
    }

    saveCart() {
        localStorage.setItem('cart', JSON.stringify(this.items));
    }

    updateCartUI() {
        const cartCount = this.getItemCount();
        const cartCountElement = document.getElementById('cart-count');
        if (cartCountElement) {
            cartCountElement.textContent = formatPersianNumber(cartCount);
            cartCountElement.style.display = cartCount > 0 ? 'inline' : 'none';
        }
    }

    showNotification(message, type = 'info') {
        // Create notification element
        const notification = document.createElement('div');
        notification.className = `alert alert-${type} alert-dismissible fade show position-fixed`;
        notification.style.cssText = 'top: 20px; right: 20px; z-index: 9999; max-width: 300px;';
        notification.innerHTML = `
            <strong>${message}</strong>
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;
        
        document.body.appendChild(notification);
        
        // Auto-remove after 3 seconds
        setTimeout(() => {
            if (notification.parentNode) {
                notification.parentNode.removeChild(notification);
            }
        }, 3000);
    }
}

// Initialize cart
let cart;

// DOM Ready
document.addEventListener('DOMContentLoaded', function() {
    // Initialize shopping cart
    cart = new ShoppingCart();
    
    // Format all prices and numbers
    formatPageNumbers();
    
    // Handle add to cart buttons
    const addToCartButtons = document.querySelectorAll('.btn-add-to-cart');
    addToCartButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            e.preventDefault();
            const productId = parseInt(this.dataset.productId);
            const quantity = parseInt(this.dataset.quantity || 1);
            cart.addItem(productId, quantity);
        });
    });
    
    // Handle quantity changes
    const quantityInputs = document.querySelectorAll('.quantity-input');
    quantityInputs.forEach(input => {
        input.addEventListener('change', function() {
            const productId = parseInt(this.dataset.productId);
            const quantity = parseInt(this.value);
            cart.updateQuantity(productId, quantity);
        });
    });
    
    // Handle remove from cart buttons
    const removeButtons = document.querySelectorAll('.btn-remove-from-cart');
    removeButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            e.preventDefault();
            const productId = parseInt(this.dataset.productId);
            cart.removeItem(productId);
        });
    });
    
    // Handle search form
    const searchForm = document.querySelector('.search-form');
    if (searchForm) {
        searchForm.addEventListener('submit', function(e) {
            const searchInput = this.querySelector('input[name="term"]');
            if (!searchInput.value.trim()) {
                e.preventDefault();
                searchInput.focus();
            }
        });
    }
    
    // Handle lazy loading images
    const lazyImages = document.querySelectorAll('img[data-src]');
    if ('IntersectionObserver' in window) {
        const imageObserver = new IntersectionObserver((entries, observer) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    const img = entry.target;
                    img.src = img.dataset.src;
                    img.classList.remove('lazy');
                    observer.unobserve(img);
                }
            });
        });
        
        lazyImages.forEach(img => imageObserver.observe(img));
    }
    
    // Handle filter forms
    const filterForms = document.querySelectorAll('.filter-form');
    filterForms.forEach(form => {
        const inputs = form.querySelectorAll('input, select');
        inputs.forEach(input => {
            input.addEventListener('change', function() {
                form.submit();
            });
        });
    });
    
    // Handle smooth scrolling
    const scrollLinks = document.querySelectorAll('a[href^="#"]');
    scrollLinks.forEach(link => {
        link.addEventListener('click', function(e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                target.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });
});

// Format all numbers and prices on the page
function formatPageNumbers() {
    // Format prices
    const prices = document.querySelectorAll('.price, .original-price, .currency');
    prices.forEach(price => {
        const text = price.textContent.replace(/[^\d]/g, '');
        if (text) {
            const formatted = formatCurrency(parseInt(text));
            price.textContent = formatted;
        }
    });
    
    // Format general numbers
    const numbers = document.querySelectorAll('.persian-number');
    numbers.forEach(number => {
        const text = number.textContent.replace(/[^\d]/g, '');
        if (text) {
            number.textContent = formatPersianNumber(text);
        }
    });
    
    // Format dates
    const dates = document.querySelectorAll('.persian-date');
    dates.forEach(date => {
        const dateValue = date.dataset.date || date.textContent;
        if (dateValue) {
            try {
                const formatted = formatPersianDate(dateValue);
                date.textContent = formatted;
            } catch (e) {
                console.warn('Invalid date format:', dateValue);
            }
        }
    });
}

// Utility functions
function showLoader() {
    const loader = document.createElement('div');
    loader.id = 'page-loader';
    loader.innerHTML = `
        <div class="d-flex justify-content-center align-items-center" style="height: 100vh;">
            <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">در حال بارگذاری...</span>
            </div>
        </div>
    `;
    loader.style.cssText = 'position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(255,255,255,0.8); z-index: 9999;';
    document.body.appendChild(loader);
}

function hideLoader() {
    const loader = document.getElementById('page-loader');
    if (loader) {
        loader.remove();
    }
}

function confirmAction(message, callback) {
    if (confirm(message)) {
        callback();
    }
}

function copyToClipboard(text) {
    navigator.clipboard.writeText(text).then(function() {
        showNotification('کپی شد!', 'success');
    }, function() {
        showNotification('خطا در کپی کردن', 'error');
    });
}

function showNotification(message, type = 'info') {
    const notification = document.createElement('div');
    notification.className = `alert alert-${type} alert-dismissible fade show position-fixed`;
    notification.style.cssText = 'top: 20px; right: 20px; z-index: 9999; max-width: 300px;';
    notification.innerHTML = `
        <strong>${message}</strong>
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
    
    document.body.appendChild(notification);
    
    setTimeout(() => {
        if (notification.parentNode) {
            notification.parentNode.removeChild(notification);
        }
    }, 3000);
}

// Export functions for global use
window.cart = cart;
window.formatPersianNumber = formatPersianNumber;
window.formatCurrency = formatCurrency;
window.formatPersianDate = formatPersianDate;
window.showLoader = showLoader;
window.hideLoader = hideLoader;
window.confirmAction = confirmAction;
window.copyToClipboard = copyToClipboard;
window.showNotification = showNotification;