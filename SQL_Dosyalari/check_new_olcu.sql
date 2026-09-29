-- Yeni eklenen Atatürk ürünlerinin ölçüleri
SELECT DISTINCT u."Baslik", v."Olcu", v."SatisFiyati"
FROM "Urunler" u
JOIN "UrunSecenekleri" v ON u."Id" = v."UrunId"
WHERE u."KategoriId" = 10
ORDER BY u."Baslik", v."Olcu"
LIMIT 30;