DO $$
DECLARE
    v_urun RECORD;
    v_boyut text;
    v_boyut_sira integer;
    v_taban_fiyat numeric;
    v_led_fark numeric;
    v_adet integer;
    v_aydinlatma RECORD;
BEGIN
    SELECT "Id", "Baslik"
    INTO v_urun
    FROM "Urunler"
    WHERE "Id" = 12817
      AND NOT "SilindiMi";

    IF NOT FOUND THEN
        RAISE EXCEPTION '12817 ID numarali urun bulunamadi.';
    END IF;

    IF v_urun."Baslik" NOT ILIKE '%ayna%'
       OR v_urun."Baslik" NOT ILIKE '%lazer%' THEN
        RAISE EXCEPTION 'Urun dogrulanamadi. Bulunan urun: %', v_urun."Baslik";
    END IF;

    SELECT COUNT(*)
    INTO v_adet
    FROM "UrunSecenekleri"
    WHERE "UrunId" = 12817
      AND NOT "SilindiMi"
      AND COALESCE("Olcu", '') NOT IN ('30x60 cm', '40x80 cm', '50x100 cm', '60x120 cm', '70x140 cm', '80x160 cm');

    IF v_adet > 1 THEN
        RAISE EXCEPTION '12817 urunu icin tanimsiz % aktif varyant var. Elden duzeltin.', v_adet;
    END IF;

    IF v_adet = 1 THEN
        UPDATE "UrunSecenekleri"
        SET "Olcu" = '30x60 cm',
            "SatisFiyati" = 4800,
            "FiyatFarki" = 0,
            "StokAdedi" = 100,
            "UretimSuresiGun" = 3,
            "Sira" = 10,
            "VarsayilanMi" = true,
            "AktifMi" = true
        WHERE "UrunId" = 12817
          AND NOT "SilindiMi"
          AND COALESCE("Olcu", '') NOT IN ('30x60 cm', '40x80 cm', '50x100 cm', '60x120 cm', '70x140 cm', '80x160 cm');
    END IF;

    FOR v_boyut, v_boyut_sira, v_taban_fiyat, v_led_fark IN
        SELECT *
        FROM (VALUES
            ('30x60 cm',  10, 4800, 600),
            ('40x80 cm',  20, 5520, 600),
            ('50x100 cm', 30, 6000, 600),
            ('60x120 cm', 40, 6720, 1000),
            ('70x140 cm', 50, 7500, 1000),
            ('80x160 cm', 60, 8400, 1000)
        ) AS olculer("Olcu", "Sira", "TabanFiyat", "LedFark")
    LOOP
        IF NOT EXISTS (
            SELECT 1 FROM "UrunSecenekleri"
            WHERE "UrunId" = 12817
              AND "Olcu" = v_boyut
              AND COALESCE("CerceveTipi", '') = ''
              AND NOT "SilindiMi"
        ) THEN
            INSERT INTO "UrunSecenekleri" (
                "UrunId",
                "Olcu",
                "CerceveTipi",
                "SatisFiyati",
                "MaliyetFiyati",
                "FiyatFarki",
                "StokAdedi",
                "UretimSuresiGun",
                "Sira",
                "VarsayilanMi",
                "AktifMi",
                "SilindiMi",
                "OnSipariseAcikMi",
                "OlusturulmaTarihi"
            )
            VALUES (
                12817,
                v_boyut,
                '',
                v_taban_fiyat,
                0,
                0,
                100,
                3,
                v_boyut_sira,
                CASE WHEN v_boyut = '30x60 cm' THEN true ELSE false END,
                true,
                false,
                false,
                NOW()
            );
        END IF;

        FOR v_aydinlatma IN
            SELECT *
            FROM (VALUES
                ('Beyaz LED',   v_boyut_sira + 1, v_led_fark),
                ('Günışığı LED', v_boyut_sira + 2, v_led_fark),
                ('Amber LED',    v_boyut_sira + 3, v_led_fark)
            ) AS aydinlatmalar("Tip", "Sira", "Fark")
        LOOP
            IF NOT EXISTS (
                SELECT 1 FROM "UrunSecenekleri"
                WHERE "UrunId" = 12817
                  AND "Olcu" = v_boyut
                  AND "CerceveTipi" = v_aydinlatma."Tip"
                  AND NOT "SilindiMi"
            ) THEN
                INSERT INTO "UrunSecenekleri" (
                    "UrunId",
                    "Olcu",
                    "CerceveTipi",
                    "SatisFiyati",
                    "MaliyetFiyati",
                    "FiyatFarki",
                    "StokAdedi",
                    "UretimSuresiGun",
                    "Sira",
                    "VarsayilanMi",
                    "AktifMi",
                    "SilindiMi",
                    "OnSipariseAcikMi",
                    "OlusturulmaTarihi"
                )
                VALUES (
                    12817,
                    v_boyut,
                    v_aydinlatma."Tip",
                    v_taban_fiyat + v_aydinlatma."Fark",
                    0,
                    v_aydinlatma."Fark",
                    100,
                    3,
                    v_aydinlatma."Sira",
                    false,
                    true,
                    false,
                    false,
                    NOW()
                );
            END IF;
        END LOOP;
    END LOOP;

    UPDATE "Urunler"
    SET "Fiyat" = 4800,
        "KdvOrani" = 20,
        "UretimSuresiGun" = 3
    WHERE "Id" = 12817;

    RAISE NOTICE '12817 urunu icin 24 varyant basariyla eklendi: %', v_urun."Baslik";
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
WHERE "UrunId" = 12817
  AND NOT "SilindiMi"
ORDER BY "Sira", "Id";
