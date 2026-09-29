-- Railway-YENI: eski Genel kanvas kataloğunda ters yönü Excel'de bulunan 11 ölçüyü günceller.
-- Otomatik COMMIT yapmaz. Kontrol sonuçlarından sonra COMMIT veya ROLLBACK çalıştırın.
BEGIN;

CREATE TEMP TABLE "GenelKanvasTersOlcuFiyat" (
    "Olcu" text PRIMARY KEY,
    "BeklenenFiyat" numeric NOT NULL
) ON COMMIT DROP;

INSERT INTO "GenelKanvasTersOlcuFiyat" VALUES
('100cm x 50cm', 1317.74),
('120cm x 80cm', 2017.53),
('140cm x 70cm', 2057.01),
('150cm x 100cm', 2795.50),
('150cm x 60cm', 1962.61),
('150cm x 90cm', 2577.50),
('160cm x 80cm', 2495.72),
('180cm x 120cm', 3729.48),
('180cm x 90cm', 2976.68),
('250cm x 150cm', 6069.56),
('70cm x 35cm', 882.61);

SELECT
    COUNT(*) AS "GuncellenecekVaryant",
    COUNT(DISTINCT u."Id") AS "EtkilenecekUrun"
FROM "UrunSecenekleri" s
INNER JOIN "Urunler" u ON u."Id" = s."UrunId"
INNER JOIN "GenelKanvasTersOlcuFiyat" f ON f."Olcu" = s."Olcu"
WHERE u."UrunTipi" = 'Genel'
  AND u."Baslik" ILIKE '%kanvas%'
  AND NOT u."SilindiMi"
  AND NOT s."SilindiMi";

UPDATE "UrunSecenekleri" s
SET "SatisFiyati" = f."BeklenenFiyat"
FROM "Urunler" u, "GenelKanvasTersOlcuFiyat" f
WHERE s."UrunId" = u."Id"
  AND s."Olcu" = f."Olcu"
  AND u."UrunTipi" = 'Genel'
  AND u."Baslik" ILIKE '%kanvas%'
  AND NOT u."SilindiMi"
  AND NOT s."SilindiMi";

-- Etkilenen ürünlerin kart fiyatını en düşük aktif varyant fiyatına eşitler.
UPDATE "Urunler" u
SET "Fiyat" = kaynak."Fiyat"
FROM (
    SELECT s."UrunId", MIN(s."SatisFiyati") AS "Fiyat"
    FROM "UrunSecenekleri" s
    WHERE NOT s."SilindiMi"
      AND s."AktifMi"
      AND s."SatisFiyati" > 0
      AND EXISTS (
          SELECT 1
          FROM "UrunSecenekleri" hedef
          INNER JOIN "GenelKanvasTersOlcuFiyat" f ON f."Olcu" = hedef."Olcu"
          WHERE hedef."UrunId" = s."UrunId" AND NOT hedef."SilindiMi"
      )
    GROUP BY s."UrunId"
) kaynak
WHERE u."Id" = kaynak."UrunId"
  AND u."UrunTipi" = 'Genel'
  AND u."Baslik" ILIKE '%kanvas%'
  AND NOT u."SilindiMi";

SELECT
    COUNT(*) AS "KontrolEdilenVaryant",
    COUNT(*) FILTER (WHERE s."SatisFiyati" = f."BeklenenFiyat") AS "DogruFiyatliVaryant",
    COUNT(*) FILTER (WHERE s."SatisFiyati" IS DISTINCT FROM f."BeklenenFiyat") AS "HataliFiyatliVaryant"
FROM "UrunSecenekleri" s
INNER JOIN "Urunler" u ON u."Id" = s."UrunId"
INNER JOIN "GenelKanvasTersOlcuFiyat" f ON f."Olcu" = s."Olcu"
WHERE u."UrunTipi" = 'Genel'
  AND u."Baslik" ILIKE '%kanvas%'
  AND NOT u."SilindiMi"
  AND NOT s."SilindiMi";

-- Beklenen: 615 kontrol, 615 doğru, 0 hatalı.
-- Doğruysa aynı Query Tool sekmesinde: COMMIT;
-- Beklenmeyen sonuçta: ROLLBACK;
