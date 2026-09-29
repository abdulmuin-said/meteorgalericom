"""
UrunSecenekleri + Yorumlar + SenTasarla — COPY formatında dump.
--column-inserts yok, timeout 1800s.
"""
import subprocess, os

PGPASSWORD = "SdnZSAfjFNqdAOKxTtrKSrXJfdmENCLm"
HOST = "reseau.proxy.rlwy.net"
PORT = "47970"
USER = "postgres"
DB = "railway"
OUT = r"C:\meteorgalericom\meteorgaleri_data_secenek.sql"
ENV = {**os.environ, "PGPASSWORD": PGPASSWORD}

TABLES = ["UrunSecenekleri", "Yorumlar", "SenTasarla"]

def run_pgdump(table):
    proc = subprocess.run(
        [
            "pg_dump",
            "-h", HOST, "-p", PORT, "-U", USER, "-d", DB,
            "--data-only", "--no-owner", "--no-acl",
            # --column-inserts YOK: COPY formatı ~10x hızlı
            "-t", f'public."{table}"',
        ],
        capture_output=True, env=ENV, timeout=1800
    )
    out = proc.stdout.decode("utf-8", errors="replace") if proc.stdout else ""
    err = proc.stderr.decode("utf-8", errors="replace") if proc.stderr else ""
    return proc.returncode, out, err

print(f"-> {OUT}")
with open(OUT, "w", encoding="utf-8") as f:
    f.write("SET session_replication_role = replica;\n\n")
    for table in TABLES:
        print(f"  [{table}] dumping...", flush=True)
        rc, out, err = run_pgdump(table)
        if rc != 0 or not out.strip():
            print(f"  HATA: {err[:200]}")
        else:
            f.write(f"\n-- TABLE: {table}\n")
            f.write(out)
            print(f"  yazildi ({len(out)//1024}KB)", flush=True)
    f.write("\nSET session_replication_role = DEFAULT;\n")

size = os.path.getsize(OUT) / 1024 / 1024
print(f"Tamamlandi: {size:.1f} MB")
