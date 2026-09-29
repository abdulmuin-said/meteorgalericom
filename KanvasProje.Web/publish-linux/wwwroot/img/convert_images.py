import os
from PIL import Image

# Klasör Yolları (Kendi yoluna göre düzenle)
# Örn: C:/Projeler/Canvasia/wwwroot/img/products_yeni
INPUT_FOLDER = "products_yeni"   
OUTPUT_FOLDER = "products_webp"

# Çıktı klasörü yoksa oluştur
if not os.path.exists(OUTPUT_FOLDER):
    os.makedirs(OUTPUT_FOLDER)

print("🚀 Dönüştürme işlemi başlıyor...")

count = 0
errors = 0

# Klasördeki tüm dosyaları gez
for filename in os.listdir(INPUT_FOLDER):
    if filename.lower().endswith(('.jpg', '.jpeg', '.png')):
        try:
            # Dosya yolları
            input_path = os.path.join(INPUT_FOLDER, filename)
            
            # Yeni dosya adı (uzantıyı .webp yap)
            file_name_without_ext = os.path.splitext(filename)[0]
            new_filename = f"{file_name_without_ext}.webp"
            output_path = os.path.join(OUTPUT_FOLDER, new_filename)

            # Resmi aç ve dönüştür
            with Image.open(input_path) as img:
                # WebP olarak kaydet (quality=80 hem boyut küçültür hem kaliteyi korur)
                img.save(output_path, "WEBP", quality=80, method=6)
            
            print(f"✅ Dönüştürüldü: {filename} -> {new_filename}")
            count += 1
            
        except Exception as e:
            print(f"❌ HATA ({filename}): {e}")
            errors += 1

print("-" * 30)
print(f"🎉 İşlem Tamamlandı!")
print(f"toplam Başarılı: {count}")
print(f"toplam Hata: {errors}")