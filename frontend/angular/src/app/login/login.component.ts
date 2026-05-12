import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';

interface LoginResponse {
  Message?: string;
  message?: string;
}

@Component({
  selector: 'login-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent {
  username = '';
  password = '';
  loading = false;
  error = '';
  success = '';

  constructor(private router: Router, private http: HttpClient) {}

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

      this.success = response.Message ?? response.message ?? 'Login successful!';
      this.username = '';
      this.password = '';
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
