# ==========================================
# Stage 1: Build
# ==========================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Proje dosyalarını kopyala ve restore et (cache optimizasyonu)
COPY KanvasProje.Core/KanvasProje.Core.csproj KanvasProje.Core/
COPY KanvasProje.Data/KanvasProje.Data.csproj KanvasProje.Data/
COPY KanvasProje.Service/KanvasProje.Service.csproj KanvasProje.Service/
COPY KanvasProje.Web/KanvasProje.Web.csproj KanvasProje.Web/
COPY KanvasProje.sln .

RUN dotnet restore KanvasProje.sln

# Tüm kaynak kodunu kopyala ve publish et
COPY . .
RUN dotnet publish KanvasProje.Web/KanvasProje.Web.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ==========================================
# Stage 2: Runtime
# ==========================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

# Gerekli klasörleri oluştur. Railway'de kalıcı klasörler entrypoint içinde
# /app/storage altına bağlanır; bu klasörler image içindeki başlangıç içeriği taşır.
RUN mkdir -p /app/logs /app/App_Data /app/wwwroot/uploads /app/wwwroot/img/products /app/wwwroot/media/products/videos

# Publish çıktısını kopyala
COPY --from=build /app/publish .
COPY docker-entrypoint.sh /docker-entrypoint.sh
RUN chmod +x /docker-entrypoint.sh

# Ortam değişkenleri
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_EnableDiagnostics=0
ENV PERSISTENT_ROOT=/app/storage

EXPOSE 8080

# Health check - Railway PORT değişkenini, yoksa yerel 8080'i kullanır.
HEALTHCHECK --interval=30s --timeout=5s --retries=3 \
    CMD curl -f "http://localhost:${PORT:-8080}/health" || exit 1

ENTRYPOINT ["/docker-entrypoint.sh"]
