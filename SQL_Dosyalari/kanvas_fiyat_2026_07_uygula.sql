-- Railway-YENI. Sadece UrunTipi = Kanvas Tablo kayıtlarını günceller.
-- Excel'de olmayan 200cm x 100cm makam panoları dahil edilmez.
BEGIN;

CREATE TEMP TABLE "KanvasFiyatGuncelleme" (
    "Olcu" text PRIMARY KEY,
    "CercevesizKargoDahil" numeric NOT NULL,
    "CerceveliKargoDahil" numeric NOT NULL
) ON COMMIT DROP;

INSERT INTO "KanvasFiyatGuncelleme" VALUES
('10X30',433.76,684.04),('18X25',457.03,715.06),('20X30',510.33,784.58),('20X50',603.39,923.46),('20X60',649.21,991.14),
('25X25',512.45,786.69),('25X35',565.32,862.13),('25X60',712.81,1067.42),('25X75',790.35,1180.22),('30X30',567.44,865.65),
('30X35',596.34,905.83),('30X45',671.21,1003.97),('30X60',757.22,1125.94),('30X70',832.55,1223.83),('30X90',965.31,1403.82),
('35X50',738.89,1096.32),('35X70',882.61,1287.98),('35X80',963.90,1392.54),('40X40',710.69,1055.44),('40X50',797.30,1167.43),
('40X60',864.98,1258.37),('40X80',1018.89,1461.63),('40X120',1349.46,1888.78),('45X90',1157.70,1639.22),('45X100',1240.88,1746.36),
('50X50',875.56,1270.36),('50X70',1061.82,1506.67),('50X100',1317.74,1838.73),('50X110',1431.24,1976.21),('50X120',1520.68,2091.73),
('50X150',1767.41,2413.90),('55X100',1384.01,1919.10),('60X40',864.98,1258.37),('60X60',1072.40,1517.96),('60X90',1360.74,1884.56),
('60X100',1460.05,2009.25),('60X120',1692.69,2293.35),('60X140',1873.78,2527.32),('60X150',1962.61,2641.53),('60X180',2249.31,3005.77),
('60X200',2438.94,3246.87),('70X70',1282.49,1780.92),('70X100',1604.56,2183.37),('70X120',1854.76,2486.44),('70X140',2057.01,2742.27),
('70X150',2158.55,2869.90),('70X160',2273.99,3012.83),('75X75',1394.49,1919.71),('75X90',1563.66,2129.07),('75X120',1935.82,2582.31),
('75X175',2550.71,3346.66),('80X80',1513.61,2065.62),('80X110',1906.21,2541.42),('80X120',2017.53,2680.23),('80X150',2372.99,3117.48),
('80X160',2495.72,3267.70),('85X150',2474.90,3235.59),('90X60',1360.74,1884.56),('90X90',1748.30,2356.01),('90X110',2058.44,2723.25),
('90X120',2195.73,2888.75),('90X130',2322.94,3043.45),('90X140',2450.22,3199.64),('90X150',2577.50,3355.12),('90X170',2845.56,3678.87),
('90X180',2976.68,3838.90),('100X60',1460.05,2009.25),('100X100',2005.57,2671.09),('100X130',2506.30,3258.53),('100X150',2795.50,3606.25),
('100X160',2937.20,3776.15),('100X180',3230.49,4127.25),('100X235',4101.91,5158.70),('109X148',2950.60,3784.61),('110X110',2386.07,3109.40),
('110X120',2534.50,3288.85),('110X160',3160.47,4033.26),('110X180',3474.10,4406.82),('110X200',3790.11,4782.05),('110X250',4680.56,5820.55),
('120X60',1349.46,1888.78),('120X70',1854.76,2486.44),('120X120',2711.02,3495.69),('120X125',2787.17,3587.34),('120X150',3219.91,4096.23),
('120X160',3382.28,4288.91),('120X180',3729.48,4696.74),('130X130',3064.72,3910.72),('140X140',3448.72,4357.47),('140X200',4628.39,5730.31),
('140X290',6511.96,7902.93),('150X70',2158.55,2869.90),('150X150',3859.65,4833.96),('150X250',6069.56,7373.10),('150X280',6701.09,8102.63),
('150X290',6911.36,8345.33),('150X300',7121.64,8589.45),('153X204',5193.91,6357.16),('180X200',5750.00,6999.26);

UPDATE "UrunSecenekleri" s
SET "SatisFiyati" = f."CercevesizKargoDahil"
FROM "Urunler" u
    , "KanvasFiyatGuncelleme" f
WHERE s."UrunId" = u."Id"
  AND f."Olcu" = upper(replace(replace(replace(s."Olcu", 'cm', ''), ' ', ''), '×', 'X'))
  AND u."UrunTipi" = 'Kanvas Tablo'
  AND NOT u."SilindiMi"
  AND NOT s."SilindiMi";

-- Ürün kartlarında doğru başlangıç fiyatının görünmesi için en düşük aktif varyantı eşitler.
UPDATE "Urunler" u
SET "Fiyat" = kaynak."Fiyat"
FROM (
    SELECT s."UrunId", MIN(s."SatisFiyati") AS "Fiyat"
    FROM "UrunSecenekleri" s
    INNER JOIN "Urunler" urun ON urun."Id" = s."UrunId"
    WHERE urun."UrunTipi" = 'Kanvas Tablo'
      AND NOT urun."SilindiMi"
      AND NOT s."SilindiMi"
      AND s."AktifMi"
      AND s."SatisFiyati" > 0
    GROUP BY s."UrunId"
) kaynak
WHERE u."Id" = kaynak."UrunId";

SELECT
    COUNT(*) AS "GuncellenenVaryant",
    MIN(s."SatisFiyati") AS "EnDusukYeniFiyat",
    MAX(s."SatisFiyati") AS "EnYuksekYeniFiyat"
FROM "UrunSecenekleri" s
INNER JOIN "Urunler" u ON u."Id" = s."UrunId"
INNER JOIN "KanvasFiyatGuncelleme" f
    ON f."Olcu" = upper(replace(replace(replace(s."Olcu", 'cm', ''), ' ', ''), '×', 'X'))
WHERE u."UrunTipi" = 'Kanvas Tablo' AND NOT u."SilindiMi" AND NOT s."SilindiMi";

SELECT s."Olcu", COUNT(*) AS "GuncellenmeyenVaryant"
FROM "UrunSecenekleri" s
INNER JOIN "Urunler" u ON u."Id" = s."UrunId"
WHERE u."UrunTipi" = 'Kanvas Tablo' AND NOT u."SilindiMi" AND NOT s."SilindiMi"
  AND NOT EXISTS (
      SELECT 1 FROM "KanvasFiyatGuncelleme" f
      WHERE f."Olcu" = upper(replace(replace(replace(s."Olcu", 'cm', ''), ' ', ''), '×', 'X'))
  )
GROUP BY s."Olcu"
ORDER BY "GuncellenmeyenVaryant" DESC;

-- Kontrol sonuçları doğruysa ayrı bir sorguda COMMIT; çalıştırın. Vazgeçerseniz ROLLBACK; çalıştırın.
