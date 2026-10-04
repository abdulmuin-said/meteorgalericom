# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

**MeteorGaleri** (`meteorgaleri.com`) — Independent ASP.NET Core 8.0 MVC / .NET 10 e-commerce platform for premium canvas and glass wall art, backed by PostgreSQL. Completely rebranded from CANVASIA with an authentic Retro / Vintage identity.

Authoritative running reference: `proje_tanitimi.md` (Turkish) — contains detailed architectural decisions, migration records, and step-by-step guides.

## Brand Identity & Aesthetic (Retro / Vintage)

- **Background (Body):** `#FFFBF0` (Warm cream)
- **Surface / Cards:** `#FDF6E3` (Warm sand)
- **Text / Ink:** `#1B2A4A` (Deep nostalgic navy)
- **Primary / Accent:** `#C0392B` (Retro brick / crimson red)
- **Deep Accent:** `#922B21` (Dark retro red)
- **Borders & Dividers:** `#F5E6C8` (Warm cream-beige)
- **Footer Bar:** `#131E35` (Deep midnight navy)
- **Typography:**
  - Headings: `Playfair Display` (Classic retro serif)
  - Body / UI: `Source Sans 3` (Clean readable sans-serif)

## Test Accounts & URLs

- **Public Site:** `http://localhost:5002`
- **Admin Panel:** `http://localhost:5002/Admin`
  - Email: `meteor_medya@hotmail.com`
  - Password: `MeteorAdmin2024!`
- **Customer Account:** `http://localhost:5002/Profil`
  - Email: `musteri@meteorgaleri.com`
  - Password: `MeteorUser2024!`

## Commands

### Local development (Linux / Host)
```bash
# PostgreSQL must be running (database: meteorgaleridb)
cd KanvasProje.Web && dotnet run --launch-profile http  # http://localhost:5002
```

### Restore database from clean dump
```bash
# Proje kökündeki 6.9 MB temiz dump (14.510 ürün, 223.856 seçenek, 5.143 yorum):
createdb -U postgres meteorgaleridb
# PostgreSQL 16+ or Docker:
docker run --rm --network host -e PGPASSWORD=muin6655 -v "$(pwd)":/backup postgres:17 pg_restore -h localhost -U postgres -d meteorgaleridb --no-owner --no-privileges -v /backup/meteorgaleridb_clean.dump
```

### Build & Tailwind
```bash
dotnet build KanvasProje.sln
cd KanvasProje.Web && npm run build:storefront-css   # Tailwind → wwwroot/css/storefront.css
```

There is **no test project** in the solution — don't fabricate `dotnet test` instructions.

## Architecture

Clean Architecture with four projects referenced top-down (Web → Service → Data → Core):

- **KanvasProje.Core** — Entities (`Varliklar/`), DTOs, interfaces, helpers.
- **KanvasProje.Data** — `KanvasDbContext`, EF Core migrations (Npgsql), generic repository + UnitOfWork pattern.
- **KanvasProje.Service** — Business logic services, AutoMapper profiles, `SepetService` (DB-backed cart), `BrevoApiEmailService`.
- **KanvasProje.Web** — MVC controllers, Razor views, Identity, `Admin` Area, and startup pipeline in `Program.cs`.

### Runtime pipeline highlights (`KanvasProje.Web/Program.cs`)
- Loads `secrets.json` before environment variables.
- Configures global `HtmlEncoder` with `UnicodeRanges.All` so Turkish characters in image URLs and SEO metadata don't get entity-encoded.
- Probes the Postgres connection at startup; if unreachable, Hangfire and background services degrade gracefully.
- Runs `context.Database.Migrate()` and `EnsureMissingMarch2026SchemaAsync`.
- Identity uses `AppUser` + `IdentityRole`, Turkish error descriptions (`TurkceIdentityErrorDescriber`), 30-day sliding cookie, 5-try lockout.
- Rate limiter policies: `"auth"` (10/5min per IP) and `"general"` (100/min per IP).
- Hangfire dashboard is mounted at `/admin/hangfire`.

### Persistence conventions (PostgreSQL, quoted identifiers)

Tables and columns use **Turkish PascalCase** and require double quotes in raw SQL: `"Urunler"`, `"Kategoriler"`, `"UrunResimleri"`, `"SepetItems"`, `"AspNetUsers"`, etc.

Key property names:
- `Urun.Baslik` (product name, **not** `Ad`/`Name`), `Urun.Slug`, `Urun.Fiyat`, `Urun.IndirimliFiyat`, `Urun.EtkinFiyat`, `Urun.AnaGorselUrl`
- `UrunResim.ResimYolu` (image path, **not** `Url`/`ImageUrl`), `UrunResim.Sira`
- `Kategori.Ad`, `Kategori.Slug`

### Configuration & secrets

- `KanvasProje.Web/secrets.json` — gitignored local connection string and credentials.
- Local DB: `Host=localhost;Port=5432;Database=meteorgaleridb;Username=postgres;Password=muin6655`.
- Production: Railway managed PostgreSQL and environment variables.
