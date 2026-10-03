# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

**CANVASİA** (formerly *MeteorGaleri*) — ASP.NET Core 8.0 MVC e-commerce site for canvas wall art, backed by PostgreSQL. Turkish-language UI and Turkish-named entities/tables throughout.

Authoritative running reference: `PROJE_DOKUMANTASYONU.txt` (Turkish) — contains DB credentials, SQL restore commands, URL patterns, and known-issue notes. Consult it before guessing.

## Commands

### Local development (hybrid: DB in Docker, web on host)
```bash
docker-compose up -d db                 # Postgres only
cd KanvasProje.Web && dotnet watch run  # http://localhost:5002
```

### Full Docker stack (production-like)
```bash
docker-compose build --no-cache
docker-compose up -d                    # web on http://localhost:8080
```

### Restore the bundled SQL dump into the `db` container
```bash
# PowerShell
Get-Content kanvasdb_yedek.sql | docker exec -i kanvasproje-db psql -U kanvasuser -d kanvasdb
```

### Build / migrations / tailwind
```bash
dotnet build KanvasProje.sln
dotnet ef migrations add <Name> --project KanvasProje.Data --startup-project KanvasProje.Web
dotnet ef database update --project KanvasProje.Data --startup-project KanvasProje.Web
cd KanvasProje.Web && npm run watch:storefront-css   # Tailwind → wwwroot/css/storefront.css
```

There is **no test project** in the solution — don't fabricate `dotnet test` instructions.

## Architecture

Clean Architecture with four projects referenced top-down (Web → Service → Data → Core):

- **KanvasProje.Core** — Entities (`Varliklar/`), DTOs, interfaces, helpers. No framework dependencies beyond EF Core abstractions.
- **KanvasProje.Data** — `KanvasDbContext`, EF Core migrations (Npgsql), generic repository + UnitOfWork pattern.
- **KanvasProje.Service** — Business logic services, AutoMapper profiles, `SepetService` (DB-backed cart).
- **KanvasProje.Web** — MVC controllers, Razor views, Identity, an `Admin` Area, and startup pipeline in `Program.cs`.

### Runtime pipeline highlights (`KanvasProje.Web/Program.cs`)
- Loads `secrets.json` as an additional config source before env vars.
- Probes the Postgres connection at startup; if unreachable, **Hangfire and the `AbandonedCartService` hosted service are disabled** instead of crashing. Preserve this degraded-start behavior when touching startup code.
- On startup it runs `context.Database.Migrate()` **and** `EnsureMissingMarch2026SchemaAsync` — a hand-rolled idempotent `DO $$ ... $$` block that adds columns/tables to cover drift between the migration history and the expected March-2026 schema. If you add columns to `Urunler`, `Kategoriler`, `UrunSecenekleri`, or `UrunResimleri`, mirror them in that SQL block or it will diverge in prod.
- Identity uses `AppUser` + `IdentityRole`, Turkish error descriptions (`TurkceIdentityErrorDescriber`), 30-day sliding cookie, 5-try lockout.
- Admin-area auth redirects are intercepted to emit JSON 401/403 for `/api/admin/*` and log via `IAdminSecurityAuditService`.
- Rate limiter policies: `"auth"` (10/5min per IP) and `"general"` (100/min per IP).
- Global maintenance-mode middleware short-circuits non-admin/non-auth traffic based on `ISiteSettingsService`.
- HTTPS redirect is skipped when `DOTNET_RUNNING_IN_CONTAINER=true` (the reverse proxy handles TLS).
- Hangfire dashboard is mounted at `/admin/hangfire` with `LocalRequestsOnlyAuthorizationFilter`.

### Persistence conventions (PostgreSQL, quoted identifiers)

Tables and columns use **Turkish PascalCase** and require double quotes in raw SQL: `"Urunler"`, `"Kategoriler"`, `"UrunResimleri"`, `"SepetItems"`, `"AspNetUsers"`, etc.

Property names that are easy to get wrong — these are the canonical spellings:
- `Urun.Baslik` (product name, **not** `Ad`/`Name`), `Urun.Slug`, `Urun.Fiyat`, `Urun.IndirimliFiyat`, `Urun.EtkinFiyat`, `Urun.AnaGorselUrl`
- `UrunResim.ResimYolu` (image path, **not** `Url`/`ImageUrl`), `UrunResim.Sira`
- `Kategori.Ad`, `Kategori.Slug`

### URL patterns (public site)
`/Urun` (list) · `/Urun/Detay/{slug}-{id}` · `/Urun?k={kategoriId}` · `/Urun?s={arama}` · `/Sepet` · `/Siparis/Odeme` · `/Hesap/GirisYap` · `/Hesap/KayitOl` · `/Profil/*` · `/Favori` · `/Kurumsal/Iletisim` · `/admin/*` (Area).

### Payments
İyzico integration (`IyzicoPaymentService`) in **sandbox mode** — `ApiKey` in `appsettings.json` is a placeholder until production keys land.

## Configuration & secrets

- `KanvasProje.Web/secrets.json` — local dev DB connection string (gitignored in spirit; do not commit real values).
- `.env` / `.env.example` — docker-compose values: Postgres creds and Brevo SMTP.
- Default local connection: `Host=localhost;Port=5432;Database=kanvasdb;Username=kanvasuser;Password=changeme_in_production`.
- Docker container names: `kanvasproje-db`, `kanvasproje-web`. Volumes persist uploads (`/app/wwwroot/img/products`), media (`/app/wwwroot/media/products`), logs, and `App_Data`.

## Frontend

- Tailwind (local build via `npm run build:storefront-css`) writes to `wwwroot/css/storefront.css`. A Tailwind CDN is also referenced in some views.
- Brand palette lives in `tailwind.config.js` under `theme.extend.colors.canvasia`. Fonts: Cormorant Garamond (headings) + Manrope (body).
- `wwwroot/css/site.css` is **legacy** — `_Layout.cshtml` overrides its body padding with `!important`. Don't re-introduce those paddings.
