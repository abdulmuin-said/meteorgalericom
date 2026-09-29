-- Tüm ürünlerin seçeneklerini ölçüye göre (küçükten büyüğe) sırala - Düzeltilmiş
DO $$
DECLARE
    v_urun_id integer;
    v_sira integer;
    v_id integer;
    v_olcu text;
    v_w integer;
    v_h integer;
    v_alan integer;
    rec RECORD;
BEGIN
    FOR v_urun_id IN SELECT DISTINCT "UrunId" FROM "UrunSecenekleri" WHERE "SilindiMi" = false LOOP
        v_sira := 1;
        
        -- Önce tüm kayıtları al ve geçici tablo yap
        FOR rec IN 
            SELECT "Id", "Olcu",
                   (regexp_matches("Olcu", E'^(\\d+)cm\\s*x\\s*(\\d+)cm'))[1]::integer as w,
                   (regexp_matches("Olcu", E'^(\\d+)cm\\s*x\\s*(\\d+)cm'))[2]::integer as h
            FROM "UrunSecenekleri"
            WHERE "UrunId" = v_urun_id AND "SilindiMi" = false AND "Olcu" ~ '^\d+cm\s*x\s*\d+cm$'
        LOOP
            UPDATE "UrunSecenekleri" 
            SET "Sira" = v_sira 
            WHERE "Id" = rec."Id";
            v_sira := v_sira + 1;
        END LOOP;
    END LOOP;
    
    RAISE NOTICE 'Siralama tamamlandi - % satir', v_sira;
END $$;