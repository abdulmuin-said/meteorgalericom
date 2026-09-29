-- Dini ve Hat Sanatı (kategori 8) fiyatlarını güncelle
UPDATE "UrunSecenekleri" v
SET 
    "SatisFiyati" = COALESCE(
        (SELECT AVG(v2."SatisFiyati")
         FROM "UrunSecenekleri" v2
         JOIN "Urunler" u2 ON v2."UrunId" = u2."Id"
         WHERE u2."KategoriId" NOT IN (10, 8) 
           AND NOT u2."SilindiMi" 
           AND NOT v2."SilindiMi" 
           AND v2."Olcu" = v."Olcu"
           AND v2."Olcu" IS NOT NULL
           AND v2."Olcu" NOT LIKE '%Parçalı%'
           AND v2."Olcu" != 'Tek Parçalı'
        ),
        v."SatisFiyati"
    ),
    "MaliyetFiyati" = COALESCE(
        (SELECT AVG(v2."SatisFiyati") * 0.6
         FROM "UrunSecenekleri" v2
         JOIN "Urunler" u2 ON v2."UrunId" = u2."Id"
         WHERE u2."KategoriId" NOT IN (10, 8) 
           AND NOT u2."SilindiMi" 
           AND NOT v2."SilindiMi" 
           AND v2."Olcu" = v."Olcu"
           AND v2."Olcu" IS NOT NULL
           AND v2."Olcu" NOT LIKE '%Parçalı%'
           AND v2."Olcu" != 'Tek Parçalı'
        ),
        v."MaliyetFiyati"
    )
FROM "Urunler" u
WHERE v."UrunId" = u."Id"
  AND u."KategoriId" = 8
  AND NOT v."SilindiMi"
  AND v."Olcu" IS NOT NULL
  AND v."Olcu" NOT LIKE '%Parçalı%'
  AND v."Olcu" != 'Tek Parçalı';