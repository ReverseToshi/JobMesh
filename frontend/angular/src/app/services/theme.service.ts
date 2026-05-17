import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { BehaviorSubject } from 'rxjs';

export type ThemeMode = 'light' | 'dark';

@Injectable({
  providedIn: 'root',
})
export class ThemeService {
  private readonly THEME_KEY = 'jobmesh_theme';
  private readonly isBrowser: boolean;
  private readonly themeSubject = new BehaviorSubject<ThemeMode>('light');

  readonly theme$ = this.themeSubject.asObservable();

  constructor(@Inject(PLATFORM_ID) platformId: object) {
    this.isBrowser = isPlatformBrowser(platformId);
    this.themeSubject.next(this.getStoredTheme());
  }

  getTheme(): ThemeMode {
    return this.themeSubject.value;
  }

  setTheme(theme: ThemeMode): void {
    if (this.isBrowser) {
      localStorage.setItem(this.THEME_KEY, theme);
    }

    this.themeSubject.next(theme);
  }

  toggleTheme(): void {
    this.setTheme(this.getTheme() === 'light' ? 'dark' : 'light');
  }

  private getStoredTheme(): ThemeMode {
    if (!this.isBrowser) {
      return 'light';
    }

    const storedTheme = localStorage.getItem(this.THEME_KEY);
    return storedTheme === 'dark' ? 'dark' : 'light';
  }
}