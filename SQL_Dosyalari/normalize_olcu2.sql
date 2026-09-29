-- Standart formata çevirmek için bir temp tablo ve cursor kullanarak
-- Basit yaklaşım: önce parantezleri kaldır, sonra formatla

-- Parantez içeriğini kaldır (örn: "100 cm x 60 cm (5 Parça...)" -> "100 cm x 60 cm")
UPDATE "UrunSecenekleri" 
SET "Olcu" = TRIM(REGEXP_REPLACE("Olcu", '\s*\([^)]*\)\s*', '', 'g'))
WHERE "Olcu" LIKE '%(%';