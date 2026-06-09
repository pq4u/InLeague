# Specyfikacja endpointów REST API

Bazowy URL: `/api`

Konwencja odpowiedzi błędów:
```json
{ "error": "Opis błędu", "details": { "pole": ["komunikat"] } }
```

---

## Auth — `/api/auth`

### POST `/api/auth/register`
Rejestracja nowego użytkownika. Dostępne publicznie.

**Request body:**
```json
{
  "email": "jan.kowalski@example.com",
  "password": "Haslo123!",
  "firstName": "Jan",
  "lastName": "Kowalski"
}
```

**Walidacja:**
- `email` — wymagane, poprawny format email, unikalny
- `password` — wymagane, min 8 znaków, min 1 wielka litera, min 1 cyfra
- `firstName`, `lastName` — opcjonalne, max 50 znaków

**Response 201:**
```json
{
  "token": "eyJhbGci...",
  "expiresAt": "2024-12-01T14:30:00Z",
  "user": {
    "id": "uuid",
    "email": "jan.kowalski@example.com",
    "firstName": "Jan",
    "lastName": "Kowalski",
    "roles": ["User"]
  }
}
```

**Response 400** — email zajęty lub błędy walidacji.

---

### POST `/api/auth/login`
Logowanie. Dostępne publicznie.

**Request body:**
```json
{
  "email": "jan.kowalski@example.com",
  "password": "Haslo123!"
}
```

**Response 200:** (taka sama struktura jak przy rejestracji)

**Response 401:** nieprawidłowe dane logowania.

---

### GET `/api/auth/me`
Dane zalogowanego użytkownika. Wymaga tokenu JWT.

**Response 200:**
```json
{
  "id": "uuid",
  "email": "jan.kowalski@example.com",
  "firstName": "Jan",
  "lastName": "Kowalski",
  "roles": ["User"],
  "driverId": null
}
```

---

## Ligi — `/api/leagues`

### GET `/api/leagues`
Lista wszystkich lig. Dostępne publicznie.

**Query params:**
- `isActive` (bool, opcjonalny) — filtr po statusie aktywności

**Response 200:**
```json
[
  {
    "id": "uuid",
    "name": "Liga Zimowa 2024",
    "description": "Sezon zimowy",
    "startDate": "2024-01-15",
    "endDate": "2024-03-30",
    "isActive": true,
    "raceCount": 8,
    "createdAt": "2024-01-01T10:00:00Z"
  }
]
```

---

### GET `/api/leagues/{id}`
Szczegóły ligi z listą wyścigów. Dostępne publicznie.

**Response 200:**
```json
{
  "id": "uuid",
  "name": "Liga Zimowa 2024",
  "description": "Sezon zimowy",
  "startDate": "2024-01-15",
  "endDate": "2024-03-30",
  "isActive": true,
  "createdAt": "2024-01-01T10:00:00Z",
  "races": [
    {
      "id": "uuid",
      "name": "Runda 1",
      "location": "Tor Poznań",
      "raceDate": "2024-01-20T10:00:00Z",
      "numberOfLaps": 15,
      "resultCount": 12
    }
  ]
}
```

**Response 404** — liga nie istnieje.

---

### POST `/api/leagues`
Tworzenie ligi. Wymaga roli **Admin**.

**Request body:**
```json
{
  "name": "Liga Zimowa 2024",
  "description": "Sezon zimowy",
  "startDate": "2024-01-15",
  "endDate": "2024-03-30"
}
```

**Walidacja:**
- `name` — wymagane, 3–100 znaków
- `startDate` — wymagane
- `endDate` — opcjonalne, musi być po `startDate`

**Response 201** — zwraca utworzoną ligę.

---

### PUT `/api/leagues/{id}`
Aktualizacja ligi. Wymaga roli **Admin**.

**Request body:** (wszystkie pola opcjonalne — partial update)
```json
{
  "name": "Liga Zimowa 2024 — edycja",
  "isActive": false
}
```

**Response 200** — zwraca zaktualizowaną ligę.
**Response 404** — liga nie istnieje.

---

### DELETE `/api/leagues/{id}`
Usunięcie ligi. Wymaga roli **Admin**.

**Response 204** — brak treści.
**Response 404** — liga nie istnieje.
**Response 409** — liga posiada wyścigi (nie można usunąć).

---

## Wyścigi — `/api/leagues/{leagueId}/races`

### GET `/api/leagues/{leagueId}/races`
Lista wyścigów w lidze. Dostępne publicznie.

**Response 200:**
```json
[
  {
    "id": "uuid",
    "leagueId": "uuid",
    "name": "Runda 1",
    "location": "Tor Poznań",
    "raceDate": "2024-01-20T10:00:00Z",
    "numberOfLaps": 15,
    "notes": null,
    "resultCount": 12,
    "createdAt": "2024-01-10T12:00:00Z"
  }
]
```

---

### GET `/api/leagues/{leagueId}/races/{id}`
Szczegóły wyścigu z wynikami. Dostępne publicznie.

**Response 200:**
```json
{
  "id": "uuid",
  "leagueId": "uuid",
  "name": "Runda 1",
  "location": "Tor Poznań",
  "raceDate": "2024-01-20T10:00:00Z",
  "numberOfLaps": 15,
  "notes": null,
  "results": [
    {
      "id": "uuid",
      "driverId": "uuid",
      "driverName": "Jan Kowalski",
      "kartId": "uuid",
      "kartNumber": "7",
      "lapTimeMs": 94532,
      "totalTimeMs": 1417980,
      "startingPosition": 3,
      "finishingPosition": 1,
      "lapsCompleted": 15,
      "status": "Finished"
    }
  ]
}
```

---

### POST `/api/leagues/{leagueId}/races`
Tworzenie wyścigu. Wymaga roli **Admin**.

**Request body:**
```json
{
  "name": "Runda 1",
  "location": "Tor Poznań",
  "raceDate": "2024-01-20T10:00:00Z",
  "numberOfLaps": 15,
  "notes": null
}
```

**Walidacja:**
- `name` — wymagane, 3–100 znaków
- `raceDate` — wymagane
- `numberOfLaps` — wymagane, min 1

**Response 201.**
**Response 404** — liga nie istnieje.

---

### PUT `/api/leagues/{leagueId}/races/{id}`
Aktualizacja wyścigu. Wymaga roli **Admin**.

**Response 200** lub **404**.

---

### DELETE `/api/leagues/{leagueId}/races/{id}`
Usunięcie wyścigu. Wymaga roli **Admin**.

**Response 204** lub **404**.

---

## Zawodnicy — `/api/drivers`

### GET `/api/drivers`
Lista wszystkich zawodników. Dostępne publicznie.

**Response 200:**
```json
[
  {
    "id": "uuid",
    "firstName": "Jan",
    "lastName": "Kowalski",
    "fullName": "Jan Kowalski",
    "racingNumber": "7",
    "dateOfBirth": "1995-06-15",
    "createdAt": "2024-01-01T10:00:00Z"
  }
]
```

---

### GET `/api/drivers/{id}`
Szczegóły zawodnika. Dostępne publicznie.

**Response 200** — jak w liście, plus opcjonalne statystyki.
**Response 404.**

---

### POST `/api/drivers`
Tworzenie zawodnika. Wymaga roli **Admin**.

**Request body:**
```json
{
  "firstName": "Jan",
  "lastName": "Kowalski",
  "racingNumber": "7",
  "dateOfBirth": "1995-06-15"
}
```

**Walidacja:**
- `firstName`, `lastName` — wymagane, 2–50 znaków
- `racingNumber` — opcjonalne, max 10 znaków

**Response 201.**

---

### PUT `/api/drivers/{id}`
Aktualizacja zawodnika. Wymaga roli **Admin**.

**Response 200** lub **404**.

---

### DELETE `/api/drivers/{id}`
Usunięcie zawodnika. Wymaga roli **Admin**.

**Response 204** lub **404**.
**Response 409** — zawodnik ma wyniki (nie można usunąć).

---

## Gokarty — `/api/karts`

### GET `/api/karts`
Lista gokartów. Dostępne publicznie.

**Query params:**
- `isActive` (bool, opcjonalny)

**Response 200:**
```json
[
  {
    "id": "uuid",
    "number": "7",
    "model": "Rotax Max",
    "category": "Senior",
    "isActive": true
  }
]
```

---

### GET `/api/karts/{id}`

**Response 200** lub **404**.

---

### POST `/api/karts`
Wymaga roli **Admin**.

**Request body:**
```json
{
  "number": "7",
  "model": "Rotax Max",
  "category": "Senior"
}
```

**Walidacja:**
- `number` — wymagane, max 10 znaków, unikalny

**Response 201.**

---

### PUT `/api/karts/{id}`
Wymaga roli **Admin**. **Response 200** lub **404**.

---

### DELETE `/api/karts/{id}`
Wymaga roli **Admin**. **Response 204**, **404** lub **409** (ma wyniki).

---

## Wyniki — `/api/races/{raceId}/results`

### GET `/api/races/{raceId}/results`
Lista wyników wyścigu, posortowana po `finishingPosition`. Dostępne publicznie.

**Response 200:**
```json
[
  {
    "id": "uuid",
    "raceId": "uuid",
    "driverId": "uuid",
    "driverName": "Jan Kowalski",
    "kartId": "uuid",
    "kartNumber": "7",
    "lapTimeMs": 94532,
    "totalTimeMs": 1417980,
    "startingPosition": 3,
    "finishingPosition": 1,
    "lapsCompleted": 15,
    "status": "Finished",
    "notes": null
  }
]
```

---

### POST `/api/races/{raceId}/results`
Dodanie wyniku. Wymaga roli **Admin**.

**Request body:**
```json
{
  "driverId": "uuid",
  "kartId": "uuid",
  "lapTimeMs": 94532,
  "totalTimeMs": 1417980,
  "startingPosition": 3,
  "finishingPosition": 1,
  "lapsCompleted": 15,
  "status": "Finished",
  "notes": null
}
```

**Walidacja:**
- `driverId`, `kartId` — wymagane, muszą istnieć w bazie
- `lapTimeMs`, `totalTimeMs` — wymagane, min 0
- `startingPosition` — wymagane, min 1
- `finishingPosition` — opcjonalne (null = DNF), min 1
- `lapsCompleted` — wymagane, min 0
- Jeden zawodnik może mieć jeden wynik w jednym wyścigu (409 Conflict jeśli duplikat)

**Response 201.**

---

### PUT `/api/races/{raceId}/results/{id}`
Aktualizacja wyniku. Wymaga roli **Admin**.

**Response 200** lub **404**.

---

### DELETE `/api/races/{raceId}/results/{id}`
Usunięcie wyniku. Wymaga roli **Admin**.

**Response 204** lub **404**.

---

## Przykład kontrolera (LeaguesController)

```csharp
[ApiController]
[Route("api/[controller]")]
public class LeaguesController : ControllerBase
{
    private readonly ILeagueService _service;

    public LeaguesController(ILeagueService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool? isActive)
    {
        var leagues = await _service.GetAllAsync(isActive);
        return Ok(leagues);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var league = await _service.GetByIdAsync(id);
        return league is null ? NotFound() : Ok(league);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateLeagueDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLeagueDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _service.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }
}
```
