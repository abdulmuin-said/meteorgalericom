-- Ölçüleri kontrol - sayısal olarak doğru mu?
SELECT "Olcu", COUNT(*) as adet 
FROM "UrunSecenekleri" 
WHERE "Olcu" NOT LIKE '%Parçalı%' AND "Olcu" NOT LIKE '%Tek Parçalı%'
GROUP BY "Olcu" 
ORDER BY adet DESC;