import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { AuthService } from '../../core/auth.service';
import { SettingsStore } from '../../core/settings.store';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, MatCardModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatProgressBarModule],
  template: `
    <div class="login-wrap">
      <mat-card class="login-card">
        @if (loading()) { <mat-progress-bar mode="indeterminate"></mat-progress-bar> }
        <mat-card-header>
          <mat-card-title>🐂 Leather ERP</mat-card-title>
          <mat-card-subtitle>Deri Üretim Yönetimi</mat-card-subtitle>
        </mat-card-header>
        <mat-card-content>
          <form (ngSubmit)="submit()">
            <mat-form-field class="full-width">
              <mat-label>Kullanıcı adı</mat-label>
              <input matInput name="username" [(ngModel)]="username" required autocomplete="username" />
            </mat-form-field>
            <mat-form-field class="full-width">
              <mat-label>Parola</mat-label>
              <input matInput type="password" name="password" [(ngModel)]="password" required autocomplete="current-password" />
            </mat-form-field>
            @if (error()) { <p class="low">{{ error() }}</p> }
            <button mat-raised-button color="primary" class="full-width" type="submit" [disabled]="loading()">Giriş Yap</button>
          </form>
          <p class="muted demo">Demo: <b>admin</b> / <b>admin123</b></p>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .login-wrap { min-height: 100vh; display: flex; align-items: center; justify-content: center; background: linear-gradient(135deg, #3f51b5, #5c6bc0); padding: 16px; }
    .login-card { width: 360px; max-width: 100%; overflow: hidden; }
    .demo { text-align: center; margin-top: 12px; font-size: 13px; }
  `]
})
export class LoginComponent {
  private auth = inject(AuthService);
  private settings = inject(SettingsStore);
  private router = inject(Router);

  username = 'admin';
  password = 'admin123';
  loading = signal(false);
  error = signal<string | null>(null);

  submit(): void {
    this.error.set(null);
    this.loading.set(true);
    this.auth.login(this.username, this.password).subscribe({
      next: () => {
        this.settings.load();
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.error.set(err?.error?.error ?? 'Giriş başarısız.');
        this.loading.set(false);
      }
    });
  }
}
