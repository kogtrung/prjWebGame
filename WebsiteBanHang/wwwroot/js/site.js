// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Video error handling
document.addEventListener('DOMContentLoaded', function() {
    // Handle video loading errors
    const videoIframes = document.querySelectorAll('iframe[src*="youtube"]');
    videoIframes.forEach(iframe => {
        // Add a class for styling purposes
        iframe.classList.add('video-player');
        
        // Create a fallback container to display when video fails
        const fallbackContainer = document.createElement('div');
        fallbackContainer.className = 'video-fallback d-none';
        fallbackContainer.innerHTML = `
            <div class="text-center p-4 bg-dark text-light rounded">
                <i class="fas fa-exclamation-circle fa-3x mb-3"></i>
                <p>Video không có sẵn</p>
                <p class="small">Video này không hoạt động</p>
                <a href="${iframe.src}" target="_blank" class="btn btn-sm btn-outline-light mt-2">
                    <i class="fab fa-youtube me-1"></i>Xem trên YouTube
                </a>
            </div>
        `;
        
        // Insert the fallback after the iframe
        iframe.parentNode.insertBefore(fallbackContainer, iframe.nextSibling);
        
        // Set a timeout to check if YouTube is blocked or not loading
        setTimeout(() => {
            try {
                // Try to access iframe content - if blocked, this will fail
                const iframeContent = iframe.contentWindow;
                if (!iframeContent || !iframeContent.document) {
                    // Show fallback if iframe content is inaccessible
                    iframe.classList.add('d-none');
                    fallbackContainer.classList.remove('d-none');
                }
            } catch (e) {
                // Show fallback if an error occurred
                iframe.classList.add('d-none');
                fallbackContainer.classList.remove('d-none');
            }
        }, 3000); // Check after 3 seconds
    });
    
    // Handle HTML5 video elements
    const videoElements = document.querySelectorAll('video');
    videoElements.forEach(video => {
        video.addEventListener('error', function() {
            // Create placeholder for failed video
            const videoContainer = video.closest('.ratio');
            if (videoContainer) {
                const fallbackMessage = document.createElement('div');
                fallbackMessage.className = 'video-error p-4 bg-dark text-light rounded';
                fallbackMessage.innerHTML = `
                    <div class="text-center">
                        <i class="fas fa-exclamation-circle fa-3x mb-3"></i>
                        <p>Video không có sẵn</p>
                        <p class="small">Video này không hoạt động</p>
                    </div>
                `;
                
                video.classList.add('d-none');
                videoContainer.appendChild(fallbackMessage);
            }
        });
    });
});

// Refresh game images function
function refreshGameImages() {
    // Show loading indicator
    const refreshButton = document.getElementById('refreshImagesBtn');
    if (refreshButton) {
        refreshButton.disabled = true;
        refreshButton.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Refreshing...';
    }
    
    // Call API endpoint
    fetch('/api/images/refresh')
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                // Show success message
                showToast('Game images refreshed successfully', 'success');
                
                // Reload page after a short delay to show updated images
                setTimeout(() => {
                    window.location.reload();
                }, 1500);
            } else {
                showToast('Failed to refresh game images', 'error');
            }
        })
        .catch(error => {
            console.error('Error refreshing game images:', error);
            showToast('Error refreshing game images', 'error');
        })
        .finally(() => {
            // Reset button state
            if (refreshButton) {
                refreshButton.disabled = false;
                refreshButton.innerHTML = '<i class="bi bi-arrow-clockwise"></i> Refresh Images';
            }
        });
}

// Simple toast notification function
function showToast(message, type = 'info') {
    // Check if toast container exists, if not create it
    let toastContainer = document.getElementById('toast-container');
    if (!toastContainer) {
        toastContainer = document.createElement('div');
        toastContainer.id = 'toast-container';
        toastContainer.className = 'position-fixed bottom-0 end-0 p-3';
        document.body.appendChild(toastContainer);
    }
    
    // Create toast element
    const toastId = 'toast-' + Date.now();
    const toastEl = document.createElement('div');
    toastEl.id = toastId;
    toastEl.className = `toast align-items-center text-white bg-${type === 'success' ? 'success' : type === 'error' ? 'danger' : 'primary'}`;
    toastEl.setAttribute('role', 'alert');
    toastEl.setAttribute('aria-live', 'assertive');
    toastEl.setAttribute('aria-atomic', 'true');
    
    // Create toast content
    toastEl.innerHTML = `
        <div class="d-flex">
            <div class="toast-body">
                ${message}
            </div>
            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
        </div>
    `;
    
    // Add toast to container
    toastContainer.appendChild(toastEl);
    
    // Initialize and show toast
    const toast = new bootstrap.Toast(toastEl, { autohide: true, delay: 5000 });
    toast.show();
    
    // Remove toast element when hidden
    toastEl.addEventListener('hidden.bs.toast', () => {
        toastEl.remove();
    });
}

// Xóa tất cả code liên quan đến việc kiểm tra và sửa lỗi hình ảnh liên tục
// Các hình ảnh sẽ tải bình thường mà không cần các đoạn code kiểm tra lặp lại
