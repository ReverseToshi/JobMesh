import { CommonModule, isPlatformBrowser } from '@angular/common';
import { ChangeDetectorRef, Component, Inject, NgZone, OnInit, PLATFORM_ID } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../services/auth.service';
import { ThemeService, ThemeMode } from '../services/theme.service';

interface JobItem {
  name: string;
  type: string;
  status?: string;
  priority?: string;
  createdAt?: string;
  completedAt?: string;
}

interface JobForm {
  jobType: string;
  payload: string;
  priority: string;
  retryCount: number;
}

@Component({
  selector: 'user-dashboard-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-dashboard.component.html',
  styleUrls: ['./user-dashboard.component.scss'],
})
export class UserDashboardComponent implements OnInit {
  currentUser = 'User';
  history: JobItem[] = [];
  loading = false;
  theme: ThemeMode = 'light';
  showJobModal = false;
  jobSubmitting = false;
  jobForm: JobForm = {
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
    private ngZone: NgZone,
    @Inject(PLATFORM_ID) private platformId: object,
  ) {}

  ngOnInit(): void {
    // Check if running in browser
    if (!isPlatformBrowser(this.platformId)) {
      return;
    }

    // Check for valid token
    const token = this.auth.getToken();
    if (!token) {
      this.router.navigate(['/login']);
      return;
    }

    // Initialize component
    this.currentUser = this.decodeUserFromToken(token) || 'User';
    this.theme = this.themeService.getTheme();
    this.loadHistory();
  }

  /**
   * Load user's job history from API
   */
  private loadHistory(): void {
    const token = this.auth.getToken();
    if (!token) {
      this.router.navigate(['/login']);
      return;
    }

    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json',
    });

    this.loading = true;

    this.http.get<any[]>('/api/my/jobs', { headers }).subscribe({
      next: (data) => {
        this.ngZone.run(() => {
          this.history = (data || []).map((job) => ({
            name: job.type || 'Job',
            type: job.jobType || 'Standard',
            status: job.status || 'Pending',
            priority: job.priority || 'Normal',
            createdAt: job.createdAt,
            completedAt: job.completedAt,
          }));
          this.loading = false;
          this.cdr.markForCheck();
        });
      },
      error: (error) => {
        this.ngZone.run(() => {
          console.error('Failed to load jobs:', error);
          this.history = [];
          this.loading = false;
          this.cdr.markForCheck();
        });
      },
    });
  }

  /**
   * Refresh the job history
   */
  refreshHistory(): void {
    this.loadHistory();
  }

  /**
   * Toggle between light and dark theme
   */
  toggleTheme(): void {
    this.themeService.toggleTheme();
    this.theme = this.themeService.getTheme();
  }

  /**
   * Logout user and redirect to login page
   */
  logout(): void {
    this.auth.clearToken();
    this.router.navigate(['/login']);
  }

  /**
   * Open job submission modal
   */
  openJobModal(): void {
    this.showJobModal = true;
  }

  /**
   * Close job submission modal
   */
  closeJobModal(): void {
    this.showJobModal = false;
    this.resetJobForm();
  }

  /**
   * Reset job form to initial state
   */
  resetJobForm(): void {
    this.jobForm = {
      jobType: '',
      payload: '',
      priority: 'Normal',
      retryCount: 0,
    };
  }

  /**
   * Submit new job to API
   */
  submitJob(): void {
    // Validate form
    if (!this.jobForm.jobType.trim() || !this.jobForm.payload.trim()) {
      alert('Please fill in Job Type and Payload fields.');
      return;
    }

    // Get token
    const token = this.auth.getToken();
    if (!token) {
      this.router.navigate(['/login']);
      return;
    }

    // Prepare headers
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json',
    });

    // Prepare payload
    const jobPayload = {
      jobType: this.jobForm.jobType.trim(),
      payload: this.jobForm.payload.trim(),
      priority: this.jobForm.priority,
      retryCount: Number(this.jobForm.retryCount) || 0,
    };

    this.jobSubmitting = true;

    this.http.post('/api/jobs', jobPayload, { headers }).subscribe({
      next: () => {
        this.ngZone.run(() => {
          alert('Job submitted successfully!');
          this.closeJobModal();
          this.loadHistory(); // Refresh the list
          this.jobSubmitting = false;
          this.cdr.markForCheck();
        });
      },
      error: (error) => {
        this.ngZone.run(() => {
          const errorMessage = error.error?.message || error.message || 'Unknown error';
          alert(`Failed to submit job: ${errorMessage}`);
          this.jobSubmitting = false;
          this.cdr.markForCheck();
        });
      },
    });
  }

  /**
   * Decode username from JWT token
   * Token format: base64(username|timestamp)
   */
  private decodeUserFromToken(token: string): string | null {
    try {
      const decoded = atob(token);
      const [username] = decoded.split('|');
      return username?.trim() || null;
    } catch (error) {
      console.error('Failed to decode token:', error);
      return null;
    }
  }
}