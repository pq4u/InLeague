import { Component, inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from '../../../core/services/auth.service';
import { ThemeService } from '../../../core/services/theme.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterModule, MatToolbarModule, MatButtonModule, MatIconModule],
  template: `
    <mat-toolbar color="primary">
      <span routerLink="/">InLeague Karting</span>
      <span class="spacer"></span>
      <button mat-button routerLink="/leagues">Ligi</button>
      @if (auth.isLoggedIn()) {
        @if (auth.isAdmin()) {
          <button mat-button routerLink="/admin/drivers">Kierowcy</button>
          <button mat-button routerLink="/admin/karts">Gokarty</button>
        }
        <button mat-button (click)="auth.logout()">Wyloguj ({{ auth.currentUser()?.email }})</button>
      } @else {
        <button mat-button routerLink="/login">Zaloguj się</button>
        <button mat-button routerLink="/register">Zarejestruj się</button>
      }

      <button mat-icon-button (click)="theme.toggleTheme()" aria-label="Przełącz motyw" class="theme-toggle">
        <mat-icon>{{ theme.theme() === 'light' ? 'dark_mode' : 'light_mode' }}</mat-icon>
      </button>
    </mat-toolbar>
  `,
  styles: [`
    .spacer { flex: 1 1 auto; }
    span[routerLink] { cursor: pointer; }
    .theme-toggle {
      margin-left: 8px;
      transition: transform 0.4s ease, background-color 0.3s ease;
    }
    .theme-toggle:hover {
      transform: rotate(30deg);
      background-color: rgba(255, 255, 255, 0.1);
    }
  `]
})
export class NavbarComponent {
  auth = inject(AuthService);
  theme = inject(ThemeService);
}
