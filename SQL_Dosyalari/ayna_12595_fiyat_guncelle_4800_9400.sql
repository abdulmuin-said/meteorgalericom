DO $$
DECLARE
    v_urun RECORD;
    v_boyut text;
    v_taban_fiyat numeric;
    v_led_fark numeric;
    v_adet integer;
    v_aydinlatma RECORD;
BEGIN
    SELECT "Id", "Baslik"
    INTO v_urun
    FROM "Urunler"
    WHERE "Id" = 12595
      AND NOT "SilindiMi";

    IF NOT FOUND THEN
        RAISE EXCEPTION '12595 ID numarali urun bulunamadi.';
    END IF;

    SELECT COUNT(*)
    INTO v_adet
    FROM "UrunSecenekleri"
    WHERE "UrunId" = 12595
      AND NOT "SilindiMi";

    IF v_adet <> 24 THEN
        RAISE EXCEPTION '12595 urunu icin 24 varyant bekleniyordu, bulunan: %', v_adet;
    END IF;

    FOR v_boyut, v_taban_fiyat, v_led_fark IN
        SELECT *
        FROM (VALUES
            ('30x60 cm',  4800, 600),
            ('40x80 cm',  5520, 600),
            ('50x100 cm', 6000, 600),
            ('60x120 cm', 6720, 1000),
            ('70x140 cm', 7500, 1000),
            ('80x160 cm', 8400, 1000)
        ) AS olculer("Olcu", "TabanFiyat", "LedFark")
    LOOP
        UPDATE "UrunSecenekleri"
        SET "SatisFiyati" = v_taban_fiyat,
            "FiyatFarki" = 0
        WHERE "UrunId" = 12595
          AND "Olcu" = v_boyut
          AND COALESCE("CerceveTipi", '') = ''
          AND NOT "SilindiMi";

        FOR v_aydinlatma IN
            SELECT *
            FROM (VALUES
                ('Beyaz LED'),
                ('Günışığı LED'),
                ('Amber LED')
            ) AS aydinlatmalar("Tip")
        LOOP
            UPDATE "UrunSecenekleri"
            SET "SatisFiyati" = v_taban_fiyat + v_led_fark,
                "FiyatFarki" = v_led_fark
            WHERE "UrunId" = 12595
              AND "Olcu" = v_boyut
              AND "CerceveTipi" = v_aydinlatma."Tip"
              AND NOT "SilindiMi";
        END LOOP;
    END LOOP;

    UPDATE "Urunler"
    SET "Fiyat" = 4800,
        "IndirimliFiyat" = NULL
    WHERE "Id" = 12595;

    -- Daha önce sepete eklenmiş ürünlerin eski düşük fiyatla satın alınmasını engeller.
    UPDATE "SepetItems" sepet
    SET "Fiyat" = secenek."SatisFiyati"
    FROM "UrunSecenekleri" secenek
    WHERE sepet."UrunSecenekId" = secenek."Id"
      AND secenek."UrunId" = 12595
      AND NOT sepet."SilindiMi";

    RAISE NOTICE '12595 urunu varyant fiyatlari basariyla guncellendi: %', v_urun."Baslik";
END
$$;

SELECT
    "Id",
    "Olcu",
    CASE
        WHEN COALESCE("CerceveTipi", '') = '' THEN 'LED''siz'
        ELSE "CerceveTipi"
    END AS "Aydinlatma",
    "SatisFiyati",
    "FiyatFarki",
    "StokAdedi",
    "AktifMi",
    "Sira"
FROM "UrunSecenekleri"
WHERE "UrunId" = 12595
  AND NOT "SilindiMi"
ORDER BY "Sira", "Id";
