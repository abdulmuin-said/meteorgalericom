import json
import os
import datetime

# Dosya adları
json_dosyasi = 'urunler_kategorili_final_tam.json'
sql_dosyasi = 'veritabani_guncelleme.sql'

def sql_olustur():
    print("⏳ SQL dosyası hazırlanıyor...")
    
    if not os.path.exists(json_dosyasi):
        print(f"HATA: '{json_dosyasi}' dosyası bulunamadı! Lütfen dosyanın bu script ile aynı klasörde olduğundan emin ol.")
        return

    with open(json_dosyasi, 'r', encoding='utf-8') as f:
        urunler = json.load(f)

    sql_komutlari = []
    
    # Başlangıç notu
    sql_komutlari.append("-- BU DOSYA PYTHON SCRIPT İLE OTOMATİK OLUŞTURULMUŞTUR")
    sql_komutlari.append(f"-- Oluşturulma Tarihi: {datetime.datetime.now()}")
    sql_komutlari.append("-- ---------------------------------------------------------")

    sayac = 0
    for urun in urunler:
        baslik = urun.get('Baslik', '').replace("'", "''") # Tek tırnakları SQL için kaçır
        
        if not baslik:
            continue

        # 1. TEMİZLİK: Önce bu ürünün eski varyantlarını ve resimlerini temizle (Çakışma olmasın)
        # Not: Ürünleri ID ile değil Başlık ile buluyoruz çünkü JSON'da ID yok.
        sql_komutlari.append(f"\n-- Ürün: {baslik}")
        sql_komutlari.append(f"DELETE FROM \"UrunSecenekleri\" WHERE \"UrunId\" IN (SELECT \"Id\" FROM \"Urunler\" WHERE \"Baslik\" = '{baslik}');")
        sql_komutlari.append(f"DELETE FROM \"UrunResimleri\" WHERE \"UrunId\" IN (SELECT \"Id\" FROM \"Urunler\" WHERE \"Baslik\" = '{baslik}');")

        # 2. VARYANTLAR (SEÇENEKLER) EKLEME
        varyantlar = urun.get('Varyantlar', [])
        en_dusuk_fiyat = 999999.0
        
        for v in varyantlar:
            olcu = v.get('Olcu', '').replace("'", "''")
            
            # Fiyatı sayıya çevir (Virgül/nokta kontrolü)
            fiyat_raw = v.get('Fiyat', 0)
            if isinstance(fiyat_raw, str):
                fiyat_raw = float(fiyat_raw.replace(',', '.'))
            fiyat = float(fiyat_raw)
            
            # Maliyet hesabı (Örn: Satış fiyatının %60'ı maliyet olsun)
            maliyet = round(fiyat * 0.6, 2)
            
            # En düşük fiyatı takip et (Ana tabloya yazacağız)
            if fiyat < en_dusuk_fiyat and fiyat > 0:
                en_dusuk_fiyat = fiyat

            sql_komutlari.append(f"""
INSERT INTO "UrunSecenekleri" ("UrunId", "Olcu", "SatisFiyati", "StokAdedi", "SilindiMi", "OlusturulmaTarihi", "CerceveTipi", "MaliyetFiyati")
SELECT "Id", '{olcu}', {fiyat}, 100, false, NOW(), 'Standart', {maliyet}
FROM "Urunler" WHERE "Baslik" = '{baslik}';""")

        # 3. RESİMLERİ EKLEME
        # Ana resim
        ana_resim = urun.get('Resim', '')
        if ana_resim:
            sql_komutlari.append(f"""
INSERT INTO "UrunResimleri" ("UrunId", "ResimYolu", "VarsayilanMi", "SilindiMi", "OlusturulmaTarihi")
SELECT "Id", '{ana_resim}', true, false, NOW()
FROM "Urunler" WHERE "Baslik" = '{baslik}';""")

        # Diğer resimler
        diger_resimler = urun.get('DigerResimler', [])
        for img in diger_resimler:
            if img == ana_resim: continue # Aynısı varsa atla
            sql_komutlari.append(f"""
INSERT INTO "UrunResimleri" ("UrunId", "ResimYolu", "VarsayilanMi", "SilindiMi", "OlusturulmaTarihi")
SELECT "Id", '{img}', false, false, NOW()
FROM "Urunler" WHERE "Baslik" = '{baslik}';""")

        # 4. ANA FİYAT GÜNCELLEME (Ürün listeleme sayfası için kritik!)
        if en_dusuk_fiyat < 999999.0:
            sql_komutlari.append(f"UPDATE \"Urunler\" SET \"Fiyat\" = {en_dusuk_fiyat} WHERE \"Baslik\" = '{baslik}';")
        
        sayac += 1

    # Dosyayı kaydet
    with open(sql_dosyasi, 'w', encoding='utf-8') as f:
        f.write('\n'.join(sql_komutlari))

    print(f"✅ İŞLEM TAMAM! '{sql_dosyasi}' dosyası oluşturuldu.")
    print(f"Toplam {sayac} ürün için SQL komutları hazırlandı.")
    print("Şimdi bu .sql dosyasını pgAdmin Query Tool'da çalıştırabilirsin.")

if __name__ == "__main__":
    sql_olustur()