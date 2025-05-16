// News Page Manager

document.addEventListener('DOMContentLoaded', function() {
    // Sample news data for demonstration
    const sampleNewsData = [
        {
            id: 1,
            title: "Elden Ring Shadow of the Erdtree DLC Gets Release Date",
            summary: "FromSoftware has finally announced the release date for the highly anticipated Elden Ring DLC, Shadow of the Erdtree. Players can expect new challenging bosses and expanded lore.",
            imageUrl: "/images/news/elden-ring.jpg",
            category: "New Releases",
            createdAt: "2023-06-15"
        },
        {
            id: 2,
            title: "PlayStation 6 Rumors: What We Know So Far",
            summary: "Sony might be working on their next-generation console. Rumors suggest improved ray tracing, 8K support, and backward compatibility with PS4 and PS5 games.",
            imageUrl: "/images/news/playstation.jpg",
            category: "Industry Updates",
            createdAt: "2023-06-12"
        },
        {
            id: 3,
            title: "Nintendo Switch 2 Expected to Launch Next Year",
            summary: "Nintendo is rumored to be working on the successor to the Switch. Here's everything we've gathered about the potential specs, features, and release window.",
            imageUrl: "/images/news/nintendo.jpg",
            category: "Upcoming Games",
            createdAt: "2023-06-10"
        },
        {
            id: 4,
            title: "The Last of Us Part III Reportedly in Development",
            summary: "Naughty Dog is said to be working on The Last of Us Part III, according to industry insiders. The game is expected to continue Ellie's story several years after the events of Part II.",
            imageUrl: "/images/news/last-of-us.jpg",
            category: "Industry Updates",
            createdAt: "2023-06-08"
        },
        {
            id: 5,
            title: "GTA 6 Trailer Breaks YouTube Records",
            summary: "Rockstar Games' Grand Theft Auto 6 trailer has become the most-watched video game trailer in YouTube history, accumulating over 100 million views in just 24 hours.",
            imageUrl: "/images/news/gta6.jpg",
            category: "Industry Updates",
            createdAt: "2023-06-05"
        },
        {
            id: 6,
            title: "Final Fantasy VII Rebirth Review: A Masterpiece",
            summary: "Our review of Final Fantasy VII Rebirth is in, and it's a masterpiece. The game builds upon the foundation of Remake while expanding the world and narrative in exciting ways.",
            imageUrl: "/images/news/ff7.jpg",
            category: "Game Reviews",
            createdAt: "2023-06-03"
        }
    ];
    
    // Generate fallback images for news items
    function createFallbackImages() {
        // Get all news images
        const newsImages = document.querySelectorAll('.news-card-image img');
        
        // For each image that fails to load, create a fallback
        newsImages.forEach(img => {
            img.addEventListener('error', function() {
                // Create canvas for fallback image
                const canvas = document.createElement('canvas');
                const ctx = canvas.getContext('2d');
                
                // Set dimensions
                canvas.width = 600;
                canvas.height = 320;
                
                // Create gradient background
                const gradient = ctx.createLinearGradient(0, 0, canvas.width, canvas.height);
                gradient.addColorStop(0, '#1f2937');
                gradient.addColorStop(1, '#374151');
                ctx.fillStyle = gradient;
                ctx.fillRect(0, 0, canvas.width, canvas.height);
                
                // Add some random shapes for visual interest
                for (let i = 0; i < 10; i++) {
                    ctx.beginPath();
                    ctx.arc(
                        Math.random() * canvas.width,
                        Math.random() * canvas.height,
                        Math.random() * 50 + 10,
                        0,
                        Math.PI * 2
                    );
                    ctx.fillStyle = `rgba(245, 158, 11, ${Math.random() * 0.2})`;
                    ctx.fill();
                }
                
                // Add text
                const title = img.alt || 'Gaming News';
                ctx.font = 'bold 24px Inter, sans-serif';
                ctx.fillStyle = 'white';
                ctx.textAlign = 'center';
                ctx.fillText('Gaming News', canvas.width / 2, canvas.height / 2);
                
                // Draw Metacritic-style icon
                ctx.beginPath();
                ctx.arc(canvas.width / 2, canvas.height / 2 - 60, 30, 0, Math.PI * 2);
                ctx.fillStyle = '#f59e0b';
                ctx.fill();
                
                ctx.font = 'bold 28px Inter, sans-serif';
                ctx.fillStyle = 'white';
                ctx.textAlign = 'center';
                ctx.textBaseline = 'middle';
                ctx.fillText('M', canvas.width / 2, canvas.height / 2 - 60);
                
                // Set as image source
                img.src = canvas.toDataURL('image/png');
            });
        });
    }
    
    // Handle category filters
    function setupCategoryFilters() {
        const categoryFilters = document.querySelectorAll('.news-category-filter');
        categoryFilters.forEach(filter => {
            filter.addEventListener('click', function() {
                // Remove active class from all filters
                categoryFilters.forEach(f => f.classList.remove('active'));
                // Add active class to clicked filter
                this.classList.add('active');
                
                // Filter news based on category
                const category = this.textContent.trim();
                filterNewsByCategory(category);
            });
        });
    }
    
    // Filter news by category
    function filterNewsByCategory(category) {
        const newsCards = document.querySelectorAll('.news-card');
        
        newsCards.forEach(card => {
            const cardCategory = card.querySelector('.news-card-category').textContent.trim();
            
            if (category === 'All' || cardCategory === category) {
                card.style.display = '';
            } else {
                card.style.display = 'none';
            }
        });
    }
    
    // Handle sort options
    function setupSortOptions() {
        const sortSelect = document.querySelector('.news-sort-select');
        if (sortSelect) {
            sortSelect.addEventListener('change', function() {
                const sortMethod = this.value;
                sortNews(sortMethod);
            });
        }
    }
    
    // Sort news based on selected method
    function sortNews(method) {
        const newsGrid = document.querySelector('.news-grid');
        const newsCards = Array.from(newsGrid.querySelectorAll('.news-card:not(.news-featured)'));
        
        newsCards.sort((a, b) => {
            const dateA = new Date(a.querySelector('.news-card-date').textContent);
            const dateB = new Date(b.querySelector('.news-card-date').textContent);
            
            if (method === 'newest') {
                return dateB - dateA;
            } else if (method === 'oldest') {
                return dateA - dateB;
            }
            
            return 0;
        });
        
        // Remove all cards except featured
        const featuredCard = newsGrid.querySelector('.news-featured');
        while (newsGrid.lastChild) {
            newsGrid.removeChild(newsGrid.lastChild);
        }
        
        // Add featured card back
        if (featuredCard) {
            newsGrid.appendChild(featuredCard);
        }
        
        // Add sorted cards
        newsCards.forEach(card => {
            newsGrid.appendChild(card);
        });
    }
    
    // Initialize
    createFallbackImages();
    setupCategoryFilters();
    setupSortOptions();
}); 