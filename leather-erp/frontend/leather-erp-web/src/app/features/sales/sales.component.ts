import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ApiService } from '../../core/api.service';
import { SettingsStore } from '../../core/settings.store';
import { FinishedGoods, SalesOrder } from '../../core/models';

@Component({
  selector: 'app-sales',
  standalone: true,
  imports: [
    CommonModule, FormsModule, MatCardModule, MatFormFieldModule, MatInputModule,
    MatSelectModule, MatButtonModule, MatTableModule, MatSnackBarModule
  ],
  template: `
    <h1 class="page-title">Satış</h1>

    <mat-card class="mb">
      <mat-card-header><mat-card-title>Yeni Satış</mat-card-title>
        <mat-card-subtitle>Mamul stoğundan düşülür; satış anındaki maliyet (COGS) dondurulur</mat-card-subtitle>
      </mat-card-header>
      <mat-card-content>
        <div class="row">
          <mat-form-field>
            <mat-label>Ürün (eldeki mamul)</mat-label>
            <mat-select [(ngModel)]="sale.productId" (ngModelChange)="onProductChange()">
              @for (f of inStock(); track f.id) {
                <mat-option [value]="f.productId">{{ f.product?.name }} — {{ f.quantityOnHand }} adet</mat-option>
              }
            </mat-select>
          </mat-form-field>
          <mat-form-field><mat-label>Adet</mat-label><input matInput type="number" [(ngModel)]="sale.quantity" /></mat-form-field>
          <mat-form-field><mat-label>Birim satış fiyatı</mat-label><input matInput type="number" [(ngModel)]="sale.unitPrice" /></mat-form-field>
          <mat-form-field><mat-label>Müşteri</mat-label><input matInput [(ngModel)]="sale.customerName" /></mat-form-field>
        </div>
        @if (selected(); as f) {
          <p class="muted">Eldeki: <b>{{ f.quantityOnHand }} adet</b> · birim maliyet {{ s.money(f.averageUnitCost) }}
            @if (sale.unitPrice) { · tahmini birim kâr <b>{{ s.money(sale.unitPrice - f.averageUnitCost) }}</b> }
          </p>
        }
        <button mat-raised-button color="primary" (click)="create()" [disabled]="!sale.productId || !sale.quantity || !sale.unitPrice">Satışı Kaydet</button>
      </mat-card-content>
    </mat-card>

    <mat-card>
      <mat-card-header><mat-card-title>Satışlar</mat-card-title></mat-card-header>
      <mat-card-content>
        <table mat-table [dataSource]="sales()" class="full">
          <ng-container matColumnDef="date"><th mat-header-cell *matHeaderCellDef>Tarih</th><td mat-cell *matCellDef="let x">{{ x.saleDate | date:'dd.MM.yyyy' }}</td></ng-container>
          <ng-container matColumnDef="product"><th mat-header-cell *matHeaderCellDef>Ürün</th><td mat-cell *matCellDef="let x">{{ x.product?.name }}</td></ng-container>
          <ng-container matColumnDef="customer"><th mat-header-cell *matHeaderCellDef>Müşteri</th><td mat-cell *matCellDef="let x">{{ x.customerName || '—' }}</td></ng-container>
          <ng-container matColumnDef="qty"><th mat-header-cell *matHeaderCellDef class="right">Adet</th><td mat-cell *matCellDef="let x" class="right">{{ x.quantity }}</td></ng-container>
          <ng-container matColumnDef="price"><th mat-header-cell *matHeaderCellDef class="right">Birim fiyat</th><td mat-cell *matCellDef="let x" class="right">{{ s.money(x.unitPrice) }}</td></ng-container>
          <ng-container matColumnDef="revenue"><th mat-header-cell *matHeaderCellDef class="right">Ciro</th><td mat-cell *matCellDef="let x" class="right">{{ s.money(x.revenue) }}</td></ng-container>
          <ng-container matColumnDef="profit"><th mat-header-cell *matHeaderCellDef class="right">Kâr</th><td mat-cell *matCellDef="let x" class="right" [class.low]="x.profit < 0">{{ s.money(x.profit) }}</td></ng-container>
          <tr mat-header-row *matHeaderRowDef="cols"></tr>
          <tr mat-row *matRowDef="let row; columns: cols"></tr>
        </table>
        @if (!sales().length) { <p class="muted">Henüz satış yok.</p> }
      </mat-card-content>
    </mat-card>
  `,
  styles: [`.right { text-align: right; }`]
})
export class SalesComponent implements OnInit {
  private api = inject(ApiService);
  private snack = inject(MatSnackBar);
  s = inject(SettingsStore);

  finished = signal<FinishedGoods[]>([]);
  sales = signal<SalesOrder[]>([]);
  cols = ['date', 'product', 'customer', 'qty', 'price', 'revenue', 'profit'];

  sale: { productId: string; quantity: number; unitPrice: number; customerName?: string } =
    { productId: '', quantity: 1, unitPrice: 0 };

  inStock = () => this.finished().filter(f => f.quantityOnHand > 0);
  selected = () => this.finished().find(f => f.productId === this.sale.productId) ?? null;

  ngOnInit(): void { this.refresh(); }

  refresh(): void {
    this.api.getFinishedGoods().subscribe(v => this.finished.set(v));
    this.api.getSales().subscribe(v => this.sales.set(v));
  }

  /** Ürün seçilince önerilen satış fiyatını reçete fiyatlandırmasından doldur. */
  onProductChange(): void {
    if (!this.sale.productId) return;
    this.api.pricingForProduct(this.sale.productId).subscribe({
      next: (pp) => { if (!this.sale.unitPrice) this.sale.unitPrice = Math.round(pp.price.salePrice * 100) / 100; },
      error: () => {}
    });
  }

  create(): void {
    this.api.createSale(this.sale).subscribe({
      next: () => {
        this.snack.open('Satış kaydedildi, mamul stoğu güncellendi', 'Tamam', { duration: 3000 });
        this.sale = { productId: '', quantity: 1, unitPrice: 0 };
        this.refresh();
      },
      error: (e) => this.snack.open(e?.error?.error ?? 'Hata', 'Kapat', { duration: 5000 })
    });
  }
}
