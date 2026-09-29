-- Dini kategori fiyat kontrolü
SELECT u."Baslik", v."Olcu", v."SatisFiyati"
FROM "Urunler" u
JOIN "UrunSecenekleri" v ON u."Id" = v."UrunId"
WHERE u."KategoriId" = 8 AND NOT v."SilindiMi"
LIMIT 20;