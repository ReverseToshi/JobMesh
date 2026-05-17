import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { ThemeMode, ThemeService } from '../services/theme.service';

interface DashboardSectionLink {
  path: string;
  title: string;
  summary: string;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent implements OnInit {
  currentUser = 'Operator';
  theme: ThemeMode = 'light';

  sections: DashboardSectionLink[] = [
    { path: '/dashboard/queues', title: 'Queue monitoring', summary: 'Pending, processing, DLQ counts, latency, and queue controls.' },
    { path: '/dashboard/workers', title: 'Worker monitoring', summary: 'Online status, CPU, memory, heartbeats, and worker actions.' },
    { path: '/dashboard/submit', title: 'Task submission', summary: 'Create jobs, set priority, retry count, timeout, and schedule.' },
    { path: '/dashboard/history', title: 'Task history', summary: 'Searchable task records with status, duration, retries, and logs.' },
    { path: '/dashboard/activity', title: 'Real-time activity', summary: 'Worker events, task lifecycle updates, queue warnings, and retries.' },
    { path: '/dashboard/logs', title: 'Logs & errors', summary: 'Service logs, stack traces, filters, and export options.' },
    { path: '/dashboard/analytics', title: 'Analytics & charts', summary: 'Throughput, queue growth, worker utilization, and failure trends.' },
    { path: '/dashboard/dlq', title: 'Dead-letter queue', summary: 'Permanent failures, failure reasons, retry history, and replay actions.' },
    { path: '/dashboard/scheduling', title: 'Scheduling', summary: 'Cron jobs, recurring tasks, delayed runs, and calendar scheduling.' },
    { path: '/dashboard/users', title: 'Users & roles', summary: 'Admin, operator, and viewer permissions plus audit logs.' },
    { path: '/dashboard/alerts', title: 'Alerting', summary: 'Worker offline alerts, queue thresholds, and failure-rate warnings.' },
  ];

  constructor(
    private authService: AuthService,
    private router: Router,
    private themeService: ThemeService,
  ) {}

  ngOnInit(): void {
    const token = this.authService.getToken();
    if (!token) {
      this.router.navigate(['/login']);
      return;
    }

    this.currentUser = this.resolveCurrentUser(token);
    this.theme = this.themeService.getTheme();
  }

  logout(): void {
    this.authService.clearToken();
    this.router.navigate(['/login']);
  }

  toggleTheme(): void {
    this.themeService.toggleTheme();
    this.theme = this.themeService.getTheme();
  }

  private resolveCurrentUser(token: string): string {
    try {
      const decoded = atob(token);
      const [username] = decoded.split('|');
      return username || 'Operator';
    } catch {
      return 'Operator';
    }
  }
}
