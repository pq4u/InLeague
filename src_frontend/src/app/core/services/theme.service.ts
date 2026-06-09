import { Injectable, signal, effect } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private readonly THEME_KEY = 'inleague_theme';
  
  // Sygnał przechowujący aktualny motyw: 'light' lub 'dark'
  theme = signal<'light' | 'dark'>('light');

  constructor() {
    this.initializeTheme();
    
    // Automatycznie dodawaj/usuwaj klasę dark-theme z elementu html przy zmianie motywu
    effect(() => {
      const currentTheme = this.theme();
      if (typeof document !== 'undefined') {
        const root = document.documentElement;
        if (currentTheme === 'dark') {
          root.classList.add('dark-theme');
        } else {
          root.classList.remove('dark-theme');
        }
        localStorage.setItem(this.THEME_KEY, currentTheme);
      }
    });
  }

  toggleTheme(): void {
    this.theme.update(current => current === 'light' ? 'dark' : 'light');
  }

  private initializeTheme(): void {
    if (typeof window !== 'undefined' && typeof localStorage !== 'undefined') {
      const savedTheme = localStorage.getItem(this.THEME_KEY) as 'light' | 'dark' | null;
      if (savedTheme === 'light' || savedTheme === 'dark') {
        this.theme.set(savedTheme);
      } else {
        // Jeśli nie zapisano wyboru, sprawdź preferencje systemowe
        const prefersDark = (typeof window !== 'undefined' && typeof window.matchMedia === 'function')
          ? window.matchMedia('(prefers-color-scheme: dark)').matches
          : false;
        this.theme.set(prefersDark ? 'dark' : 'light');
      }
    }
  }
}
