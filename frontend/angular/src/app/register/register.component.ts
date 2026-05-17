import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { firstValueFrom } from 'rxjs';

interface RegisterResponse {
  Message?: string;
  message?: string;
}

interface RegisterErrorResponse {
  Message?: string;
  message?: string;
}

@Component({
  selector: 'register-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class RegisterComponent {
  username = '';
  password = '';
  confirmPassword = '';
  loading = false;
  error = '';
  success = '';

  constructor(private http: HttpClient) {}

  async register() {
    this.error = '';
    this.success = '';

    if (!this.username.trim() || !this.password.trim()) {
      this.error = 'Username and password are required.';
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.error = 'Passwords do not match.';
      return;
    }

    this.loading = true;

    try {
      const response = await firstValueFrom(
        this.http.post<RegisterResponse>('/api/register', {
          username: this.username,
          password: this.password,
        }),
      );

      this.success = response.Message ?? response.message ?? 'Registration successful!';
      window.alert(this.success);
      this.username = '';
      this.password = '';
      this.confirmPassword = '';
    } catch (error) {
      const backendMessage =
        error instanceof HttpErrorResponse
          ? ((error.error as RegisterErrorResponse | null)?.Message ??
            (error.error as RegisterErrorResponse | null)?.message)
          : undefined;

      if (error instanceof HttpErrorResponse && error.status === 409) {
        this.error = backendMessage ?? 'Username already exists.';
      } else if (error instanceof HttpErrorResponse && error.status === 400) {
        this.error = backendMessage ?? 'Failed to register user.';
      } else {
        this.error = backendMessage ?? 'Unable to register right now. Please try again.';
      }

      window.alert(this.error);
    } finally {
      this.loading = false;
    }
  }
}
