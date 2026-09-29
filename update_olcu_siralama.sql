-- Tüm ürünlerin seçeneklerini ölçüye göre (küçükten büyüğe) sırala
DO $$
DECLARE
    v_urun_id integer;
    v_sira integer;
    v_id integer;
    v_olcu text;
    v_w integer;
    v_h integer;
BEGIN
    FOR v_urun_id IN SELECT DISTINCT "UrunId" FROM "UrunSecenekleri" WHERE "SilindiMi" = false LOOP
        v_sira := 1;
        
        FOR v_id, v_olcu IN 
            SELECT "Id", "Olcu"
            FROM "UrunSecenekleri"
            WHERE "UrunId" = v_urun_id AND "SilindiMi" = false AND "Olcu" ~ '^\d+cm\s*x\s*\d+cm$'
        LOOP
            v_w := CAST(SUBSTRING(v_olcu FROM E'^(\\d+)cm\\s*x\\s*(\\d+)cm') AS integer);
            v_h := CAST(SUBSTRING(v_olcu FROM E'^\\d+cm\\s*x\\s*(\\d+)cm') AS integer);
            
            UPDATE "UrunSecenekleri" SET "Sira" = v_sira WHERE "Id" = v_id;
            v_sira := v_sira + 1;
        END LOOP;
    END LOOP;
    
    RAISE NOTICE 'Siralama tamamlandi';
END $$;