-- ============================================================
-- MeteorGaleri DB Kurulum Scripti
-- Railway dump import SONRASI çalıştırılacak
-- ============================================================

-- 1. MÜŞTERİ VERİLERİNİ TEMİZLE (ürünler/yorumlar kalır)
-- --------------------------------------------------------

-- Bağımlı tablolar önce
DELETE FROM "SiparisDetaylari";
DELETE FROM "IadeTalepleri";
DELETE FROM "Siparisler";
DELETE FROM "SepetItems";
DELETE FROM "Sepetler";
DELETE FROM "Adresler";
DELETE FROM "BultenAbonelikleri";
DELETE FROM "ZiyaretciLoglari";
DELETE FROM "IletisimMesajlari";

-- 2. KULLANICILARI TEMİZLE (admin dışındakiler)
-- Önce AspNetUserRoles ve AspNetUserClaims temizle
DELETE FROM "AspNetUserRoles" WHERE "UserId" NOT IN (
    SELECT ur."UserId" FROM "AspNetUserRoles" ur
    INNER JOIN "AspNetRoles" r ON r."Id" = ur."RoleId"
    WHERE r."Name" IN ('SuperAdmin','Admin','LegacyAdmin','SiparisYoneticisi','UrunYoneticisi')
);
DELETE FROM "AspNetUserClaims" WHERE "UserId" NOT IN (
    SELECT ur."UserId" FROM "AspNetUserRoles" ur
    INNER JOIN "AspNetRoles" r ON r."Id" = ur."RoleId"
    WHERE r."Name" IN ('SuperAdmin','Admin','LegacyAdmin','SiparisYoneticisi','UrunYoneticisi')
);
DELETE FROM "AspNetUserLogins" WHERE "UserId" NOT IN (
    SELECT ur."UserId" FROM "AspNetUserRoles" ur
    INNER JOIN "AspNetRoles" r ON r."Id" = ur."RoleId"
    WHERE r."Name" IN ('SuperAdmin','Admin','LegacyAdmin','SiparisYoneticisi','UrunYoneticisi')
);
DELETE FROM "AspNetUserTokens" WHERE "UserId" NOT IN (
    SELECT ur."UserId" FROM "AspNetUserRoles" ur
    INNER JOIN "AspNetRoles" r ON r."Id" = ur."RoleId"
    WHERE r."Name" IN ('SuperAdmin','Admin','LegacyAdmin','SiparisYoneticisi','UrunYoneticisi')
);

-- Müşteri kullanıcıları sil
DELETE FROM "AspNetUsers" WHERE "Id" NOT IN (
    SELECT ur."UserId" FROM "AspNetUserRoles" ur
    INNER JOIN "AspNetRoles" r ON r."Id" = ur."RoleId"
    WHERE r."Name" IN ('SuperAdmin','Admin','LegacyAdmin','SiparisYoneticisi','UrunYoneticisi')
);

-- 3. YORUMLARDAKİ MARKA İZLERİNİ TEMİZLE
-- Yorumlar kalıyor ama içerisinde "Canvasia" geçiyorsa güncelle
UPDATE "Yorumlar" SET "Yorum" = REPLACE("Yorum", 'Canvasia', 'MeteorGaleri')
WHERE "Yorum" ILIKE '%Canvasia%';
UPDATE "Yorumlar" SET "Yorum" = REPLACE("Yorum", 'CANVASİA', 'METEORGALERİ')
WHERE "Yorum" ILIKE '%canvasia%';

-- 4. SİTE AYARLARINI GÜNCELLE
-- --------------------------------------------------------
UPDATE "SiteAyarlari" SET
    "SiteAdi"          = 'MeteorGaleri',
    "MarkaAdi"         = 'MeteorGaleri',
    "SiteBasligi"      = 'MeteorGaleri - Online Dekorasyon Mağazası',
    "BaseUrl"          = 'https://www.meteorgaleri.com',
    "Email"            = 'meteor_medya@hotmail.com',
    "BildirimAliciEmail" = 'meteor_medya@hotmail.com',
    "TemaRengi"        = '#C0392B',
    "MetaTitle"        = 'MeteorGaleri - Premium Kanvas Tablo ve Duvar Dekorasyonu',
    "MetaDescription"  = 'MeteorGaleri; kanvas tablo, cam tablo, duvar dekorasyonu ve yaşam alanlarına özel 14.000+ ürün sunar.',
    "MetaKeywords"     = 'kanvas tablo, cam tablo, duvar dekorasyonu, duvar sanatı, tablo, dekorasyon, MeteorGaleri',
    "VarsayilanSosyalPaylasimGorseliUrl" = '/EmailTemplates/meteorgaleri-logo.png',
    "FooterAciklamasi" = 'Premium kanvas tablo ve duvar dekorasyonu. 14.000+ ürün seçeneği.'
WHERE "Id" = 1;

-- 5. KARGO FİRMASI GÜNCELLE
UPDATE "KargoFirmalari" SET "GondericiUnvan" = 'MeteorGaleri'
WHERE "GondericiUnvan" = 'Canvasia';

-- 6. KATEGORİ SEO BAŞLIKLARINI GÜNCELLE
UPDATE "Kategoriler" SET
    "SeoTitle" = REPLACE("SeoTitle", 'Canvasia', 'MeteorGaleri'),
    "SeoDescription" = REPLACE("SeoDescription", 'Canvasia', 'MeteorGaleri')
WHERE "SeoTitle" ILIKE '%Canvasia%' OR "SeoDescription" ILIKE '%Canvasia%';

-- 7. KURUMSAL SAYFALARDAKI MARKA İZLERİ
UPDATE "KurumsalSayfalar" SET
    "Icerik" = REPLACE("Icerik", 'Canvasia', 'MeteorGaleri')
WHERE "Icerik" ILIKE '%Canvasia%';

-- 8. YENİ ADMİN HESABI OLUŞTUR
-- NOT: Bu INSERT'i SADECE AspNetUsers tablosuna Admin eklemek için kullan.
-- Şifre hash'i "MeteorAdmin2024!" için BCrypt. Gerçek deploy'da
-- appsettings.json > AdminSettings ile seed et (SeedDefaultAdmin: true).
-- Aşağıdaki satırı aktif etmek yerine secrets.json üzerinden seed kullan:

-- INSERT INTO "AspNetUsers" ...  -- secrets.json/AdminSettings yöntemi önerilir

-- 9. KONTROL
SELECT 'SiteAyarlari' as tablo, COUNT(*) FROM "SiteAyarlari"
UNION ALL SELECT 'Urunler (aktif)', COUNT(*) FROM "Urunler" WHERE "AktifMi"=true AND "SilindiMi"=false
UNION ALL SELECT 'Kategoriler', COUNT(*) FROM "Kategoriler" WHERE "AktifMi"=true
UNION ALL SELECT 'AspNetUsers (kalan)', COUNT(*) FROM "AspNetUsers"
UNION ALL SELECT 'Yorumlar', COUNT(*) FROM "Yorumlar"
UNION ALL SELECT 'Siparisler', COUNT(*) FROM "Siparisler";
