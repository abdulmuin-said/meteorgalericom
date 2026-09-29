using KanvasProje.Core.Varliklar;
using KanvasProje.Data;
using KanvasProje.Service.Helpers;
using KanvasProje.Web.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KanvasProje.Web.Data
{
    public static class DbSeeder
    {
        public static async Task VerileriYukle(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();

            var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var configuration = serviceScope.ServiceProvider.GetRequiredService<IConfiguration>();
            var context = serviceScope.ServiceProvider.GetRequiredService<KanvasDbContext>();

            var roleNames = AdminSecurityRoles.AllAdminRoles
                .Concat(new[] { AdminSecurityRoles.Uye })
                .Distinct(StringComparer.OrdinalIgnoreCase);

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var adminEmail = configuration["AdminSettings:Email"];
            var adminPassword = configuration["AdminSettings:Password"];
            var seedDefaultAdmin = configuration.GetValue<bool>("AdminSettings:SeedDefaultAdmin");

            if (seedDefaultAdmin && !string.IsNullOrWhiteSpace(adminEmail) && !string.IsNullOrWhiteSpace(adminPassword))
            {
                var adminUser = await userManager.FindByEmailAsync(adminEmail);

                if (adminUser == null)
                {
                    adminUser = new AppUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        AdSoyad = "Sistem Yöneticisi",
                        Sehir = "İstanbul",
                        EmailConfirmed = true
                    };

                    var createResult = await userManager.CreateAsync(adminUser, adminPassword);
                    if (!createResult.Succeeded)
                    {
                        adminUser = null;
                    }
                }

                if (adminUser != null)
                {
                    if (!await userManager.IsInRoleAsync(adminUser, AdminSecurityRoles.LegacyAdmin))
                    {
                        await userManager.AddToRoleAsync(adminUser, AdminSecurityRoles.LegacyAdmin);
                    }
                }
            }

            foreach (var user in userManager.Users.ToList())
            {
                var roles = await userManager.GetRolesAsync(user);
                if (roles.Count == 0)
                {
                    await userManager.AddToRoleAsync(user, AdminSecurityRoles.Uye);
                }
            }

            if (!await context.KargoFirmalari.IgnoreQueryFilters().AnyAsync(x => x.Kod == "aras"))
            {
                context.KargoFirmalari.Add(new KargoFirmasi
                {
                    Ad = "Aras Kargo",
                    Kod = "aras",
                    LogoUrl = "/aras-logo.svg",
                    Telefon = "444 25 52",
                    TakipUrl = "https://www.araskargo.com.tr/tr/online-islemler/kargo-takip",
                    GondericiUnvan = "MeteorGaleri",
                    GondericiAdres = "Merkez Mah. Sanat Sok. No:5 İstanbul / Türkiye",
                    GondericiTelefon = "0850 123 45 67",
                    AktifMi = true,
                    VarsayilanMi = true
                });
            }

            var mevcutKodlar = await context.UrunOzellikTanimlari
                .IgnoreQueryFilters()
                .Select(x => x.Kod)
                .ToListAsync();

            foreach (var definition in UrunOzellikCatalog.GetDefaultDefinitions())
            {
                if (mevcutKodlar.Contains(definition.Kod, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                var entity = new UrunOzellikTanimi
                {
                    OlusturulmaTarihi = DateTime.UtcNow,
                    SilindiMi = false
                };

                UrunOzellikCatalog.ApplySeed(entity, definition);
                context.UrunOzellikTanimlari.Add(entity);
            }

            await context.SaveChangesAsync();

            var jsonToCategoryMap = new Dictionary<string, string>
            {
                { "Şehir ve Mimari", "Sehir ve Mimari" },
                { "Manzara Temalı", "Manzara Temali" },
                { "Çiçekli ve Dekoratif", "Cicekli ve Dekoratif" },
                { "Hayvanlar Alemi", "Hayvanlar Alemi" },
                { "Modern & Soyut", "Modern ve Soyut" },
                { "Çocuk Odası", "Cocuk Odasi" },
                { "Farklı Tasarımlar", "Farkli Tasarimlar" },
                { "Dini ve Hat Sanatı", "Dini ve Hat Sanati" },
                { "Klasik Eserler", "Klasik Eserler" },
                { "Atatürk ve Türkiye", "Ataturk ve Turkiye" },
                { "Kadın Temalı", "Kadin Temali" },
                { "Yiyecek ve İçecek", "Yiyecek ve Icecek" },
                { "Osmanlı ve Tuğra", "Osmanli ve Tugra" },
                { "Soyut ve Sanatsal", "Soyut ve Sanatsal" }
            };

            var existingKategoriler = await context.Kategoriler.IgnoreQueryFilters().ToListAsync();
            if (!existingKategoriler.Any(x => x.Ad == "Şehir ve Mimari"))
            {
                int sira = 1;
                foreach (var kvp in jsonToCategoryMap)
                {
                    var kategori = new Kategori
                    {
                        Ad = kvp.Key,
                        Slug = kvp.Value.ToLower().Replace(" ", "-"),
                        KisaAciklama = "",
                        Aciklama = "",
                        SeoTitle = kvp.Key + " - MeteorGaleri",
                        SeoDescription = kvp.Key + " kategorisindeki kanvas tablolar",
                        Sira = sira,
                        AktifMi = true,
                        SilindiMi = false,
                        OlusturulmaTarihi = DateTime.UtcNow
                    };
                    context.Kategoriler.Add(kategori);
                    sira++;
                }
                await context.SaveChangesAsync();
            }

            var existingSlaytlar = await context.Slaytlar.IgnoreQueryFilters().ToListAsync();
            if (!existingSlaytlar.Any())
            {
                context.Slaytlar.AddRange(new List<Slayt>
                {
                    new Slayt
                    {
                        Baslik = "Yaşam Alanınıza Sanat Katın",
                        AltBaslik = "Seçkin Tablo Koleksiyonları",
                        Aciklama = "Evinizin ve ofisinizin havasını değiştirecek kanvas ve cam tablolar.",
                        ResimUrl = "/img/banner/slider-1.webp",
                        VideoUrl = "/video/slider1.mp4",
                        Tur = "Video",
                        Sira = 1,
                        AktifMi = true,
                        OlusturmaTarihi = DateTime.UtcNow
                    },
                    new Slayt
                    {
                        Baslik = "Modern ve Minimalist Çizgiler",
                        AltBaslik = "Yeni Nesil Duvar Sanatı",
                        Aciklama = "Göz alıcı detaylar ve yüksek kaliteli baskı teknolojisi.",
                        ResimUrl = "/img/banner/slider-2.webp",
                        VideoUrl = "/video/slider2.mp4",
                        Tur = "Video",
                        Sira = 2,
                        AktifMi = true,
                        OlusturmaTarihi = DateTime.UtcNow
                    },
                    new Slayt
                    {
                        Baslik = "Doğanın Canlı Renkleri",
                        AltBaslik = "Manzara Eserleri",
                        Aciklama = "Doğanın en güzel anlarını yansıtan tablolar.",
                        ResimUrl = "/img/banner/slider-3.webp",
                        VideoUrl = null,
                        Tur = "Görsel",
                        Sira = 3,
                        AktifMi = true,
                        OlusturmaTarihi = DateTime.UtcNow
                    }
                });
                await context.SaveChangesAsync();
            }
        }
    }
}
