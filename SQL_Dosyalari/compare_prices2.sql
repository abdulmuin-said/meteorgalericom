-- Bizim mevcut fiyatlar (Kanvas Tablo kategorisindeki)
SELECT DISTINCT v."Olcu", MIN(v."SatisFiyati") as min_fiyat, MAX(v."SatisFiyati") as max_fiyat
FROM "Urunler" u
JOIN "UrunSecenekleri" v ON u."Id" = v."UrunId"
WHERE u."KategoriId" = 29 AND NOT u."SilindiMi" AND NOT v."SilindiMi"
GROUP BY v."Olcu"
ORDER BY v."Olcu";