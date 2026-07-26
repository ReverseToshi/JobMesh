import { CommonModule, isPlatformBrowser } from '@angular/common';
import { ChangeDetectionStrategy, ChangeDetectorRef, Component, Inject, OnInit, PLATFORM_ID } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../services/auth.service';
import { ThemeService, ThemeMode } from '../services/theme.service';

@Component({
  selector: 'user-dashboard-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-dashboard.component.html',
  styleUrls: ['./user-dashboard.component.scss'],
})
export class UserDashboardComponent implements OnInit {
  currentUser = 'User';
  history: Array<{ name: string; type: string; status?: string; priority?: string }> = [];
  loading = false;
  theme: ThemeMode = 'light';
  showJobModal = false;
  jobSubmitting = false;
  jobForm = {
    jobType: '',
    payload: '',
    priority: 'Normal',
    retryCount: 0,
  };

  constructor(
    private auth: AuthService,
    private http: HttpClient,
    private router: Router,
    private themeService: ThemeService,
    private cdr: ChangeDetectorRef,
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
    this.loadHistory();
  }

  refreshHistory(): void {
    this.loadHistory();
  }

  private loadHistory(): void {
    const token = this.auth.getToken();
    if (!token) {
      this.router.navigate(['/login']);
      return;
    }

    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });

    this.loading = true;
    this.http.get<any[]>('/api/my/jobs', { headers }).subscribe({
      next: (data) => {
        const newData = (data || []).map((d) => ({
          name: d.type ?? 'Job',
          type: d.jobType ?? 'Standard',
          status: d.status,
          priority: d.priority,
        }));
        this.history.splice(0, this.history.length, ...newData);
        this.loading = false;
      },
      error: () => {
        this.history.splice(0, this.history.length);
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

  openJobModal(): void {
    this.showJobModal = true;
  }

  closeJobModal(): void {
    this.showJobModal = false;
    this.resetJobForm();
  }

  resetJobForm(): void {
    this.jobForm = {
      jobType: '',
      payload: '',
      priority: 'Normal',
      retryCount: 0,
    };
  }

  async submitJob(): Promise<void> {
    if (!this.jobForm.jobType || !this.jobForm.payload) {
      alert('Please fill in Job Type and Payload fields.');
      return;
    }

    this.jobSubmitting = true;
    try {
      const token = this.auth.getToken();
      if (!token) {
        this.router.navigate(['/login']);
        return;
      }

      const headers = new HttpHeaders({
        Authorization: `Bearer ${token}`,
      });

      const jobPayload = {
        jobType: this.jobForm.jobType,
        payload: this.jobForm.payload,
        priority: this.jobForm.priority,
        retryCount: this.jobForm.retryCount,
      };

      await new Promise((resolve, reject) => {
        this.http.post('/api/jobs', jobPayload, { headers }).subscribe({
          next: () => {
            alert('Job submitted successfully!');
            this.closeJobModal();
            resolve(true);
            // Optionally refresh history here
          },
          error: (err) => {
            alert('Failed to submit job: ' + (err.error?.message || 'Unknown error'));
            reject(err);
          },
        });
      });
    } catch (error) {
      console.error('Job submission error:', error);
    } finally {
      this.jobSubmitting = false;
    }
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

