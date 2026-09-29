-- Bizim mevcut fiyatlar (Atatürk kategorisi dışındaki kanvas ürünleri)
SELECT DISTINCT v."Olcu", v."SatisFiyati" as bizim_fiyat
FROM "Urunler" u
JOIN "UrunSecenekleri" v ON u."Id" = v."UrunId"
WHERE u."KategoriId" != 10 AND u."UrunTipi" = 'Kanvas Tablo' AND NOT u."SilindiMi"
ORDER BY v."Olcu", v."SatisFiyati";