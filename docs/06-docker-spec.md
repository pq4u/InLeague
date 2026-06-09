# Specyfikacja Docker i Docker Compose

## Pliki do utworzenia

```
karting-league/
├── docker-compose.yml
├── docker-compose.override.yml      # dev overrides
├── backend/
│   └── KartingLeague.Api/
│       └── Dockerfile
└── frontend/
    └── karting-league-app/
        ├── Dockerfile
        └── nginx.conf
```

---

## docker-compose.yml

```yaml
version: '3.9'

services:

  db:
    image: postgres:16-alpine
    container_name: karting_db
    restart: unless-stopped
    environment:
      POSTGRES_DB:       karting_league
      POSTGRES_USER:     postgres
      POSTGRES_PASSWORD: ${DB_PASSWORD:-postgres}
    volumes:
      - postgres_data:/var/lib/postgresql/data
    ports:
      - "5432:5432"
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 10s
      timeout: 5s
      retries: 5

  api:
    build:
      context: ./backend/KartingLeague.Api
      dockerfile: Dockerfile
    container_name: karting_api
    restart: unless-stopped
    depends_on:
      db:
        condition: service_healthy
    environment:
      ASPNETCORE_ENVIRONMENT:          Production
      ConnectionStrings__DefaultConnection: >
        Host=db;Port=5432;Database=karting_league;
        Username=postgres;Password=${DB_PASSWORD:-postgres}
      Jwt__Key:               ${JWT_KEY:?JWT_KEY env var is required}
      Jwt__Issuer:            KartingLeagueApi
      Jwt__Audience:          KartingLeagueFrontend
      Jwt__ExpirationMinutes: 60
    ports:
      - "5000:8080"

  frontend:
    build:
      context: ./frontend/karting-league-app
      dockerfile: Dockerfile
    container_name: karting_frontend
    restart: unless-stopped
    depends_on:
      - api
    ports:
      - "4200:80"

volumes:
  postgres_data:
```

---

## docker-compose.override.yml (dev)

```yaml
version: '3.9'

services:
  api:
    environment:
      ASPNETCORE_ENVIRONMENT: Development
    volumes:
      - ./backend/KartingLeague.Api:/app
    # Hot reload w trybie dev — uruchom przez: dotnet watch run

  db:
    ports:
      - "5432:5432"   # dostęp do bazy z hosta podczas dev
```

---

## Dockerfile — backend

```dockerfile
# backend/KartingLeague.Api/Dockerfile

# --- Etap 1: Build ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Kopiuj plik projektu i przywróć zależności (cache layer)
COPY *.csproj ./
RUN dotnet restore

# Kopiuj resztę kodu i zbuduj
COPY . .
RUN dotnet publish -c Release -o /app/publish --no-restore

# --- Etap 2: Runtime ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Utwórz użytkownika bez uprawnień roota
RUN adduser --disabled-password --gecos '' appuser
USER appuser

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "KartingLeague.Api.dll"]
```

---

## Dockerfile — frontend

```dockerfile
# frontend/karting-league-app/Dockerfile

# --- Etap 1: Build ---
FROM node:20-alpine AS build
WORKDIR /app

# Kopiuj package.json i zainstaluj zależności (cache layer)
COPY package*.json ./
RUN npm ci

# Kopiuj kod i zbuduj produkcyjnie
COPY . .
RUN npm run build -- --configuration production

# --- Etap 2: Nginx ---
FROM nginx:1.25-alpine
COPY --from=build /app/dist/karting-league-app/browser /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

---

## nginx.conf

```nginx
server {
    listen 80;
    server_name localhost;
    root /usr/share/nginx/html;
    index index.html;

    # Przekazuj /api do backendu
    location /api/ {
        proxy_pass         http://api:8080;
        proxy_http_version 1.1;
        proxy_set_header   Upgrade $http_upgrade;
        proxy_set_header   Connection keep-alive;
        proxy_set_header   Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto $scheme;
    }

    # Angular SPA — wszystkie trasy kieruj do index.html
    location / {
        try_files $uri $uri/ /index.html;
    }

    # Gzip
    gzip on;
    gzip_types text/plain text/css application/json application/javascript text/xml;
}
```

> Gdy nginx proxy jest aktywne, zmień `apiUrl` w `environment.production.ts` na `/api` (relatywny URL).

---

## Plik .env (nie commituj do git)

```env
# .env
DB_PASSWORD=bezpieczne_haslo_do_bazy
JWT_KEY=BARDZO_DLUGI_LOSOWY_KLUCZ_MINIMUM_32_ZNAKI_ZMIEN_TO
```

Dodaj do `.gitignore`:
```
.env
.env.local
```

---

## Uruchamianie

### Development (bez Dockera)

```bash
# 1. Uruchom tylko bazę danych przez Docker
docker compose up db -d

# 2. Backend (terminal 1)
cd backend/KartingLeague.Api
dotnet watch run

# 3. Frontend (terminal 2)
cd frontend/karting-league-app
ng serve
```

### Produkcja (pełny Docker Compose)

```bash
# Zbuduj i uruchom wszystko
docker compose up --build -d

# Sprawdź logi
docker compose logs -f api
docker compose logs -f frontend

# Zatrzymaj
docker compose down

# Zatrzymaj i usuń wolumeny (reset bazy)
docker compose down -v
```

### Migracje

```bash
# Dodaj migrację (lokalnie, wymaga dotnet-ef)
dotnet tool install --global dotnet-ef
cd backend/KartingLeague.Api
dotnet ef migrations add InitialCreate
dotnet ef database update

# Przez Docker (gdy kontener działa)
docker compose exec api dotnet ef database update
```

---

## .gitignore (fragmenty)

```gitignore
# .NET
bin/
obj/
*.user
appsettings.*.json
!appsettings.json
!appsettings.Development.json

# Angular
node_modules/
dist/
.angular/

# Docker / env
.env
.env.*
docker-compose.override.yml

# IDE
.vscode/
.idea/
*.suo
```
