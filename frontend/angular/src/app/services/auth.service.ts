import { Inject, Injectable, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { BehaviorSubject } from 'rxjs';

interface StoredAuth {
  token: string;
  expiresAt: number;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly TOKEN_KEY = 'jobmesh_token';
  private readonly DEFAULT_TTL_MS = 60 * 60 * 1000;
  private tokenSubject = new BehaviorSubject<string | null>(null);
  private readonly isBrowser: boolean;
  private expiryTimer: ReturnType<typeof setTimeout> | null = null;

  public token$ = this.tokenSubject.asObservable();

  constructor(@Inject(PLATFORM_ID) platformId: object) {
    this.isBrowser = isPlatformBrowser(platformId);
    const storedAuth = this.getStoredAuth();
    if (storedAuth) {
      this.scheduleExpiry(storedAuth.expiresAt);
      this.tokenSubject.next(storedAuth.token);
    }
  }

  setToken(token: string, expiresInMs: number = this.DEFAULT_TTL_MS): void {
    const expiresAt = Date.now() + expiresInMs;

    if (this.isBrowser) {
      localStorage.setItem(this.TOKEN_KEY, JSON.stringify({ token, expiresAt } satisfies StoredAuth));
    }

    this.scheduleExpiry(expiresAt);
    this.tokenSubject.next(token);
  }

  getToken(): string | null {
    return this.tokenSubject.value;
  }

  private getStoredAuth(): StoredAuth | null {
    if (!this.isBrowser) {
      return null;
    }

    const rawValue = localStorage.getItem(this.TOKEN_KEY);
    if (!rawValue) {
      return null;
    }

    try {
      const parsed = JSON.parse(rawValue) as Partial<StoredAuth>;
      if (!parsed.token || typeof parsed.expiresAt !== 'number') {
        localStorage.removeItem(this.TOKEN_KEY);
        return null;
      }

      if (Date.now() >= parsed.expiresAt) {
        localStorage.removeItem(this.TOKEN_KEY);
        return null;
      }

      return { token: parsed.token, expiresAt: parsed.expiresAt };
    } catch {
      localStorage.removeItem(this.TOKEN_KEY);
      return null;
    }
  }

  clearToken(): void {
    this.clearExpiryTimer();

    if (this.isBrowser) {
      localStorage.removeItem(this.TOKEN_KEY);
    }

    this.tokenSubject.next(null);
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }

  private scheduleExpiry(expiresAt: number): void {
    this.clearExpiryTimer();

    if (!this.isBrowser) {
      return;
    }

    const delay = Math.max(0, expiresAt - Date.now());
    this.expiryTimer = setTimeout(() => {
      this.clearToken();
    }, delay);
  }

  private clearExpiryTimer(): void {
    if (this.expiryTimer) {
      clearTimeout(this.expiryTimer);
      this.expiryTimer = null;
    }
  }
}
