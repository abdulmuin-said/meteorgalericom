-- Tüm mevcut fiyatlar (Atatürk dışında)
SELECT v."Olcu", MIN(v."SatisFiyati") as min_fiyat, MAX(v."SatisFiyati") as max_fiyat, COUNT(*) as adet
FROM "Urunler" u
JOIN "UrunSecenekleri" v ON u."Id" = v."UrunId"
WHERE u."KategoriId" != 10 AND NOT u."SilindiMi" AND NOT v."SilindiMi" AND v."Olcu" IS NOT NULL
GROUP BY v."Olcu"
ORDER BY v."Olcu";