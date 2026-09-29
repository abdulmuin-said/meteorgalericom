-- Boşluklu x'leri düzelt: "150 x 100cm" -> "150cm x 100cm"
UPDATE "UrunSecenekleri" 
SET "Olcu" = REGEXP_REPLACE("Olcu", '(\d+)\s+x\s+(\d+cm)', '$1cm x $2', 'g')
WHERE "Olcu" LIKE '% x %';

-- Parantez içindeki parça sayılarını temizle
UPDATE "UrunSecenekleri" 
SET "Olcu" = TRIM(REGEXP_REPLACE("Olcu", '\s*\([^)]*\)\s*', '', 'g'))
WHERE "Olcu" LIKE '%(%';