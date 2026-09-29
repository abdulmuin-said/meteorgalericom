-- Çift cm'yi düzelt: "60cm x 40cmcm" -> "60cm x 40cm"
UPDATE "UrunSecenekleri" 
SET "Olcu" = REPLACE("Olcu", 'cmcm', 'cm')
WHERE "Olcu" LIKE '%cmcm%';