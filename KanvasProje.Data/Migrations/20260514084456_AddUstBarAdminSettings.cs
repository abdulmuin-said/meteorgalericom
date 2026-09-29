using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KanvasProje.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUstBarAdminSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
ALTER TABLE public.""SiteAyarlari""
ADD COLUMN IF NOT EXISTS ""UstBarEtkin"" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE public.""SiteAyarlari""
ADD COLUMN IF NOT EXISTS ""UstBarHizi"" double precision NOT NULL DEFAULT 0.0;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
ALTER TABLE public.""SiteAyarlari""
DROP COLUMN IF EXISTS ""UstBarEtkin"";

ALTER TABLE public.""SiteAyarlari""
DROP COLUMN IF EXISTS ""UstBarHizi"";");
        }
    }
}
