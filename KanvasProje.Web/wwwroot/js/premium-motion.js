(function () {
    'use strict';

    var reduceMotion = window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    var mobileQuery = window.matchMedia ? window.matchMedia('(max-width: 767px)') : null;

    function ready(callback) {
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', callback, { once: true });
        } else {
            callback();
        }
    }

    function nextFrame(callback) {
        requestAnimationFrame(function () {
            requestAnimationFrame(callback);
        });
    }

    function revealNow(element) {
        element.classList.add('is-visible');
    }

    function initHeroMotion() {
        var hero = document.querySelector('[data-motion-hero]');
        if (!hero) return;

        hero.classList.add('motion-hero-prep');
        if (reduceMotion) {
            revealNow(hero);
            return;
        }

        nextFrame(function () {
            hero.classList.add('is-visible');
            window.setTimeout(function () {
                hero.classList.remove('motion-hero-prep', 'is-visible');
            }, 760);
        });
    }

    function initScrollReveal() {
        var sections = Array.prototype.slice.call(document.querySelectorAll('[data-motion-section]'));
        var staggerGroups = Array.prototype.slice.call(document.querySelectorAll('[data-motion-stagger]'));
        var items = [];

        sections.forEach(function (section) {
            section.classList.add('motion-reveal');
        });

        staggerGroups.forEach(function (group) {
            var groupItems = Array.prototype.slice.call(group.querySelectorAll('[data-motion-item]'));
            var delayStep = mobileQuery && mobileQuery.matches ? 28 : 55;

            groupItems.forEach(function (item, index) {
                item.classList.add('motion-stagger-item');
                item.style.setProperty('--motion-delay', Math.min(index * delayStep, 420) + 'ms');
                items.push(item);
            });
        });

        var targets = sections.concat(items);
        if (!targets.length) return;

        if (reduceMotion || !('IntersectionObserver' in window)) {
            targets.forEach(revealNow);
            return;
        }

        var observer = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (!entry.isIntersecting) return;

                entry.target.classList.add('is-animating');
                revealNow(entry.target);
                observer.unobserve(entry.target);

                window.setTimeout(function () {
                    entry.target.classList.remove('is-animating');
                }, 900);
            });
        }, {
            threshold: 0.12,
            rootMargin: '0px 0px -8% 0px'
        });

        targets.forEach(function (target) {
            observer.observe(target);
        });
    }

    function setMobileItemDelays(drawer) {
        var items = Array.prototype.slice.call(drawer.querySelectorAll('[data-mobile-nav-item]'));
        items.forEach(function (item, index) {
            item.style.setProperty('--mobile-nav-delay', Math.min(70 + (index * 28), 360) + 'ms');
        });

        Array.prototype.slice.call(drawer.querySelectorAll('[data-mobile-category]')).forEach(function (details) {
            Array.prototype.slice.call(details.querySelectorAll('[data-mobile-subitem]')).forEach(function (item, index) {
                item.style.setProperty('--mobile-sub-delay', Math.min(index * 22, 260) + 'ms');
            });
        });
    }

    function initMobileNavigation() {
        var drawer = document.getElementById('mobileNav');
        var backdrop = document.getElementById('mobileNavBackdrop');
        var closeTimer = null;

        if (!drawer || !backdrop) return;

        setMobileItemDelays(drawer);

        window.toggleMobileNav = function (open) {
            var shouldOpen = typeof open === 'boolean' ? open : drawer.classList.contains('hidden');
            window.clearTimeout(closeTimer);

            if (shouldOpen) {
                drawer.classList.remove('hidden');
                backdrop.classList.remove('hidden');
                document.documentElement.classList.add('overflow-hidden');
                document.body.classList.add('overflow-hidden');

                nextFrame(function () {
                    drawer.classList.add('is-open');
                    backdrop.classList.add('is-open');
            });
            // Adjust dropdown position to stay within viewport
            var parentLi = dropdown.closest('[data-category-menu-item]');
            if (parentLi) {
                parentLi.addEventListener('mouseenter', function () {
                    requestAnimationFrame(() => {
                        var vw = window.innerWidth;
                        var rect = dropdown.getBoundingClientRect();
                        var overflowLeft = rect.left < 0;
                        var overflowRight = rect.right > vw;
                        if (overflowLeft) {
                            dropdown.style.left = '0';
                            dropdown.style.right = '';
                            dropdown.style.transform = 'translateX(0)';
                        } else if (overflowRight) {
                            dropdown.style.left = '';
                            dropdown.style.right = '0';
                            dropdown.style.transform = 'translateX(0)';
                        } else {
                            dropdown.style.left = '50%';
                            dropdown.style.right = '';
                            dropdown.style.transform = 'translateX(-50%)';
                        }
                    });
                });
                parentLi.addEventListener('focusin', function () {
                    var vw = window.innerWidth;
                    var rect = dropdown.getBoundingClientRect();
                    var overflowLeft = rect.left < 0;
                    var overflowRight = rect.right > vw;
                    if (overflowLeft) {
                        dropdown.style.left = '0';
                        dropdown.style.right = '';
                        dropdown.style.transform = 'translateX(0)';
                    } else if (overflowRight) {
                        dropdown.style.left = '';
                        dropdown.style.right = '0';
                        dropdown.style.transform = 'translateX(0)';
                    } else {
                        dropdown.style.left = '50%';
                        dropdown.style.right = '';
                        dropdown.style.transform = 'translateX(-50%)';
                    }
                });
            }
                return;
            }

            drawer.classList.remove('is-open');
            backdrop.classList.remove('is-open');
            document.documentElement.classList.remove('overflow-hidden');
            document.body.classList.remove('overflow-hidden');

            closeTimer = window.setTimeout(function () {
                drawer.classList.add('hidden');
                backdrop.classList.add('hidden');
            }, reduceMotion ? 0 : 430);
        };
    }

    function initCategoryPreview() {
        var dropdowns = Array.prototype.slice.call(document.querySelectorAll('[data-category-dropdown]'));
        if (!dropdowns.length) return;

        dropdowns.forEach(function (dropdown) {
            var panel = dropdown.querySelector('[data-category-preview-panel]');
            if (!panel) return;

            var image = panel.querySelector('[data-category-preview-image]');
            var title = panel.querySelector('[data-category-preview-title]');
            var text = panel.querySelector('[data-category-preview-text]');
            var links = Array.prototype.slice.call(dropdown.querySelectorAll('[data-category-preview-title][href]'));
            var changeTimer = null;

            function ensureImage(src, alt) {
                if (!src) return;

                var media = panel.querySelector('.category-preview-media');
                if (!image && media) {
                    image = document.createElement('img');
                    image.setAttribute('data-category-preview-image', '');
                    image.className = 'h-full w-full object-cover';
                    image.loading = 'lazy';
                    image.decoding = 'async';
                    media.innerHTML = '';
                    media.appendChild(image);
                }

                if (image) {
                    image.src = src;
                    image.alt = alt || '';
                }
            }

            function updatePreview(link) {
                var src = link.getAttribute('data-category-preview-src') || '';
                var nextTitle = link.getAttribute('data-category-preview-title') || link.textContent.trim();
                var nextText = link.getAttribute('data-category-preview-text') || 'Bu koleksiyondaki tasarımları keşfedin.';

                window.clearTimeout(changeTimer);
                panel.classList.add('is-preview-changing');

                changeTimer = window.setTimeout(function () {
                    ensureImage(src, nextTitle);
                    if (title) title.textContent = nextTitle;
                    if (text) text.textContent = nextText;
                    panel.classList.remove('is-preview-changing');
                }, reduceMotion ? 0 : 120);
            }

            links.forEach(function (link) {
                link.addEventListener('mouseenter', function () { updatePreview(link); });
                link.addEventListener('focus', function () { updatePreview(link); });
            });
            // Adjust dropdown position to stay within viewport using fixed positioning
            var parentLi = dropdown.closest('[data-category-menu-item]');
            if (parentLi) {
                var originalLeft = dropdown.style.left;
                var originalTransform = dropdown.style.transform;
                function positionDropdown() {
                    requestAnimationFrame(() => {
                        var vw = window.innerWidth;
                        var rect = dropdown.getBoundingClientRect();
                        // Switch to fixed to escape hero video layer
                        dropdown.style.position = 'fixed';
                        dropdown.style.top = rect.top + 'px';
                        // Determine horizontal overflow
                        if (rect.left < 0) {
                            dropdown.style.left = '0px';
                            dropdown.style.right = '';
                            dropdown.style.transform = 'translateX(0)';
                        } else if (rect.right > vw) {
                            dropdown.style.left = '';
                            dropdown.style.right = '0px';
                            dropdown.style.transform = 'translateX(0)';
                        } else {
                            dropdown.style.left = rect.left + 'px';
                            dropdown.style.right = '';
                            dropdown.style.transform = 'translateX(0)';
                        }
                    });
                }
                function resetPosition() {
                    dropdown.style.position = '';
                    dropdown.style.top = '';
                    dropdown.style.left = originalLeft;
                    dropdown.style.right = '';
                    dropdown.style.transform = originalTransform;
                }
                parentLi.addEventListener('mouseenter', positionDropdown);
                parentLi.addEventListener('focusin', positionDropdown);
                parentLi.addEventListener('mouseleave', resetPosition);
                parentLi.addEventListener('focusout', resetPosition);
            }
        });
    }

    function initDesktopNavOverflow() {
        var nav = document.querySelector('[data-desktop-nav]');
        var list = document.querySelector('[data-desktop-nav-list]');
        if (!nav || !list) return;

        function update() {
            var items = list.children;
            var lastItem = items[items.length - 1];
            var firstItem = items[0];
            var parent = nav.parentElement;
            var parentRect = parent.getBoundingClientRect();
            var lastRect = lastItem.getBoundingClientRect();
            var firstRect = firstItem.getBoundingClientRect();
            
            var overflowsLeft = firstRect.left < parentRect.left;
            var overflowsRight = lastRect.right > parentRect.right;

            if (overflowsLeft || overflowsRight) {
                list.classList.remove('justify-center');
                list.classList.add('justify-start');
            } else {
                list.classList.remove('justify-start');
                list.classList.add('justify-center');
            }
        }

        update();
        var timer = null;
        window.addEventListener('resize', function () {
            window.clearTimeout(timer);
            timer = window.setTimeout(update, 80);
        });
    }

    ready(function () {
        initHeroMotion();
        initScrollReveal();
        initMobileNavigation();
        initCategoryPreview();
        initDesktopNavOverflow();
    });
})();
