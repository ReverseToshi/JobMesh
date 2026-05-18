import { CommonModule, isPlatformBrowser } from '@angular/common';
import { Component, Inject, OnInit, PLATFORM_ID } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthService } from '../services/auth.service';
import { ThemeService, ThemeMode } from '../services/theme.service';

@Component({
  selector: 'user-dashboard-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './user-dashboard.component.html',
  styleUrls: ['./user-dashboard.component.scss'],
})
export class UserDashboardComponent implements OnInit {
  currentUser = 'User';
  history: Array<{ time: string; title: string; detail: string; status?: string }> = [];
  loading = true;
  theme: ThemeMode = 'light';

  constructor(
    private auth: AuthService,
    private http: HttpClient,
    private router: Router,
    private themeService: ThemeService,
    @Inject(PLATFORM_ID) private platformId: object,
  ) {}

  ngOnInit(): void {
    if (!isPlatformBrowser(this.platformId)) {
      return;
    }

    const token = this.auth.getToken();
    if (!token) {
      this.router.navigate(['/login']);
      return;
    }

    this.currentUser = this.decodeUserFromToken(token) || 'User';
    this.theme = this.themeService.getTheme();

    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });

    this.http.get<any[]>('/api/user/history', { headers }).subscribe({
      next: (data) => {
        this.history = (data || []).map((d) => ({
          time: d.time ?? new Date().toISOString(),
          title: d.title ?? d.action ?? 'Activity',
          detail: d.detail ?? d.description ?? JSON.stringify(d),
          status: d.status,
        }));
        this.loading = false;
      },
      error: () => {
        // fallback sample data
        this.history = [
          { time: new Date().toISOString(), title: 'Submitted job', detail: 'Job #12345 submitted', status: 'completed' },
          { time: new Date().toISOString(), title: 'Job failed', detail: 'Job #12344 failed with error', status: 'failed' },
          { time: new Date().toISOString(), title: 'Job retried', detail: 'Job #12343 retried', status: 'retry' },
        ];
        this.loading = false;
      },
    });
  }

  toggleTheme(): void {
    this.themeService.toggleTheme();
    this.theme = this.themeService.getTheme();
  }

  logout(): void {
    this.auth.clearToken();
    this.router.navigate(['/login']);
  }

  private decodeUserFromToken(token: string): string | null {
    try {
      // token is a simple base64 payload (username|ticks) in this project
      const decoded = atob(token);
      const [username] = decoded.split('|');
      return username || null;
    } catch {
      return null;
    }
  }
}
