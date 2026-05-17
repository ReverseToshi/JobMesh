import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { ThemeMode, ThemeService } from '../services/theme.service';

interface SectionPageData {
  title: string;
  subtitle: string;
  summary: string;
  highlights: string[];
  backendApis: string[];
  mvpPriorities: string[];
  employerWins: string[];
}

@Component({
  selector: 'app-dashboard-section-page',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './section-page.component.html',
  styleUrls: ['./section-page.component.scss'],
})
export class DashboardSectionPageComponent implements OnInit {
  page: SectionPageData = {
    title: '',
    subtitle: '',
    summary: '',
    highlights: [],
    backendApis: [],
    mvpPriorities: [],
    employerWins: [],
  };

  currentUser = 'Operator';
  theme: ThemeMode = 'light';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService,
    private themeService: ThemeService,
  ) {}

  ngOnInit(): void {
    const token = this.authService.getToken();
    if (!token) {
      this.router.navigate(['/login']);
      return;
    }

    this.currentUser = this.resolveCurrentUser(token);
    this.page = this.route.snapshot.data['page'] as SectionPageData;
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
