-- Railway-YENI fiyat doğrulama. Kalıcı veri değiştirmez.
DROP TABLE IF EXISTS "KanvasFiyatDogrulama";
CREATE TEMP TABLE "KanvasFiyatDogrulama" ("Olcu" text PRIMARY KEY, "BeklenenFiyat" numeric NOT NULL);

INSERT INTO "KanvasFiyatDogrulama" VALUES
('10X30',433.76),('18X25',457.03),('20X30',510.33),('20X50',603.39),('20X60',649.21),('25X25',512.45),('25X35',565.32),('25X60',712.81),('25X75',790.35),
('30X30',567.44),('30X35',596.34),('30X45',671.21),('30X60',757.22),('30X70',832.55),('30X90',965.31),('35X50',738.89),('35X70',882.61),('35X80',963.90),
('40X40',710.69),('40X50',797.30),('40X60',864.98),('40X80',1018.89),('40X120',1349.46),('45X90',1157.70),('45X100',1240.88),('50X50',875.56),
('50X70',1061.82),('50X100',1317.74),('50X110',1431.24),('50X120',1520.68),('50X150',1767.41),('55X100',1384.01),('60X40',864.98),('60X60',1072.40),
('60X90',1360.74),('60X100',1460.05),('60X120',1692.69),('60X140',1873.78),('60X150',1962.61),('60X180',2249.31),('60X200',2438.94),('70X70',1282.49),
('70X100',1604.56),('70X120',1854.76),('70X140',2057.01),('70X150',2158.55),('70X160',2273.99),('75X75',1394.49),('75X90',1563.66),('75X120',1935.82),
('75X175',2550.71),('80X80',1513.61),('80X110',1906.21),('80X120',2017.53),('80X150',2372.99),('80X160',2495.72),('85X150',2474.90),('90X60',1360.74),
('90X90',1748.30),('90X110',2058.44),('90X120',2195.73),('90X130',2322.94),('90X140',2450.22),('90X150',2577.50),('90X170',2845.56),('90X180',2976.68),
('100X60',1460.05),('100X100',2005.57),('100X130',2506.30),('100X150',2795.50),('100X160',2937.20),('100X180',3230.49),('100X235',4101.91),('109X148',2950.60),
('110X110',2386.07),('110X120',2534.50),('110X160',3160.47),('110X180',3474.10),('110X200',3790.11),('110X250',4680.56),('120X60',1349.46),
('120X70',1854.76),('120X120',2711.02),('120X125',2787.17),('120X150',3219.91),('120X160',3382.28),('120X180',3729.48),('130X130',3064.72),
('140X140',3448.72),('140X200',4628.39),('140X290',6511.96),('150X70',2158.55),('150X150',3859.65),('150X250',6069.56),('150X280',6701.09),
('150X290',6911.36),('150X300',7121.64),('153X204',5193.91),('180X200',5750.00);

WITH hedef AS (
    SELECT s."Id", s."Olcu", s."SatisFiyati", f."BeklenenFiyat"
    FROM "UrunSecenekleri" s
    INNER JOIN "Urunler" u ON u."Id" = s."UrunId"
    LEFT JOIN "KanvasFiyatDogrulama" f
        ON f."Olcu" = upper(replace(replace(replace(s."Olcu", 'cm', ''), ' ', ''), '×', 'X'))
    WHERE u."UrunTipi" = 'Kanvas Tablo' AND NOT u."SilindiMi" AND NOT s."SilindiMi"
)
SELECT
    COUNT(*) AS "ToplamKanvasVaryanti",
    COUNT(*) FILTER (WHERE "BeklenenFiyat" IS NOT NULL) AS "ExcelKapsamindakiVaryant",
    COUNT(*) FILTER (WHERE "BeklenenFiyat" IS NOT NULL AND "SatisFiyati" = "BeklenenFiyat") AS "DogruFiyatliVaryant",
    COUNT(*) FILTER (WHERE "BeklenenFiyat" IS NOT NULL AND "SatisFiyati" IS DISTINCT FROM "BeklenenFiyat") AS "HatalifiyatliVaryant",
    COUNT(*) FILTER (WHERE "BeklenenFiyat" IS NULL) AS "ExcelDisiKapsam"
FROM hedef;

SELECT s."Id" AS "SecenekId", u."Id" AS "UrunId", u."Baslik", s."Olcu", s."SatisFiyati", f."BeklenenFiyat"
FROM "UrunSecenekleri" s
INNER JOIN "Urunler" u ON u."Id" = s."UrunId"
INNER JOIN "KanvasFiyatDogrulama" f
    ON f."Olcu" = upper(replace(replace(replace(s."Olcu", 'cm', ''), ' ', ''), '×', 'X'))
WHERE u."UrunTipi" = 'Kanvas Tablo' AND NOT u."SilindiMi" AND NOT s."SilindiMi"
  AND s."SatisFiyati" IS DISTINCT FROM f."BeklenenFiyat"
ORDER BY u."Baslik", s."Olcu";
