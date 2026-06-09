# Specyfikacja frontendu — Angular 17+

## Inicjalizacja projektu

```bash
ng new karting-league-app --standalone --routing --style=scss
cd karting-league-app
ng add @angular/material
npm install jwt-decode
```

---

## Struktura katalogów

```
src/
└── app/
    ├── core/
    │   ├── guards/
    │   │   ├── auth.guard.ts
    │   │   └── role.guard.ts
    │   ├── interceptors/
    │   │   └── jwt.interceptor.ts
    │   ├── models/
    │   │   ├── league.model.ts
    │   │   ├── race.model.ts
    │   │   ├── driver.model.ts
    │   │   ├── kart.model.ts
    │   │   ├── race-result.model.ts
    │   │   └── auth.model.ts
    │   └── services/
    │       ├── auth.service.ts
    │       ├── league.service.ts
    │       ├── race.service.ts
    │       ├── driver.service.ts
    │       ├── kart.service.ts
    │       └── race-result.service.ts
    ├── features/
    │   ├── auth/
    │   │   ├── login/
    │   │   │   ├── login.component.ts
    │   │   │   └── login.component.html
    │   │   └── register/
    │   │       ├── register.component.ts
    │   │       └── register.component.html
    │   ├── leagues/
    │   │   ├── league-list/
    │   │   │   ├── league-list.component.ts
    │   │   │   └── league-list.component.html
    │   │   ├── league-detail/
    │   │   │   ├── league-detail.component.ts
    │   │   │   └── league-detail.component.html
    │   │   └── league-form/
    │   │       ├── league-form.component.ts
    │   │       └── league-form.component.html
    │   ├── races/
    │   │   ├── race-detail/
    │   │   │   ├── race-detail.component.ts
    │   │   │   └── race-detail.component.html
    │   │   └── race-form/
    │   │       ├── race-form.component.ts
    │   │       └── race-form.component.html
    │   ├── results/
    │   │   └── result-form/
    │   │       ├── result-form.component.ts
    │   │       └── result-form.component.html
    │   ├── drivers/
    │   │   ├── driver-list/
    │   │   │   ├── driver-list.component.ts
    │   │   │   └── driver-list.component.html
    │   │   └── driver-form/
    │   │       ├── driver-form.component.ts
    │   │       └── driver-form.component.html
    │   └── karts/
    │       ├── kart-list/
    │       │   ├── kart-list.component.ts
    │       │   └── kart-list.component.html
    │       └── kart-form/
    │           ├── kart-form.component.ts
    │           └── kart-form.component.html
    ├── shared/
    │   ├── components/
    │   │   ├── navbar/
    │   │   │   ├── navbar.component.ts
    │   │   │   └── navbar.component.html
    │   │   ├── confirm-dialog/
    │   │   │   └── confirm-dialog.component.ts
    │   │   └── time-display/
    │   │       └── time-display.component.ts
    │   └── pipes/
    │       └── race-time.pipe.ts
    ├── app.config.ts
    ├── app.routes.ts
    └── app.component.ts
```

---

## Modele (core/models)

```typescript
// auth.model.ts
export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  firstName?: string;
  lastName?: string;
}

export interface AuthResponse {
  token: string;
  expiresAt: string;
  user: UserInfo;
}

export interface UserInfo {
  id: string;
  email: string;
  firstName?: string;
  lastName?: string;
  roles: string[];
  driverId?: string;
}

// league.model.ts
export interface League {
  id: string;
  name: string;
  description?: string;
  startDate: string;
  endDate?: string;
  isActive: boolean;
  raceCount: number;
  createdAt: string;
  races?: Race[];
}

export interface CreateLeague {
  name: string;
  description?: string;
  startDate: string;
  endDate?: string;
}

export interface UpdateLeague {
  name?: string;
  description?: string;
  startDate?: string;
  endDate?: string;
  isActive?: boolean;
}

// race.model.ts
export interface Race {
  id: string;
  leagueId: string;
  name: string;
  location?: string;
  raceDate: string;
  numberOfLaps: number;
  notes?: string;
  resultCount: number;
  createdAt: string;
  results?: RaceResult[];
}

export interface CreateRace {
  name: string;
  location?: string;
  raceDate: string;
  numberOfLaps: number;
  notes?: string;
}

// driver.model.ts
export interface Driver {
  id: string;
  firstName: string;
  lastName: string;
  fullName: string;
  racingNumber?: string;
  dateOfBirth?: string;
  createdAt: string;
}

export interface CreateDriver {
  firstName: string;
  lastName: string;
  racingNumber?: string;
  dateOfBirth?: string;
}

// kart.model.ts
export interface Kart {
  id: string;
  number: string;
  model?: string;
  category?: string;
  isActive: boolean;
}

export interface CreateKart {
  number: string;
  model?: string;
  category?: string;
}

// race-result.model.ts
export type ResultStatus = 'Finished' | 'DNF' | 'DNS' | 'Disqualified';

export interface RaceResult {
  id: string;
  raceId: string;
  driverId: string;
  driverName: string;
  kartId: string;
  kartNumber: string;
  lapTimeMs: number;
  totalTimeMs: number;
  startingPosition: number;
  finishingPosition?: number;
  lapsCompleted: number;
  status: ResultStatus;
  notes?: string;
}

export interface CreateRaceResult {
  driverId: string;
  kartId: string;
  lapTimeMs: number;
  totalTimeMs: number;
  startingPosition: number;
  finishingPosition?: number;
  lapsCompleted: number;
  status: ResultStatus;
  notes?: string;
}
```

---

## AuthService

```typescript
// core/services/auth.service.ts
import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs/operators';
import { jwtDecode } from 'jwt-decode';
import { AuthResponse, LoginRequest, RegisterRequest, UserInfo } from '../models/auth.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly TOKEN_KEY = 'karting_token';
  private readonly apiUrl = `${environment.apiUrl}/auth`;

  // Signal przechowujący aktualnego użytkownika
  currentUser = signal<UserInfo | null>(this.loadUserFromToken());

  isLoggedIn  = computed(() => this.currentUser() !== null);
  isAdmin     = computed(() => this.currentUser()?.roles.includes('Admin') ?? false);

  constructor(private http: HttpClient, private router: Router) {}

  login(dto: LoginRequest) {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, dto).pipe(
      tap(res => this.handleAuthResponse(res))
    );
  }

  register(dto: RegisterRequest) {
    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, dto).pipe(
      tap(res => this.handleAuthResponse(res))
    );
  }

  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    this.currentUser.set(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  private handleAuthResponse(res: AuthResponse): void {
    localStorage.setItem(this.TOKEN_KEY, res.token);
    this.currentUser.set(res.user);
  }

  private loadUserFromToken(): UserInfo | null {
    const token = localStorage.getItem(this.TOKEN_KEY);
    if (!token) return null;

    try {
      const decoded = jwtDecode<{ exp: number }>(token);
      if (decoded.exp * 1000 < Date.now()) {
        localStorage.removeItem(this.TOKEN_KEY);
        return null;
      }
      // Pobierz dane użytkownika z localStorage lub decoduj z tokenu
      return null; // zostanie uzupełnione przez GET /auth/me przy starcie
    } catch {
      return null;
    }
  }
}
```

---

## JWT Interceptor

```typescript
// core/interceptors/jwt.interceptor.ts
import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.getToken();

  if (token) {
    req = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
  }

  return next(req);
};
```

---

## Guards

```typescript
// core/guards/auth.guard.ts
import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = () => {
  const auth   = inject(AuthService);
  const router = inject(Router);

  if (auth.isLoggedIn()) return true;

  router.navigate(['/login']);
  return false;
};

// core/guards/role.guard.ts
import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const adminGuard: CanActivateFn = () => {
  const auth   = inject(AuthService);
  const router = inject(Router);

  if (auth.isAdmin()) return true;

  router.navigate(['/']);
  return false;
};
```

---

## Routing

```typescript
// app.routes.ts
import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { adminGuard } from './core/guards/role.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'leagues', pathMatch: 'full' },

  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./features/auth/register/register.component').then(m => m.RegisterComponent)
  },

  // Ligi — publiczne
  {
    path: 'leagues',
    loadComponent: () =>
      import('./features/leagues/league-list/league-list.component').then(m => m.LeagueListComponent)
  },
  {
    path: 'leagues/:id',
    loadComponent: () =>
      import('./features/leagues/league-detail/league-detail.component').then(m => m.LeagueDetailComponent)
  },

  // Wyścigi — publiczne
  {
    path: 'leagues/:leagueId/races/:raceId',
    loadComponent: () =>
      import('./features/races/race-detail/race-detail.component').then(m => m.RaceDetailComponent)
  },

  // Panel administracyjny — wymaga roli Admin
  {
    path: 'admin',
    canActivate: [authGuard, adminGuard],
    children: [
      {
        path: 'leagues/new',
        loadComponent: () =>
          import('./features/leagues/league-form/league-form.component').then(m => m.LeagueFormComponent)
      },
      {
        path: 'leagues/:id/edit',
        loadComponent: () =>
          import('./features/leagues/league-form/league-form.component').then(m => m.LeagueFormComponent)
      },
      {
        path: 'leagues/:leagueId/races/new',
        loadComponent: () =>
          import('./features/races/race-form/race-form.component').then(m => m.RaceFormComponent)
      },
      {
        path: 'leagues/:leagueId/races/:raceId/results/new',
        loadComponent: () =>
          import('./features/results/result-form/result-form.component').then(m => m.ResultFormComponent)
      },
      {
        path: 'drivers',
        loadComponent: () =>
          import('./features/drivers/driver-list/driver-list.component').then(m => m.DriverListComponent)
      },
      {
        path: 'drivers/new',
        loadComponent: () =>
          import('./features/drivers/driver-form/driver-form.component').then(m => m.DriverFormComponent)
      },
      {
        path: 'karts',
        loadComponent: () =>
          import('./features/karts/kart-list/kart-list.component').then(m => m.KartListComponent)
      },
      {
        path: 'karts/new',
        loadComponent: () =>
          import('./features/karts/kart-form/kart-form.component').then(m => m.KartFormComponent)
      }
    ]
  },

  { path: '**', redirectTo: 'leagues' }
];
```

---

## app.config.ts

```typescript
import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { routes } from './app.routes';
import { jwtInterceptor } from './core/interceptors/jwt.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(withInterceptors([jwtInterceptor])),
    provideAnimations()
  ]
};
```

---

## Przykładowy serwis API (LeagueService)

```typescript
// core/services/league.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { League, CreateLeague, UpdateLeague } from '../models/league.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class LeagueService {
  private readonly apiUrl = `${environment.apiUrl}/leagues`;

  constructor(private http: HttpClient) {}

  getAll(isActive?: boolean): Observable<League[]> {
    let params = new HttpParams();
    if (isActive !== undefined) params = params.set('isActive', isActive);
    return this.http.get<League[]>(this.apiUrl, { params });
  }

  getById(id: string): Observable<League> {
    return this.http.get<League>(`${this.apiUrl}/${id}`);
  }

  create(dto: CreateLeague): Observable<League> {
    return this.http.post<League>(this.apiUrl, dto);
  }

  update(id: string, dto: UpdateLeague): Observable<League> {
    return this.http.put<League>(`${this.apiUrl}/${id}`, dto);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
```

---

## Przykładowy komponent (LeagueListComponent)

```typescript
// features/leagues/league-list/league-list.component.ts
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { LeagueService } from '../../../core/services/league.service';
import { AuthService } from '../../../core/services/auth.service';
import { League } from '../../../core/models/league.model';

@Component({
  selector: 'app-league-list',
  standalone: true,
  imports: [CommonModule, RouterModule, MatTableModule, MatButtonModule, MatIconModule],
  templateUrl: './league-list.component.html'
})
export class LeagueListComponent implements OnInit {
  private leagueService = inject(LeagueService);
  auth = inject(AuthService);

  leagues: League[] = [];
  displayedColumns = ['name', 'startDate', 'endDate', 'raceCount', 'status', 'actions'];
  isLoading = false;
  error: string | null = null;

  ngOnInit(): void {
    this.loadLeagues();
  }

  loadLeagues(): void {
    this.isLoading = true;
    this.leagueService.getAll().subscribe({
      next: data => {
        this.leagues = data;
        this.isLoading = false;
      },
      error: err => {
        this.error = 'Nie udało się załadować lig';
        this.isLoading = false;
      }
    });
  }

  deleteLeague(id: string): void {
    if (!confirm('Czy na pewno chcesz usunąć tę ligę?')) return;

    this.leagueService.delete(id).subscribe({
      next: () => this.loadLeagues(),
      error: () => this.error = 'Nie udało się usunąć ligi'
    });
  }
}
```

---

## Pipe do formatowania czasu

```typescript
// shared/pipes/race-time.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'raceTime', standalone: true })
export class RaceTimePipe implements PipeTransform {
  transform(ms: number | null | undefined): string {
    if (ms == null) return '—';
    const minutes = Math.floor(ms / 60000);
    const seconds = Math.floor((ms % 60000) / 1000);
    const millis  = ms % 1000;
    return `${String(minutes).padStart(2, '0')}:${String(seconds).padStart(2, '0')}.${String(millis).padStart(3, '0')}`;
  }
}
```

Użycie w szablonie: `{{ result.lapTimeMs | raceTime }}`

---

## environment.ts

```typescript
// environments/environment.ts
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api'
};

// environments/environment.production.ts
export const environment = {
  production: true,
  apiUrl: '/api'
};
```

---

## Obsługa błędów HTTP — globalny interceptor (opcjonalny)

```typescript
// core/interceptors/error.interceptor.ts
import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const auth   = inject(AuthService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        auth.logout();
      }
      if (error.status === 403) {
        router.navigate(['/']);
      }
      return throwError(() => error);
    })
  );
};
```

Dodaj do `app.config.ts`: `withInterceptors([jwtInterceptor, errorInterceptor])`
