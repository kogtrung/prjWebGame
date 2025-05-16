document.addEventListener('DOMContentLoaded', function() {
    // Tạo placeholder cho ảnh game nếu chưa có
    createGamePlaceholder();
    
    // Xử lý highlight cho từ khóa tìm kiếm
    highlightSearchTerms();
    
    // Xử lý form tìm kiếm
    setupSearchForm();
    
    // Thiết lập auto-suggest cho tìm kiếm
    setupAutoSuggest();
});

// Tạo placeholder cho ảnh game
function createGamePlaceholder() {
    // Kiểm tra xem đã có sẵn placeholder chưa
    if (!document.querySelector('img[src="/images/game-placeholder.jpg"]')) {
        const canvas = document.createElement('canvas');
        canvas.width = 400;
        canvas.height = 300;
        const ctx = canvas.getContext('2d');
        
        // Vẽ nền gradient
        const gradient = ctx.createLinearGradient(0, 0, 400, 300);
        gradient.addColorStop(0, '#2c3e50');
        gradient.addColorStop(1, '#34495e');
        ctx.fillStyle = gradient;
        ctx.fillRect(0, 0, 400, 300);
        
        // Vẽ họa tiết
        ctx.fillStyle = 'rgba(255, 255, 255, 0.05)';
        for (let i = 0; i < 10; i++) {
            const x = Math.random() * 400;
            const y = Math.random() * 300;
            const size = 30 + Math.random() * 50;
            ctx.beginPath();
            ctx.arc(x, y, size, 0, Math.PI * 2);
            ctx.fill();
        }
        
        // Vẽ biểu tượng controller
        ctx.strokeStyle = 'rgba(255, 255, 255, 0.7)';
        ctx.lineWidth = 3;
        
        // Controller body
        ctx.beginPath();
        ctx.ellipse(200, 150, 70, 45, 0, 0, Math.PI * 2);
        ctx.stroke();
        
        // D-pad
        ctx.beginPath();
        ctx.moveTo(160, 150);
        ctx.lineTo(180, 150);
        ctx.moveTo(170, 140);
        ctx.lineTo(170, 160);
        ctx.stroke();
        
        // Buttons
        ctx.beginPath();
        ctx.arc(230, 140, 8, 0, Math.PI * 2);
        ctx.stroke();
        ctx.beginPath();
        ctx.arc(245, 155, 8, 0, Math.PI * 2);
        ctx.stroke();
        
        // Thêm text
        ctx.font = 'bold 20px Arial';
        ctx.fillStyle = 'rgba(255, 255, 255, 0.8)';
        ctx.textAlign = 'center';
        ctx.textBaseline = 'bottom';
        ctx.fillText('No Image Available', 200, 240);
        
        // Chuyển canvas thành data URL và lưu vào localStorage
        const dataUrl = canvas.toDataURL('image/jpeg');
        localStorage.setItem('gamePlaceholder', dataUrl);
        
        // Cập nhật tất cả ảnh lỗi
        document.querySelectorAll('img.game-image[onerror]').forEach(img => {
            img.onerror = function() {
                this.onerror = null;
                this.src = dataUrl;
            };
        });
    }
}

// Highlight từ khóa tìm kiếm trong kết quả
function highlightSearchTerms() {
    const searchTerm = document.querySelector('.search-term')?.textContent;
    if (!searchTerm) return;
    
    const terms = searchTerm.toLowerCase().split(' ').filter(term => term.length > 2);
    if (terms.length === 0) return;
    
    // Highlight trong tiêu đề game
    document.querySelectorAll('.card-title').forEach(element => {
        let html = element.innerHTML;
        terms.forEach(term => {
            const regex = new RegExp(`(${term})`, 'gi');
            html = html.replace(regex, '<span class="highlight">$1</span>');
        });
        element.innerHTML = html;
    });
    
    // Highlight trong mô tả game
    document.querySelectorAll('.game-description').forEach(element => {
        let html = element.innerHTML;
        terms.forEach(term => {
            const regex = new RegExp(`(${term})`, 'gi');
            html = html.replace(regex, '<span class="highlight">$1</span>');
        });
        element.innerHTML = html;
    });
}

// Thiết lập form tìm kiếm
function setupSearchForm() {
    // Xử lý tự động tìm kiếm khi ngừng gõ
    const searchInput = document.querySelector('input[name="q"]');
    if (searchInput) {
        let typingTimer;
        const doneTypingInterval = 1000; // 1 giây sau khi ngừng gõ
        
        searchInput.addEventListener('keyup', (e) => {
            // Không gửi form nếu có auto-suggest
            if (document.querySelector('.search-suggestions') && 
                document.querySelector('.search-suggestions').classList.contains('show')) {
                return;
            }
            
            // Xử lý Enter để submit form
            if (e.key === 'Enter') {
                clearTimeout(typingTimer);
                document.querySelector('form').submit();
                return;
            }
            
            clearTimeout(typingTimer);
            if (searchInput.value.length > 2) {
                typingTimer = setTimeout(() => {
                    // Chỉ tự động submit form ở trang kết quả tìm kiếm
                    if (window.location.href.includes('/Home/Search')) {
                        document.querySelector('form').submit();
                    }
                }, doneTypingInterval);
            }
        });
        
        // Xóa timer khi người dùng tiếp tục gõ
        searchInput.addEventListener('keydown', () => {
            clearTimeout(typingTimer);
        });
    }
}

// Thiết lập auto-suggest cho tìm kiếm
function setupAutoSuggest() {
    const searchInput = document.querySelector('input[name="q"]');
    if (!searchInput) return;
    
    // Tạo container cho gợi ý
    const suggestionsContainer = document.createElement('div');
    suggestionsContainer.className = 'search-suggestions';
    
    // Thêm vào DOM
    searchInput.parentNode.style.position = 'relative';
    searchInput.parentNode.appendChild(suggestionsContainer);
    
    let suggestTimer;
    const suggestDelay = 300; // 300ms sau khi ngừng gõ
    
    // Xử lý khi gõ
    searchInput.addEventListener('input', () => {
        clearTimeout(suggestTimer);
        
        if (searchInput.value.length < 2) {
            suggestionsContainer.classList.remove('show');
            return;
        }
        
        suggestTimer = setTimeout(() => {
            fetchSuggestions(searchInput.value, suggestionsContainer);
        }, suggestDelay);
    });
    
    // Đóng suggestions khi click bên ngoài
    document.addEventListener('click', (e) => {
        if (!searchInput.contains(e.target) && !suggestionsContainer.contains(e.target)) {
            suggestionsContainer.classList.remove('show');
        }
    });
}

// Lấy gợi ý tìm kiếm
function fetchSuggestions(query, container) {
    // Giả lập dữ liệu gợi ý (trong thực tế, đây sẽ là AJAX request đến server)
    const mockData = {
        games: [
            { id: 1, title: 'God of War Ragnarök', metascore: 94, platform: 'PlayStation' },
            { id: 2, title: 'Elden Ring', metascore: 96, platform: 'PC, PlayStation, Xbox' },
            { id: 3, title: 'The Legend of Zelda: Tears of the Kingdom', metascore: 96, platform: 'Nintendo' },
            { id: 4, title: 'Baldur\'s Gate 3', metascore: 97, platform: 'PC, PlayStation' }
        ],
        news: [
            { id: 1, title: 'Gaming Industry Report 2024', date: 'Apr 2, 2025' },
            { id: 2, title: 'Next Generation Console Rumors', date: 'Mar 25, 2025' }
        ]
    };
    
    // Lọc dữ liệu dựa trên query
    const queryLower = query.toLowerCase();
    
    const filteredGames = mockData.games.filter(game => 
        game.title.toLowerCase().includes(queryLower) || 
        game.platform.toLowerCase().includes(queryLower)
    );
    
    const filteredNews = mockData.news.filter(news => 
        news.title.toLowerCase().includes(queryLower)
    );
    
    // Tạo HTML cho các gợi ý
    let html = '';
    
    if (filteredGames.length > 0) {
        html += '<div class="suggest-category">Games</div>';
        filteredGames.forEach(game => {
            html += `
                <div class="suggestion-item" data-id="${game.id}" data-type="game">
                    <div class="item-title">${game.title}</div>
                    <div class="item-meta">
                        <span class="metascore-chip ${game.metascore >= 90 ? 'high' : (game.metascore >= 75 ? 'medium' : 'low')}">${game.metascore}</span>
                        <span class="item-type">${game.platform}</span>
                    </div>
                </div>
            `;
        });
    }
    
    if (filteredNews.length > 0) {
        html += '<div class="suggest-category">News</div>';
        filteredNews.forEach(news => {
            html += `
                <div class="suggestion-item" data-id="${news.id}" data-type="news">
                    <div class="item-title">${news.title}</div>
                    <div class="item-meta">${news.date}</div>
                </div>
            `;
        });
    }
    
    if (filteredGames.length === 0 && filteredNews.length === 0) {
        html = '<div class="suggestion-item">No results found. Press Enter to search.</div>';
    }
    
    // Hiển thị gợi ý
    container.innerHTML = html;
    container.classList.add('show');
    
    // Xử lý khi click vào gợi ý
    container.querySelectorAll('.suggestion-item[data-id]').forEach(item => {
        item.addEventListener('click', () => {
            const id = item.getAttribute('data-id');
            const type = item.getAttribute('data-type');
            
            // Chuyển hướng dựa trên loại gợi ý
            window.location.href = type === 'game' 
                ? `/Game/Details/${id}` 
                : `/News/Details/${id}`;
        });
    });
    
    // Thêm CSS cho metascore-chip
    const style = document.createElement('style');
    style.textContent = `
        .metascore-chip {
            display: inline-block;
            min-width: 25px;
            height: 25px;
            line-height: 25px;
            text-align: center;
            color: white;
            font-weight: bold;
            border-radius: 3px;
            margin-right: 8px;
            padding: 0 4px;
            font-size: 0.8rem;
        }
        .metascore-chip.high { background-color: #6c3; }
        .metascore-chip.medium { background-color: #fc3; }
        .metascore-chip.low { background-color: #f00; }
    `;
    document.head.appendChild(style);
}

// Thêm CSS cho highlight
document.addEventListener('DOMContentLoaded', function() {
    const style = document.createElement('style');
    style.textContent = `
        .highlight {
            background-color: rgba(255, 255, 0, 0.3);
            padding: 0 2px;
            border-radius: 2px;
        }
    `;
    document.head.appendChild(style);
}); 