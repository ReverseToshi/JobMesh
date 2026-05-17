import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { AuthService } from '../services/auth.service';

interface LoginResponse {
  Message?: string;
  message?: string;
  Token?: string;
  token?: string;
}

@Component({
  selector: 'login-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent {
  username = '';
  password = '';
  loading = false;
  error = '';
  success = '';

  constructor(
    private http: HttpClient,
    private authService: AuthService,
    private router: Router,
  ) {}

  async login() {
    this.error = '';
    this.success = '';
    this.loading = true;
    try {
      const response = await firstValueFrom(
        this.http.post<LoginResponse>('/api/login', {
          username: this.username,
          password: this.password,
        }),
      );

      const token = response.Token ?? response.token;
      if (token) {
        this.authService.setToken(token);
        this.success = response.Message ?? response.message ?? 'Login successful!';
        this.username = '';
        this.password = '';
        setTimeout(() => {
          this.router.navigate(['/dashboard']);
        }, 500);
      } else {
        this.error = 'Login response missing token. Please try again.';
      }
    } catch (error) {
      if (error instanceof HttpErrorResponse && error.status === 401) {
        this.error = 'Invalid username or password.';
      } else {
        this.error = 'Unable to reach the login service. Please try again.';
      }
    } finally {
      this.loading = false;
    }
  }
}
