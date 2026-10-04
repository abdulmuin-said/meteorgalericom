#!/usr/bin/env bash
# ==============================================================================
# MeteorGaleri — Yerel Veritabanını Railway PostgreSQL'e Aktarma Betiği
# Kullanım: ./railway_db_yukle.sh "<RAILWAY_DATABASE_PUBLIC_URL>"
# Örnek:    ./railway_db_yukle.sh "postgresql://postgres:pass@roundhouse.proxy.rlwy.net:12345/railway"
# ==============================================================================
set -euo pipefail

if [ -z "${1:-}" ]; then
  echo "❌ HATA: Railway PostgreSQL bağlantı linkini girmelisiniz!"
  echo "Kullanım: ./railway_db_yukle.sh \"<RAILWAY_DATABASE_PUBLIC_URL>\""
  echo ""
  echo "İpucu: Railway panelinde PostgreSQL servisine tıklayın -> 'Connect' sekmesindeki"
  echo "'Public Networking' (veya Postgres Connection URL) linkini buraya yapıştırın."
  exit 1
fi

RAILWAY_URL="$1"
LOCAL_DB="meteorgaleridb"
LOCAL_USER="postgres"
export PGPASSWORD="${PGPASSWORD:-muin6655}"

echo "======================================================================"
echo "🚀 MeteorGaleri Veritabanı Railway'e Aktarılıyor..."
echo "Yerel DB : $LOCAL_DB"
echo "Hedef URL: $RAILWAY_URL"
echo "======================================================================"

# 1. Yerel veritabanından dump alıp doğrudan Railway'e basma
echo "📦 1/2: Veritabanı yedekleniyor ve Railway'e aktarılıyor (14.500+ ürün, seçenekler, yorumlar)..."

pg_dump -h localhost -U "$LOCAL_USER" -d "$LOCAL_DB" \
  --no-owner \
  --no-acl \
  --clean \
  --if-exists | psql "$RAILWAY_URL"

echo ""
echo "✅ 2/2: Aktarım başarıyla tamamlandı!"
echo "Tüm tablolar, ürünler, resimler ve yorumlar Railway veritabanına yüklendi."
echo "======================================================================"
