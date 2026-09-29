SELECT
  (SELECT COUNT(*) FROM "Urunler") as urunler,
  (SELECT COUNT(*) FROM "Kategoriler") as kategoriler,
  (SELECT COUNT(*) FROM "UrunResimleri") as resimler,
  (SELECT COUNT(*) FROM "UrunSecenekleri") as secenekler;
