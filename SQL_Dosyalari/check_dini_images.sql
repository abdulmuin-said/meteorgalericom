-- Dini ürünlerinin varyasyon görsellerini kontrol et
SELECT u."Baslik", v."Olcu", v."GorselUrl", COUNT(*) as adet
FROM "Urunler" u
JOIN "UrunSecenekleri" v ON u."Id" = v."UrunId"
WHERE u."KategoriId" = 8 AND NOT v."SilindiMi"
GROUP BY u."Baslik", v."Olcu", v."GorselUrl"
ORDER BY u."Baslik"
LIMIT 30;