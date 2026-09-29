-- Add AnnouncementBar settings to SiteAyarlari table
ALTER TABLE "SiteAyarlari" ADD COLUMN IF NOT EXISTS "UstBarEtkin" boolean NOT NULL DEFAULT true;
ALTER TABLE "SiteAyarlari" ADD COLUMN IF NOT EXISTS "UstBarHizi" double precision NOT NULL DEFAULT 34;

-- Update existing rows with default values
UPDATE "SiteAyarlari" SET "UstBarEtkin" = true WHERE "UstBarEtkin" IS NULL;
UPDATE "SiteAyarlari" SET "UstBarHizi" = 34 WHERE "UstBarHizi" IS NULL;