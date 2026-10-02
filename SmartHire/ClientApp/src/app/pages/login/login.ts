import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  signUpActive = false;
  loading = false;
  errorMessage = '';
  successMessage = '';

  signIn = { username: '', password: '' };
  signUp = { username: '', email: '', password: '' };

  private get returnUrl(): string {
    return this.route.snapshot.queryParamMap.get('returnUrl') || '/dashboard';
  }

  activateSignUp(): void {
    this.signUpActive = true;
    this.errorMessage = '';
    this.successMessage = '';
  }

  activateSignIn(): void {
    this.signUpActive = false;
    this.errorMessage = '';
    this.successMessage = '';
  }

  onSignIn(): void {
    if (!this.signIn.username || !this.signIn.password) {
      this.errorMessage = 'Please enter both username and password.';
      return;
    }

    this.loading = true;
    this.errorMessage = '';
    this.auth.localLogin(this.signIn).subscribe({
      next: () => {
        this.loading = false;
        this.router.navigateByUrl(this.returnUrl);
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err?.error?.message || 'Invalid username or password.';
      },
    });
  }

  onSignUp(): void {
    if (!this.signUp.username || !this.signUp.email || !this.signUp.password) {
      this.errorMessage = 'User, Email and Password are required.';
      return;
    }

    this.loading = true;
    this.errorMessage = '';
    this.auth.register(this.signUp).subscribe({
      next: () => {
        this.loading = false;
        this.successMessage = 'Registration successful. Please sign in.';
        this.signIn.username = this.signUp.username;
        this.signUp = { username: '', email: '', password: '' };
        this.activateSignIn();
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err?.error?.message || 'Registration failed.';
      },
    });
  }

  ssoLogin(): void {
    this.auth.ssoLogin(this.returnUrl);
  }
}
