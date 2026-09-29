SELECT COUNT(*) as toplam_olcu, 
       COUNT(DISTINCT "Olcu") as benzersiz_olcu,
       (SELECT COUNT(*) FROM "UrunSecenekleri" WHERE NOT "SilindiMi") as toplam_varyasyon
FROM "UrunSecenekleri" WHERE "Olcu" IS NOT NULL;