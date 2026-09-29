-- Ölçüleri standart formata çevir (X cm x Y cm)
UPDATE "UrunSecenekleri" SET "Olcu" = 
    CASE 
        -- Tüm boşlukları ve x'leri standartlaştır
        WHEN "Olcu" LIKE '%x%' THEN 
            UPPER(TRIM(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
                "Olcu", 
                'x', 'x'), 
                'X', 'x'), 
                '  ', ' '), 
                ' cm', 'cm'), 
                'cm ', 'cm')))
        ELSE "Olcu"
    END
WHERE "Olcu" IS NOT NULL AND "Olcu" != '';

-- Parantez içindeki parça sayılarını temizle ve parantezi kaldır
UPDATE "UrunSecenekleri" SET "Olcu" = 
    CASE 
        WHEN "Olcu" LIKE '%(%' THEN TRIM(SUBSTRING("Olcu" FROM 1 FOR POSITION('(' IN "Olcu") - 1))
        ELSE "Olcu"
    END
WHERE "Olcu" LIKE '%(%';