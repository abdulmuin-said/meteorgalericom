BEGIN;

DO $$
DECLARE
    v_urun_ids integer[] := ARRAY[
        9400, 9403, 9409, 9411, 9416,
        9417, 9418, 9419, 9420, 9421,
        9422, 9423, 9449, 9450, 9458,
        9459, 9460, 9462, 9463, 9464
    ];
    v_urun_sayisi integer;
    v_varyant_sayisi integer;
    v_hatali_urun_ids text;
BEGIN
    SELECT COUNT(*)
    INTO v_urun_sayisi
    FROM "Urunler"
    WHERE "Id" = ANY(v_urun_ids)
      AND "AktifMi"
      AND NOT "SilindiMi";

    IF v_urun_sayisi <> 20 THEN
        RAISE EXCEPTION '20 aktif makam ürünü bekleniyordu, bulunan: %', v_urun_sayisi;
    END IF;

    SELECT string_agg(kontrol."UrunId"::text, ', ' ORDER BY kontrol."UrunId")
    INTO v_hatali_urun_ids
    FROM (
        SELECT
            hedef."UrunId",
            COUNT(secenek."Id") FILTER (WHERE secenek."Olcu" = '120cm x 60cm') AS "120x60",
            COUNT(secenek."Id") FILTER (WHERE secenek."Olcu" = '150cm x 70cm') AS "150x70",
            COUNT(secenek."Id") FILTER (WHERE secenek."Olcu" = '200cm x 100cm') AS "200x100",
            COUNT(secenek."Id") AS "Toplam"
        FROM unnest(v_urun_ids) AS hedef("UrunId")
        LEFT JOIN "UrunSecenekleri" secenek
          ON secenek."UrunId" = hedef."UrunId"
         AND secenek."AktifMi"
         AND NOT secenek."SilindiMi"
        GROUP BY hedef."UrunId"
    ) kontrol
    WHERE
        (kontrol."UrunId" = 9400 AND
            (kontrol."120x60" <> 1 OR kontrol."Toplam" <> 1))
        OR
        (kontrol."UrunId" <> 9400 AND
            (kontrol."150x70" <> 1 OR kontrol."200x100" <> 1 OR kontrol."Toplam" <> 2));

    IF v_hatali_urun_ids IS NOT NULL THEN
        RAISE EXCEPTION 'Beklenmeyen varyant yapısına sahip ürünler: %', v_hatali_urun_ids;
    END IF;

    UPDATE "UrunSecenekleri"
    SET
        "SatisFiyati" = CASE
            WHEN "Olcu" = '200cm x 100cm' THEN 10499
            ELSE 9499
        END,
        "FiyatFarki" = CASE
            WHEN "Olcu" = '200cm x 100cm' THEN 1000
            ELSE 0
        END
    WHERE "UrunId" = ANY(v_urun_ids)
      AND "AktifMi"
      AND NOT "SilindiMi";

    GET DIAGNOSTICS v_varyant_sayisi = ROW_COUNT;
    IF v_varyant_sayisi <> 39 THEN
        RAISE EXCEPTION '39 varyant güncellenmeliydi, güncellenen: %', v_varyant_sayisi;
    END IF;

    UPDATE "Urunler"
    SET
        "Fiyat" = 9499,
        "IndirimliFiyat" = NULL
    WHERE "Id" = ANY(v_urun_ids);

    -- Daha önce sepete eklenmiş ürünlerin eski düşük fiyatla satın alınmasını engeller.
    UPDATE "SepetItems" sepet
    SET "Fiyat" = secenek."SatisFiyati"
    FROM "UrunSecenekleri" secenek
    WHERE sepet."UrunSecenekId" = secenek."Id"
      AND secenek."UrunId" = ANY(v_urun_ids)
      AND NOT sepet."SilindiMi";

    RAISE NOTICE '20 makam ürünü ve 39 varyant başarıyla güncellendi.';
END
$$;

COMMIT;

SELECT
    urun."Id" AS "UrunId",
    urun."Baslik",
    urun."Fiyat" AS "ListeFiyati",
    secenek."Id" AS "VaryantId",
    secenek."Olcu",
    secenek."SatisFiyati",
    secenek."FiyatFarki"
FROM "Urunler" urun
JOIN "UrunSecenekleri" secenek ON secenek."UrunId" = urun."Id"
WHERE urun."Id" IN (
    9400, 9403, 9409, 9411, 9416,
    9417, 9418, 9419, 9420, 9421,
    9422, 9423, 9449, 9450, 9458,
    9459, 9460, 9462, 9463, 9464
)
  AND secenek."AktifMi"
  AND NOT secenek."SilindiMi"
ORDER BY urun."Id", secenek."Sira", secenek."Id";
