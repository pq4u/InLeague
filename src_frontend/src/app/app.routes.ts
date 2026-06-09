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
        path: 'drivers/:id/edit',
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
      },
      {
        path: 'karts/:id/edit',
        loadComponent: () =>
          import('./features/karts/kart-form/kart-form.component').then(m => m.KartFormComponent)
      },
      {
        path: 'leagues/:leagueId/races/:raceId/edit',
        loadComponent: () =>
          import('./features/races/race-form/race-form.component').then(m => m.RaceFormComponent)
      }
    ]
  },

  { path: '**', redirectTo: 'leagues' }
];
