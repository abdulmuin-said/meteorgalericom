/**
 * Canvasia Premium Interactions
 * 3D Tilt, Fly-to-Cart, Like Bounce, Gallery Mode
 */

(function () {
    'use strict';

    // ==========================================
    // 1. 3D TILT EFFECT (Product Cards)
    // ==========================================
    function initTiltEffect() {
        // Touch cihazlarda tilt yok
        if ('ontouchstart' in window) return;

        document.addEventListener('mousemove', function (e) {
            const cards = document.querySelectorAll('.pro-product-card');
            cards.forEach(function (card) {
                const rect = card.getBoundingClientRect();
                const isHovering =
                    e.clientX >= rect.left &&
                    e.clientX <= rect.right &&
                    e.clientY >= rect.top &&
                    e.clientY <= rect.bottom;

                if (isHovering) {
                    const x = e.clientX - rect.left;
                    const y = e.clientY - rect.top;
                    const centerX = rect.width / 2;
                    const centerY = rect.height / 2;

                    const rotateX = ((y - centerY) / centerY) * -6; // Max ±6°
                    const rotateY = ((x - centerX) / centerX) * 6;

                    card.style.transform = 'perspective(800px) rotateX(' + rotateX + 'deg) rotateY(' + rotateY + 'deg) scale3d(1.02, 1.02, 1.02)';

                    // Dynamic light reflection
                    const glare = card.querySelector('.card-glare');
                    if (glare) {
                        const percentX = (x / rect.width) * 100;
                        const percentY = (y / rect.height) * 100;
                        glare.style.background = 'radial-gradient(circle at ' + percentX + '% ' + percentY + '%, rgba(255,255,255,0.15) 0%, transparent 60%)';
                        glare.style.opacity = '1';
                    }
                }
            });
        });

        // Reset on mouse leave
        document.addEventListener('mouseover', function (e) {
            const card = e.target.closest('.pro-product-card');
            if (!card) {
                document.querySelectorAll('.pro-product-card').forEach(function (c) {
                    c.style.transform = '';
                    const glare = c.querySelector('.card-glare');
                    if (glare) glare.style.opacity = '0';
                });
            }
        });

        // Inject glare overlay div into each product card
        document.querySelectorAll('.pro-product-card').forEach(function (card) {
            if (!card.querySelector('.card-glare')) {
                var glare = document.createElement('div');
                glare.className = 'card-glare';
                card.appendChild(glare);
            }
        });
    }

    // ==========================================
    // 2. FLY-TO-CART ANIMATION
    // ==========================================
    window.flyToCart = function (btnElement) {
        var card = btnElement.closest('.pro-product-card');
        if (!card) return;

        var img = card.querySelector('.pro-img');
        if (!img) return;

        // Find cart icon in header
        var cartIcon = document.querySelector('.header-cart-count') ||
            document.querySelector('.header-cart .fa-bag-shopping') ||
            document.querySelector('.mobile-bottom-link .fa-bag-shopping') ||
            document.querySelector('.desktop-cart-count') ||
            document.querySelector('.desktop-nav-item .fa-shopping-bag') ||
            document.querySelector('.cart-indicator') ||
            document.querySelector('.fa-shopping-bag');

        if (!cartIcon) return;

        var imgRect = img.getBoundingClientRect();
        var cartRect = cartIcon.getBoundingClientRect();

        // Create flying clone
        var flyingImg = document.createElement('img');
        flyingImg.src = img.src;
        flyingImg.className = 'flying-cart-item';
        flyingImg.style.cssText =
            'position:fixed;' +
            'top:' + imgRect.top + 'px;' +
            'left:' + imgRect.left + 'px;' +
            'width:' + imgRect.width + 'px;' +
            'height:' + imgRect.height + 'px;' +
            'object-fit:contain;' +
            'z-index:9999;' +
            'pointer-events:none;' +
            'border-radius:10px;' +
            'box-shadow:0 5px 20px rgba(0,0,0,0.3);' +
            'transition: all 0.75s cubic-bezier(0.2, 1, 0.3, 1);';

        document.body.appendChild(flyingImg);

        // Force reflow before starting animation
        flyingImg.offsetHeight;

        // Animate to cart
        var targetX = cartRect.left + cartRect.width / 2 - 20;
        var targetY = cartRect.top + cartRect.height / 2 - 20;

        flyingImg.style.top = targetY + 'px';
        flyingImg.style.left = targetX + 'px';
        flyingImg.style.width = '40px';
        flyingImg.style.height = '40px';
        flyingImg.style.opacity = '0.3';
        flyingImg.style.borderRadius = '50%';

        // Cart icon bounce on arrival
        setTimeout(function () {
            if (cartIcon) {
                cartIcon.classList.add('cart-bounce');
                setTimeout(function () {
                    cartIcon.classList.remove('cart-bounce');
                }, 600);
            }
            flyingImg.remove();
        }, 750);
    };

    // ==========================================
    // 3. LIKE BUTTON BOUNCE
    // ==========================================
    function initLikeBounce() {
        document.addEventListener('click', function (e) {
            var likeBtn = e.target.closest('.like-btn');
            if (!likeBtn) return;

            likeBtn.classList.add('like-pop');

            // Particle burst
            for (var i = 0; i < 6; i++) {
                var particle = document.createElement('span');
                particle.className = 'heart-particle';
                particle.innerHTML = '❤';
                particle.style.cssText =
                    '--angle:' + (i * 60) + 'deg;' +
                    '--delay:' + (i * 0.05) + 's;';
                likeBtn.appendChild(particle);
            }

            setTimeout(function () {
                likeBtn.classList.remove('like-pop');
                likeBtn.querySelectorAll('.heart-particle').forEach(function (p) { p.remove(); });
            }, 700);
        });
    }

    // ==========================================
    // 4. LEGACY THEME CLEANUP
    // ==========================================
    window.toggleGalleryMode = function () {
        document.documentElement.removeAttribute('data-theme');
        localStorage.removeItem('canvasia-theme');
    };

    function applySavedTheme() {
        document.documentElement.removeAttribute('data-theme');
        localStorage.removeItem('canvasia-theme');
    }

    // ==========================================
    // 5. SCROLL REVEAL (Bonus micro-interaction)
    // ==========================================
    function initScrollReveal() {
        var observer = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    entry.target.classList.add('revealed');
                    observer.unobserve(entry.target);
                }
            });
        }, { threshold: 0.1, rootMargin: '0px 0px -50px 0px' });

        document.querySelectorAll('.product-item').forEach(function (item) {
            item.classList.add('scroll-reveal');
            observer.observe(item);
        });
    }

    // ==========================================
    // 6. TOAST NOTIFICATION SYSTEM
    // ==========================================
    window.showToast = function (title, message, type = 'success') {
        // Container var mı kontrol et, yoksa oluştur
        let container = document.querySelector('.toast-container');
        if (!container) {
            container = document.createElement('div');
            container.className = 'toast-container';
            document.body.appendChild(container);
        }

        // İkon belirle
        let iconClass = 'fas fa-check-circle';
        if (type === 'error') iconClass = 'fas fa-times-circle';
        if (type === 'info') iconClass = 'fas fa-info-circle';
        if (type === 'warning') iconClass = 'fas fa-exclamation-triangle';

        // Toast elementini oluştur
        const toast = document.createElement('div');
        toast.className = `toast-message toast-${type}`;
        toast.innerHTML = `
            <div class="toast-icon"><i class="${iconClass}"></i></div>
            <div class="toast-content">
                <div class="toast-title">${title}</div>
                <div class="toast-text">${message}</div>
            </div>
        `;

        // Container'a ekle (En üste)
        container.appendChild(toast);

        // Animasyon için frame bekle
        requestAnimationFrame(() => {
            toast.classList.add('show');
        });

        // Ses efekti (Opsiyonel, kısa ve hoş bir ses)
        // const audio = new Audio('/sounds/pop.mp3');
        // audio.volume = 0.2;
        // audio.play().catch(e => {}); 

        // 4 saniye sonra kaldır
        setTimeout(() => {
            toast.classList.remove('show');
            setTimeout(() => {
                toast.remove();
            }, 400); // Transition süresi kadar bekle
        }, 4000);
    };

    // ==========================================
    // 7. WELCOME NOTIFICATION
    // ==========================================
    function initWelcomeMessage() {
        const hasVisited = localStorage.getItem('storefront-visited');

        if (!hasVisited) {
            setTimeout(() => {
                window.showToast('Hos Geldiniz', 'Yeni urunleri ve kampanyalari kesfetmeye baslayin.', 'info');
                localStorage.setItem('storefront-visited', 'true');
            }, 2000); // 2 saniye sonra göster
        } else {
            // Belki geri dönen kullanıcıya "Tekrar hoşgeldin" diyebiliriz (Session bazlı)
            const sessionVisited = sessionStorage.getItem('session-welcome');
            if (!sessionVisited) {
                // Sadece çok şık ve rahatsız etmeyen bir hatırlatma
                // window.showToast('Tekrar Hoşgeldiniz', 'Sizi tekrar görmek güzel!', 'success');
                sessionStorage.setItem('session-welcome', 'true');
            }
        }
    }

    // ==========================================
    // 8. GLOBAL ADD TO CART & FAVORITES
    // ==========================================

    // Favori İşlemleri
    window.toggleLike = window.toggleFavorite = function (urunId, btn) {
        // UI Update
        const icon = btn.querySelector('i');
        const isLiked = btn.classList.contains('liked');

        if (isLiked) {
            btn.classList.remove('liked');
            icon.classList.remove('fas');
            icon.classList.add('far');
            window.showToast('Favorilerden Çıkarıldı', 'Ürün favorilerinizden kaldırıldı.', 'info');
        } else {
            btn.classList.add('liked');
            icon.classList.remove('far');
            icon.classList.add('fas');

            // Like animation
            btn.classList.add('like-pop');
            setTimeout(() => btn.classList.remove('like-pop'), 300);

            window.showToast('Favorilere Eklendi', 'Ürün favorilerinize eklendi.', 'success');
        }

        // Backend'e favori toggle isteği
        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
        fetch('/Favori/Toggle', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': token || ''
            },
            body: 'urunId=' + urunId
        })
            .then(r => r.json())
            .then(data => {
                if (!data.success) {
                    // Giriş yapılmamışsa UI'ı geri al ve uyar
                    if (isLiked) {
                        btn.classList.add('liked');
                        icon.classList.remove('far');
                        icon.classList.add('fas');
                    } else {
                        btn.classList.remove('liked');
                        icon.classList.remove('fas');
                        icon.classList.add('far');
                    }
                    window.showToast('Uyarı', data.message || 'Lütfen giriş yapınız.', 'warning');
                }
            })
            .catch(() => {
                // Hata durumunda UI'ı geri al
                if (isLiked) {
                    btn.classList.add('liked');
                    icon.classList.remove('far');
                    icon.classList.add('fas');
                } else {
                    btn.classList.remove('liked');
                    icon.classList.remove('fas');
                    icon.classList.add('far');
                }
            });
    };

    // Global Sepete Ekle Fonksiyonu
    window.addToCart = function (btnElement, urunId, isDetail = false) {
        // Button ve Icon Seçimi
        let btnWrapper = btnElement.closest('.neon-btn-wrapper') || btnElement;
        let btnInner = btnWrapper.querySelector('.neon-btn-inner') || btnElement;
        let icon = btnInner.querySelector('i');
        const originalHtml = btnInner.innerHTML;

        // Veri Hazırlığı
        let payload = { UrunId: urunId, Adet: 1 };
        let url = '/Sepet/Ekle?UrunId=' + urunId + '&Adet=1';

        // Detay sayfasından geliyorsa varyant ve adet al
        if (isDetail) {
            const form = document.getElementById('addToCartForm');
            if (form) {
                const formData = new FormData(form);
                payload.SecenekId = formData.get('SecenekId');
                payload.Adet = formData.get('Adet');
                url = `/Sepet/Ekle?UrunId=${payload.UrunId}&Adet=${payload.Adet}`;
                if (payload.SecenekId) url += `&SecenekId=${payload.SecenekId}`;
            }
        }

        // 1. Loading Durumu
        btnInner.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Ekleniyor...';
        btnWrapper.style.pointerEvents = 'none';

        // 2. İstek Gönder - CSRF token ekle
        const token = document.querySelector('#addToCartForm input[name="__RequestVerificationToken"]')?.value
            || document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
        fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            }
        })
            .then(response => {
                if (!response.ok) throw new Error('Sunucu hatası');
                return response.json();
            })
            .then(data => {
                if (data.success) {
                    // BAŞARILI
                    btnInner.innerHTML = '<i class="fas fa-check"></i> Eklendi';
                    btnInner.style.background = '#4CAF50';
                    btnInner.style.color = '#fff'; // Yazı rengi beyaz

                    // Toast
                    window.showToast('Sepete Eklendi 🛒', 'Ürün başarıyla sepetinize eklendi.', 'success');

                    // Uçma Animasyonu (Listeleme sayfalarındaysa)
                    if (!isDetail && window.flyToCart) {
                        window.flyToCart(btnWrapper);
                    }

                    // Sepet Sayacını Güncelle
                    const indicators = document.querySelectorAll('.cart-indicator, .desktop-cart-count');
                    indicators.forEach(ind => {
                        let count = parseInt(ind.textContent) || 0;
                        ind.textContent = count + parseInt(payload.Adet);
                    });

                    // 2 Saniye sonra reset
                    setTimeout(() => {
                        btnInner.innerHTML = originalHtml;
                        btnInner.style.background = '';
                        btnInner.style.color = '';
                        btnWrapper.style.pointerEvents = 'auto';
                    }, 2000);

                } else {
                    // HATA (Stok yok vb.)
                    window.showToast('Hata', data.message, 'error');
                    btnInner.innerHTML = originalHtml;
                    btnWrapper.style.pointerEvents = 'auto';
                }
            })
            .catch(error => {
                console.error(error);
                window.showToast('Hata', 'Bir sorun oluştu.', 'error');
                btnInner.innerHTML = '<i class="fas fa-times"></i> Hata';
                btnInner.style.background = '#F44336';
                setTimeout(() => {
                    btnInner.innerHTML = originalHtml;
                    btnInner.style.background = '';
                    btnWrapper.style.pointerEvents = 'auto';
                }, 2000);
            });
    };

    // ==========================================
    // INIT
    // ==========================================
    applySavedTheme();

    document.addEventListener('DOMContentLoaded', function () {
        initTiltEffect();
        initLikeBounce();
        initScrollReveal();
        initWelcomeMessage();

        // Update gallery mode button state
        var saved = localStorage.getItem('canvasia-theme');
        if (saved === 'gallery') {
            var btn = document.getElementById('galleryModeBtn');
            if (btn) {
                var icon = btn.querySelector('i');
                if (icon) icon.className = 'fas fa-sun';
                var label = btn.querySelector('.gallery-mode-label');
                if (label) label.textContent = 'Aydınlık Mod';
            }
            // Mobile toggle icon
            var mobileBtn = document.querySelector('.mobile-gallery-toggle');
            if (mobileBtn) {
                var mobileIcon = mobileBtn.querySelector('i');
                if (mobileIcon) mobileIcon.className = 'fas fa-sun';
            }
        }

        // Detay Sayfası Butonu Bağlama
        const detailBtn = document.getElementById('ajaxSepetBtn');
        if (detailBtn) {
            detailBtn.addEventListener('click', function (e) {
                e.preventDefault(); // Form submit engelle
                const urunId = document.querySelector('input[name="UrunId"]').value;
                window.addToCart(this, urunId, true);
            });
        }
    });
})();
