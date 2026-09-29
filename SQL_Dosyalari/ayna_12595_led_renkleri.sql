BEGIN;

DO $$
DECLARE
    v_urun RECORD;
    v_boyut text;
    v_sira integer;
    v_adet integer;
    v_beyaz RECORD;
    v_renk text;
    v_renk_sira integer;
BEGIN
    SELECT "Id", "Baslik"
    INTO v_urun
    FROM "Urunler"
    WHERE "Id" = 12595
      AND NOT "SilindiMi";

    IF NOT FOUND THEN
        RAISE EXCEPTION '12595 ID numarali urun bulunamadi.';
    END IF;

    IF v_urun."Baslik" NOT ILIKE '%Instagram%'
       OR v_urun."Baslik" NOT ILIKE '%Ayna%' THEN
        RAISE EXCEPTION 'Urun dogrulanamadi. Bulunan urun: %', v_urun."Baslik";
    END IF;

    FOR v_boyut, v_sira IN
        SELECT *
        FROM (VALUES
            ('30x60 cm', 10),
            ('40x80 cm', 20),
            ('50x100 cm', 30),
            ('60x120 cm', 40),
            ('70x140 cm', 50),
            ('80x160 cm', 60)
        ) AS olculer("Olcu", "Sira")
    LOOP
        SELECT COUNT(*)
        INTO v_adet
        FROM "UrunSecenekleri"
        WHERE "UrunId" = 12595
          AND "Olcu" = v_boyut
          AND COALESCE("CerceveTipi", '') = ''
          AND NOT "SilindiMi";

        IF v_adet <> 1 THEN
            RAISE EXCEPTION '% LEDsiz varyasyonu icin 1 kayit bekleniyordu, bulunan: %', v_boyut, v_adet;
        END IF;

        UPDATE "UrunSecenekleri"
        SET "Sira" = v_sira
        WHERE "UrunId" = 12595
          AND "Olcu" = v_boyut
          AND COALESCE("CerceveTipi", '') = ''
          AND NOT "SilindiMi";

        SELECT COUNT(*)
        INTO v_adet
        FROM "UrunSecenekleri"
        WHERE "UrunId" = 12595
          AND "Olcu" = v_boyut
          AND "CerceveTipi" IN ('LED''li', 'Beyaz LED')
          AND NOT "SilindiMi";

        IF v_adet <> 1 THEN
            RAISE EXCEPTION '% icin tek bir LEDli veya Beyaz LED kaydi bekleniyordu, bulunan: %', v_boyut, v_adet;
        END IF;

        UPDATE "UrunSecenekleri"
        SET "CerceveTipi" = 'Beyaz LED',
            "Sira" = v_sira + 1,
            "AktifMi" = true
        WHERE "UrunId" = 12595
          AND "Olcu" = v_boyut
          AND "CerceveTipi" IN ('LED''li', 'Beyaz LED')
          AND NOT "SilindiMi";

        SELECT *
        INTO v_beyaz
        FROM "UrunSecenekleri"
        WHERE "UrunId" = 12595
          AND "Olcu" = v_boyut
          AND "CerceveTipi" = 'Beyaz LED'
          AND NOT "SilindiMi";

        FOR v_renk, v_renk_sira IN
            SELECT *
            FROM (VALUES
                ('Günışığı LED', v_sira + 2),
                ('Amber LED', v_sira + 3)
            ) AS renkler("Renk", "Sira")
        LOOP
            SELECT COUNT(*)
            INTO v_adet
            FROM "UrunSecenekleri"
            WHERE "UrunId" = 12595
              AND "Olcu" = v_boyut
              AND "CerceveTipi" = v_renk
              AND NOT "SilindiMi";

            IF v_adet > 1 THEN
                RAISE EXCEPTION '% / % icin birden fazla aktif varyasyon bulundu.', v_boyut, v_renk;
            ELSIF v_adet = 1 THEN
                UPDATE "UrunSecenekleri"
                SET "SatisFiyati" = v_beyaz."SatisFiyati",
                    "MaliyetFiyati" = v_beyaz."MaliyetFiyati",
                    "FiyatFarki" = v_beyaz."FiyatFarki",
                    "StokAdedi" = GREATEST("StokAdedi", 100),
                    "AktifMi" = true,
                    "Sira" = v_renk_sira
                WHERE "UrunId" = 12595
                  AND "Olcu" = v_boyut
                  AND "CerceveTipi" = v_renk
                  AND NOT "SilindiMi";
            ELSE
                INSERT INTO "UrunSecenekleri" (
                    "UrunId",
                    "Olcu",
                    "CerceveTipi",
                    "SatisFiyati",
                    "MaliyetFiyati",
                    "StokAdedi",
                    "OlusturulmaTarihi",
                    "SilindiMi",
                    "AktifMi",
                    "CerceveKalinligi",
                    "CerceveRengi",
                    "Desi",
                    "FiyatFarki",
                    "GorselUrl",
                    "KisilestirmeMetni",
                    "MalzemeTuru",
                    "OnSipariseAcikMi",
                    "OzelTasarimNotu",
                    "ParcaSayisi",
                    "Sira",
                    "TukeninceGizle",
                    "UretimSuresiGun",
                    "VaryantSku",
                    "VarsayilanMi",
                    "Yon"
                )
                VALUES (
                    12595,
                    v_boyut,
                    v_renk,
                    v_beyaz."SatisFiyati",
                    v_beyaz."MaliyetFiyati",
                    100,
                    NOW(),
                    false,
                    true,
                    v_beyaz."CerceveKalinligi",
                    v_beyaz."CerceveRengi",
                    v_beyaz."Desi",
                    v_beyaz."FiyatFarki",
                    v_beyaz."GorselUrl",
                    v_beyaz."KisilestirmeMetni",
                    v_beyaz."MalzemeTuru",
                    v_beyaz."OnSipariseAcikMi",
                    v_beyaz."OzelTasarimNotu",
                    v_beyaz."ParcaSayisi",
                    v_renk_sira,
                    v_beyaz."TukeninceGizle",
                    v_beyaz."UretimSuresiGun",
                    '',
                    false,
                    v_beyaz."Yon"
                );
            END IF;
        END LOOP;
    END LOOP;

    RAISE NOTICE 'LED renk varyasyonlari basariyla guncellendi: %', v_urun."Baslik";
END
$$;

COMMIT;

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
