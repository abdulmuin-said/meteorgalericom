"""
UrunSecenekleri ve Yorumlar — column-inserts ile dump.
Id GENERATED ALWAYS sorununu INSERT ... OVERRIDING SYSTEM VALUE ile çöz.
"""
import subprocess, os, re

PGPASSWORD = "SdnZSAfjFNqdAOKxTtrKSrXJfdmENCLm"
HOST = "reseau.proxy.rlwy.net"
PORT = "47970"
USER = "postgres"
DB = "railway"
ENV = {**os.environ, "PGPASSWORD": PGPASSWORD}

LOCAL_PGPASSWORD = "h3ker6655?"
LOCAL_ENV = {**os.environ, "PGPASSWORD": LOCAL_PGPASSWORD}

TABLES = ["UrunSecenekleri", "Yorumlar"]

for table in TABLES:
    out_file = rf"C:\meteorgalericom\import_{table}.sql"
    print(f"[{table}] dump...", flush=True)

    proc = subprocess.run(
        [
            "pg_dump",
            "-h", HOST, "-p", PORT, "-U", USER, "-d", DB,
            "--data-only", "--no-owner", "--no-acl",
            "--column-inserts",
            "-t", f'public."{table}"',
        ],
        capture_output=True, env=ENV, timeout=1200
    )
    out = proc.stdout.decode("utf-8", errors="replace")
    err = proc.stderr.decode("utf-8", errors="replace")

    if proc.returncode != 0 or not out.strip():
        print(f"  HATA: {err[:300]}")
        continue

    # GENERATED ALWAYS identity fix:
    # INSERT INTO "T" ("Id", ...) VALUES (1, ...)
    # → INSERT INTO "T" OVERRIDING SYSTEM VALUE ("Id", ...) VALUES (1, ...)
    # Sadece bu tablo için gerekli
    out = re.sub(
        r'(INSERT INTO "[^"]*" \()',
        r'INSERT INTO "\g<0>'.replace('INSERT INTO "', ''),  # bypass — direkt replace
        out
    )
    # Daha basit: tüm INSERT INTO satırlarına OVERRIDING SYSTEM VALUE ekle
    lines = out.split("\n")
    fixed = []
    for line in lines:
        if line.strip().startswith("INSERT INTO") and "OVERRIDING SYSTEM VALUE" not in line:
            # INSERT INTO "Table" ("Col"...) VALUES (...);
            # → INSERT INTO "Table" OVERRIDING SYSTEM VALUE ("Col"...) VALUES (...);
            line = line.replace(') VALUES (', ') OVERRIDING SYSTEM VALUE VALUES (', 1)
            # Aslında syntax: INSERT INTO t OVERRIDING SYSTEM VALUE VALUES(...)
            # Ama column-inserts: INSERT INTO t (cols) OVERRIDING SYSTEM VALUE VALUES (...)
            # PostgreSQL 10+ destekler
        fixed.append(line)
    out = "\n".join(fixed)

    with open(out_file, "w", encoding="utf-8") as f:
        f.write(out)
    size = os.path.getsize(out_file) / 1024
    print(f"  yazildi: {size:.0f}KB -> {out_file}", flush=True)

    # Direkt import et
    print(f"  [import basliyor]", flush=True)
    imp = subprocess.run(
        ["psql", "-h", "localhost", "-p", "5432", "-U", "postgres",
         "-d", "meteorgaleridb", "-v", "ON_ERROR_STOP=0", "-f", out_file],
        capture_output=True, env=LOCAL_ENV, timeout=600
    )
    imp_out = imp.stdout.decode("utf-8", errors="replace")
    imp_err = imp.stderr.decode("utf-8", errors="replace")

    # Son birkaç satır
    last = (imp_out + imp_err).strip().split("\n")
    for l in last[-5:]:
        print(f"  {l}", flush=True)
    print(f"  exit: {imp.returncode}", flush=True)

print("Tamamlandi")
