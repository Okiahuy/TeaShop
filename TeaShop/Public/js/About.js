document.addEventListener('DOMContentLoaded', function () {
    const promotionBanner = document.getElementById('promotion-banner');
    if (promotionBanner) {
        setTimeout(() => {
            promotionBanner.style.display = 'block';
        }, 5000); // Hiển thị sau 5 giây
    }

    // Thêm hiệu ứng chuyển động
    const sections = document.querySelectorAll('section');
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('visible');
                observer.unobserve(entry.target);
            }
        });
    }, {
        threshold: 0.1
    });

    sections.forEach(section => {
        observer.observe(section);
    });
});

function addToCart(productId) {
    let cart = JSON.parse(localStorage.getItem('cart')) || [];
    const product = getProductById(productId);
    cart.push(product);
    localStorage.setItem('cart', JSON.stringify(cart));
    updateCartUI();
}

function updateCartUI() {
    const cart = JSON.parse(localStorage.getItem('cart')) || [];
    document.getElementById('cart-count').innerText = cart.length;
}

function getProductById(productId) {
    // Giả sử chúng ta có một danh sách sản phẩm. Điều này nên được thay thế bằng cách gọi API thực tế.
    const products = [
        { id: 1, name: "Product 1" },
        { id: 2, name: "Product 2" },
        { id: 3, name: "Product 3" }
    ];
    return products.find(product => product.id === productId);
}