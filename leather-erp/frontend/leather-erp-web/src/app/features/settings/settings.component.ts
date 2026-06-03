import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ApiService } from '../../core/api.service';
import { SettingsStore } from '../../core/settings.store';
import { AppSettings } from '../../core/models';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [
    CommonModule, FormsModule, MatCardModule, MatFormFieldModule, MatInputModule,
    MatSelectModule, MatButtonModule, MatSnackBarModule
  ],
  template: `
    <h1 class="page-title">Ayarlar</h1>
    <mat-card>
      <mat-card-content>
        <div class="row">
          <mat-form-field>
            <mat-label>Deri ölçü birimi</mat-label>
            <mat-select [(ngModel)]="form.displayUnit">
              <mat-option value="Dm2">Desimetrekare (dm²)</mat-option>
              <mat-option value="SquareFoot">Ayak² (sqft)</mat-option>
            </mat-select>
          </mat-form-field>
          <mat-form-field>
            <mat-label>Para birimi</mat-label>
            <mat-select [(ngModel)]="form.currencyCode">
              <mat-option value="TRY">₺ Türk Lirası</mat-option>
              <mat-option value="USD">$ Dolar</mat-option>
              <mat-option value="EUR">€ Euro</mat-option>
            </mat-select>
          </mat-form-field>
        </div>
        <div class="row">
          <mat-form-field><mat-label>KDV oranı (0–1)</mat-label><input matInput type="number" step="0.01" [(ngModel)]="form.vatRate" /></mat-form-field>
          <mat-form-field><mat-label>Varsayılan fire (0–1)</mat-label><input matInput type="number" step="0.01" [(ngModel)]="form.defaultWasteRate" /></mat-form-field>
          <mat-form-field><mat-label>Varsayılan kâr marjı (0–1)</mat-label><input matInput type="number" step="0.01" [(ngModel)]="form.defaultProfitMargin" /></mat-form-field>
        </div>
        <button mat-raised-button color="primary" (click)="save()">Kaydet</button>
      </mat-card-content>
    </mat-card>
  `
})
export class SettingsComponent implements OnInit {
  private api = inject(ApiService);
  private store = inject(SettingsStore);
  private snack = inject(MatSnackBar);

  form: AppSettings = { ...this.store.settings() };

  ngOnInit(): void {
    this.api.getSettings().subscribe(s => this.form = { ...s });
  }

  save(): void {
    this.api.updateSettings(this.form).subscribe(s => {
      this.store.set(s);
      this.snack.open('Ayarlar kaydedildi', 'Tamam', { duration: 2500 });
    });
  }
}
