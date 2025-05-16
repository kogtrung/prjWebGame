function updateCartCount() {
    fetch('/ShoppingCart/GetCartCount')
        .then(response => response.json())
        .then(data => {
            document.getElementById("cart-count").innerText = data.totalItems || 0;
        })
        .catch(error => console.error('Lỗi:', error));
}

function updateQuantity(productId, change) {
    fetch('/ShoppingCart/UpdateQuantity', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ productId: productId, change: change })
    })
        .then(response => response.json())
        .then(() => location.reload())
        .catch(error => console.error('Lỗi:', error));
}

function removeFromCart(productId) {
    fetch('/ShoppingCart/RemoveFromCart', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ productId: productId })
    })
        .then(() => location.reload())
        .catch(error => console.error('Lỗi:', error));
}

window.onload = updateCartCount;
