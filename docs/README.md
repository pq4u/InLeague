# Karting League Management System

## Przegląd projektu

System do zarządzania ligą kartingową umożliwiający tworzenie lig, zarządzanie wyścigami, rejestrowanie wyników i przeglądanie statystyk. Projekt studencki.

## Stack technologiczny

| Warstwa | Technologia |
|---|---|
| Frontend | Angular 17+ (standalone components) |
| Backend | ASP.NET Core 8 Web API |
| Baza danych | PostgreSQL 16 |
| ORM | Entity Framework Core 8 + Npgsql |
| Autentykacja | JWT Bearer tokens |
| Dokumentacja API | Swagger / OpenAPI |
| Konteneryzacja | Docker + Docker Compose |

## Struktura repozytorium

```
karting-league/
├── backend/
│   └── KartingLeague.Api/          # ASP.NET Core Web API
│       ├── Controllers/
│       ├── Services/
│       ├── Repositories/
│       ├── Models/
│       ├── DTOs/
│       ├── Migrations/
│       └── ...
├── frontend/
│   └── karting-league-app/         # Angular application
│       ├── src/
│       │   ├── app/
│       │   │   ├── core/           # guards, interceptors, services
│       │   │   ├── features/       # ligi, wyscigi, wyniki, zawodnicy
│       │   │   └── shared/         # wspólne komponenty
│       │   └── ...
│       └── ...
├── docker-compose.yml
└── README.md
```

## Pliki specyfikacji dla agenta

Czytaj w tej kolejności:

1. `docs/01-data-model.md` — encje, pola, typy, relacje
2. `docs/02-backend-spec.md` — struktura projektu .NET, serwisy, repozytoria
3. `docs/03-api-endpoints.md` — wszystkie endpointy REST z przykładami JSON
4. `docs/04-auth-spec.md` — JWT, role, polityki autoryzacji
5. `docs/05-frontend-spec.md` — struktura Angular, serwisy, komponenty, routing
6. `docs/06-docker-spec.md` — Docker Compose, zmienne środowiskowe
7. `docs/07-implementation-tasks.md` — lista zadań w kolejności implementacji

## Zasady ogólne dla agenta

- Wszystkie nazwy klas, metod i właściwości w backendzie — po angielsku (PascalCase)
- Nazwy endpointów API — po angielsku, małe litery, kebab-case (`/api/race-results`)
- Komentarze i komunikaty walidacji — po polsku
- Każdy kontroler ma odpowiadający serwis i repozytorium
- Nigdy nie zwracaj modeli domenowych bezpośrednio z API — zawsze używaj DTO
- Wszystkie operacje zapisu/odczytu do bazy przez EF Core są asynchroniczne (`async/await`)
- Błędy HTTP: 400 Bad Request dla walidacji, 401 Unauthorized, 403 Forbidden, 404 Not Found, 500 dla nieobsłużonych wyjątków
