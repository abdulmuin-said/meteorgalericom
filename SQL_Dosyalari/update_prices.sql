-- Atatürk ürünlerinin fiyatlarını bizim fiyatlarımızla güncelle
-- Ölçü bazında ortalama fiyatı al ve uygula

-- Önce mevcut fiyatların ortalamasını hesapla (Atatürk dışı)
CREATE TEMP TABLE bizim_fiyatlar AS
SELECT v."Olcu", AVG(v."SatisFiyati") as ortalama_fiyat
FROM "Urunler" u
JOIN "UrunSecenekleri" v ON u."Id" = v."UrunId"
WHERE u."KategoriId" != 10 
  AND NOT u."SilindiMi" 
  AND NOT v."SilindiMi" 
  AND v."Olcu" IS NOT NULL
  AND v."Olcu" != 'Tek Parçalı'
  AND v."Olcu" NOT LIKE '%Parçalı%'
GROUP BY v."Olcu";

-- Atatürk ürünlerini güncelle
UPDATE "UrunSecenekleri" v
SET "SatisFiyati" = bf.ortalama_fiyat,
    "MaliyetFiyati" = bf.ortalama_fiyat * 0.6
FROM bizim_fiyatlar bf
JOIN "Urunler" u ON v."UrunId" = u."Id"
WHERE v."Olcu" = bf."Olcu"
  AND u."KategoriId" = 10
  AND NOT v."SilindiMi";

-- Silinen tablo
DROP TABLE bizim_fiyatlar;