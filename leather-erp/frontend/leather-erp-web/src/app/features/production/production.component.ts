import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ApiService } from '../../core/api.service';
import { SettingsStore } from '../../core/settings.store';
import { FinishedGoods, Product, ProductionOrder } from '../../core/models';

@Component({
  selector: 'app-production',
  standalone: true,
  imports: [
    CommonModule, FormsModule, MatCardModule, MatFormFieldModule, MatInputModule,
    MatSelectModule, MatButtonModule, MatTableModule, MatChipsModule, MatSnackBarModule
  ],
  template: `
    <h1 class="page-title">Üretim & Mamul Takibi</h1>

    <mat-card class="mb">
      <mat-card-header><mat-card-title>Yeni Üretim Emri</mat-card-title>
        <mat-card-subtitle>Onaylandığında deri stoğundan FIFO düşülür, mamul stoğa eklenir</mat-card-subtitle>
      </mat-card-header>
      <mat-card-content>
        <div class="row">
          <mat-form-field>
            <mat-label>Ürün</mat-label>
            <mat-select [(ngModel)]="order.productId">
              @for (p of products(); track p.id) { <mat-option [value]="p.id">{{ p.name }}</mat-option> }
            </mat-select>
          </mat-form-field>
          <mat-form-field><mat-label>Adet</mat-label><input matInput type="number" [(ngModel)]="order.quantity" /></mat-form-field>
          <mat-form-field><mat-label>Not</mat-label><input matInput [(ngModel)]="order.notes" /></mat-form-field>
        </div>
        <button mat-raised-button color="primary" (click)="create()" [disabled]="!order.productId || !order.quantity">Emir Oluştur (Taslak)</button>
      </mat-card-content>
    </mat-card>

    <mat-card class="mb">
      <mat-card-header><mat-card-title>Üretim Emirleri</mat-card-title></mat-card-header>
      <mat-card-content>
        <table mat-table [dataSource]="orders()" class="full">
          <ng-container matColumnDef="product"><th mat-header-cell *matHeaderCellDef>Ürün</th><td mat-cell *matCellDef="let o">{{ o.product?.name }}</td></ng-container>
          <ng-container matColumnDef="qty"><th mat-header-cell *matHeaderCellDef class="right">Adet</th><td mat-cell *matCellDef="let o" class="right">{{ o.quantity }}</td></ng-container>
          <ng-container matColumnDef="date"><th mat-header-cell *matHeaderCellDef>Tarih</th><td mat-cell *matCellDef="let o">{{ o.orderDate | date:'dd.MM.yyyy' }}</td></ng-container>
          <ng-container matColumnDef="cost"><th mat-header-cell *matHeaderCellDef class="right">Birim mlt.</th><td mat-cell *matCellDef="let o" class="right">{{ o.unitCostSnapshot ? s.money(o.unitCostSnapshot) : '—' }}</td></ng-container>
          <ng-container matColumnDef="status">
            <th mat-header-cell *matHeaderCellDef>Durum</th>
            <td mat-cell *matCellDef="let o">
              <mat-chip [class.confirmed]="o.status==='Confirmed'">{{ statusLabel(o.status) }}</mat-chip>
            </td>
          </ng-container>
          <ng-container matColumnDef="action">
            <th mat-header-cell *matHeaderCellDef></th>
            <td mat-cell *matCellDef="let o">
              @if (o.status === 'Draft') {
                <button mat-stroked-button color="primary" (click)="confirm(o)">Onayla</button>
              }
            </td>
          </ng-container>
          <tr mat-header-row *matHeaderRowDef="cols"></tr>
          <tr mat-row *matRowDef="let row; columns: cols"></tr>
        </table>
        @if (!orders().length) { <p class="muted">Henüz üretim emri yok.</p> }
      </mat-card-content>
    </mat-card>

    <mat-card>
      <mat-card-header><mat-card-title>Eldeki Mamul Stoğu</mat-card-title></mat-card-header>
      <mat-card-content>
        <table mat-table [dataSource]="finished()" class="full">
          <ng-container matColumnDef="product"><th mat-header-cell *matHeaderCellDef>Ürün</th><td mat-cell *matCellDef="let f">{{ f.product?.name }}</td></ng-container>
          <ng-container matColumnDef="qty"><th mat-header-cell *matHeaderCellDef class="right">Adet</th><td mat-cell *matCellDef="let f" class="right">{{ f.quantityOnHand }}</td></ng-container>
          <ng-container matColumnDef="unit"><th mat-header-cell *matHeaderCellDef class="right">Ort. birim mlt.</th><td mat-cell *matCellDef="let f" class="right">{{ s.money(f.averageUnitCost) }}</td></ng-container>
          <ng-container matColumnDef="value"><th mat-header-cell *matHeaderCellDef class="right">Değer</th><td mat-cell *matCellDef="let f" class="right">{{ s.money(f.totalValue) }}</td></ng-container>
          <tr mat-header-row *matHeaderRowDef="fgCols"></tr>
          <tr mat-row *matRowDef="let row; columns: fgCols"></tr>
        </table>
        @if (!finished().length) { <p class="muted">Henüz mamul stoğu yok.</p> }
      </mat-card-content>
    </mat-card>
  `,
  styles: [`
    .right { text-align: right; }
    mat-chip.confirmed { background: #c8e6c9; }
  `]
})
export class ProductionComponent implements OnInit {
  private api = inject(ApiService);
  private snack = inject(MatSnackBar);
  s = inject(SettingsStore);

  products = signal<Product[]>([]);
  orders = signal<ProductionOrder[]>([]);
  finished = signal<FinishedGoods[]>([]);

  cols = ['product', 'qty', 'date', 'cost', 'status', 'action'];
  fgCols = ['product', 'qty', 'unit', 'value'];

  order: { productId: string; quantity: number; notes?: string } = { productId: '', quantity: 1 };

  ngOnInit(): void { this.refresh(); }

  refresh(): void {
    this.api.getProducts().subscribe(v => this.products.set(v));
    this.api.getProductionOrders().subscribe(v => this.orders.set(v));
    this.api.getFinishedGoods().subscribe(v => this.finished.set(v));
  }

  statusLabel(s: string): string {
    return s === 'Confirmed' ? 'Onaylandı' : s === 'Cancelled' ? 'İptal' : 'Taslak';
  }

  create(): void {
    this.api.createProductionOrder(this.order).subscribe(() => {
      this.snack.open('Üretim emri oluşturuldu (taslak)', 'Tamam', { duration: 2500 });
      this.order = { productId: '', quantity: 1 };
      this.refresh();
    });
  }

  confirm(o: ProductionOrder): void {
    this.api.confirmProduction(o.id).subscribe({
      next: () => {
        this.snack.open('Üretim onaylandı, stoklar güncellendi', 'Tamam', { duration: 3000 });
        this.refresh();
      },
      error: (e) => this.snack.open(e?.error?.error ?? 'Hata', 'Kapat', { duration: 5000 })
    });
  }
}
