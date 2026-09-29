-- Tüm ürünlerin seçeneklerini ölçüye göre (küçükten büyüğe) sırala
-- Alan bazlı sıralama: genişlik * yükseklik

WITH parsed AS (
    SELECT 
        "Id",
        "UrunId",
        (regexp_matches("Olcu", E'^(\\d+)cm\\s*x\\s*(\\d+)cm'))[1]::numeric as w,
        (regexp_matches("Olcu", E'^(\\d+)cm\\s*x\\s*(\\d+)cm'))[2]::numeric as h
    FROM "UrunSecenekleri"
    WHERE "SilindiMi" = false AND "Olcu" ~ '^\d+cm\s*x\s*\d+cm$'
),
ranked AS (
    SELECT 
        "Id",
        ROW_NUMBER() OVER (PARTITION BY "UrunId" ORDER BY w * h ASC) as new_sira
    FROM parsed
)
UPDATE "UrunSecenekleri" us
SET "Sira" = r.new_sira
FROM ranked r
WHERE us."Id" = r."Id";