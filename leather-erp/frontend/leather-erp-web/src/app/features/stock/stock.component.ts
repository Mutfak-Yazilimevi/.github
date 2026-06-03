import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ApiService } from '../../core/api.service';
import { SettingsStore } from '../../core/settings.store';
import { LeatherLot, LeatherType, StockLevel, Supplier, UnitOfMeasure } from '../../core/models';

@Component({
  selector: 'app-stock',
  standalone: true,
  imports: [
    CommonModule, FormsModule, MatCardModule, MatFormFieldModule, MatInputModule,
    MatSelectModule, MatButtonModule, MatTableModule, MatIconModule, MatSnackBarModule
  ],
  template: `
    <h1 class="page-title">Deri Stok</h1>

    <div class="row mb">
      <mat-card>
        <mat-card-header><mat-card-title>Stok Seviyeleri</mat-card-title></mat-card-header>
        <mat-card-content>
          <table mat-table [dataSource]="levels()" class="full">
            <ng-container matColumnDef="name">
              <th mat-header-cell *matHeaderCellDef>Deri Tipi</th>
              <td mat-cell *matCellDef="let l">{{ l.leatherTypeName }}</td>
            </ng-container>
            <ng-container matColumnDef="qty">
              <th mat-header-cell *matHeaderCellDef class="right">Kalan</th>
              <td mat-cell *matCellDef="let l" class="right" [class.low]="l.isLow">{{ s.area(l.remainingDm2) }}</td>
            </ng-container>
            <ng-container matColumnDef="value">
              <th mat-header-cell *matHeaderCellDef class="right">Değer</th>
              <td mat-cell *matCellDef="let l" class="right">{{ s.money(l.stockValue) }}</td>
            </ng-container>
            <tr mat-header-row *matHeaderRowDef="levelCols"></tr>
            <tr mat-row *matRowDef="let row; columns: levelCols"></tr>
          </table>
          @if (!levels().length) { <p class="muted">Henüz stok yok.</p> }
        </mat-card-content>
      </mat-card>

      <mat-card>
        <mat-card-header><mat-card-title>Yeni Deri Tipi</mat-card-title></mat-card-header>
        <mat-card-content>
          <mat-form-field class="full-width"><mat-label>Ad</mat-label><input matInput [(ngModel)]="newType.name" placeholder="Vaketa - Kahve - 1.8mm" /></mat-form-field>
          <div class="row">
            <mat-form-field><mat-label>Cins</mat-label><input matInput [(ngModel)]="newType.kind" /></mat-form-field>
            <mat-form-field><mat-label>Renk</mat-label><input matInput [(ngModel)]="newType.color" /></mat-form-field>
          </div>
          <div class="row">
            <mat-form-field><mat-label>Kalınlık (mm)</mat-label><input matInput type="number" [(ngModel)]="newType.thicknessMm" /></mat-form-field>
            <mat-form-field><mat-label>Düşük stok eşiği (dm²)</mat-label><input matInput type="number" [(ngModel)]="newType.lowStockThresholdDm2" /></mat-form-field>
          </div>
          <button mat-raised-button color="primary" (click)="addType()" [disabled]="!newType.name">Deri Tipi Ekle</button>
        </mat-card-content>
      </mat-card>
    </div>

    <mat-card class="mb">
      <mat-card-header><mat-card-title>Deri Girişi (Lot)</mat-card-title></mat-card-header>
      <mat-card-content>
        <div class="row">
          <mat-form-field>
            <mat-label>Deri tipi</mat-label>
            <mat-select [(ngModel)]="lot.leatherTypeId">
              @for (t of types(); track t.id) { <mat-option [value]="t.id">{{ t.name }}</mat-option> }
            </mat-select>
          </mat-form-field>
          <mat-form-field>
            <mat-label>Tedarikçi</mat-label>
            <mat-select [(ngModel)]="lot.supplierId">
              <mat-option [value]="undefined">—</mat-option>
              @for (sup of suppliers(); track sup.id) { <mat-option [value]="sup.id">{{ sup.name }}</mat-option> }
            </mat-select>
          </mat-form-field>
          <mat-form-field><mat-label>Lot no</mat-label><input matInput [(ngModel)]="lot.lotNumber" /></mat-form-field>
        </div>
        <div class="row">
          <mat-form-field><mat-label>Miktar</mat-label><input matInput type="number" [(ngModel)]="lot.quantity" /></mat-form-field>
          <mat-form-field>
            <mat-label>Birim</mat-label>
            <mat-select [(ngModel)]="lot.unit">
              <mat-option value="Dm2">dm²</mat-option>
              <mat-option value="SquareFoot">ayak²</mat-option>
            </mat-select>
          </mat-form-field>
          <mat-form-field><mat-label>Birim maliyet (/dm²)</mat-label><input matInput type="number" [(ngModel)]="lot.unitCostPerDm2" /></mat-form-field>
        </div>
        <button mat-raised-button color="primary" (click)="addLot()" [disabled]="!lot.leatherTypeId || !lot.quantity">Stok Girişi Yap</button>
      </mat-card-content>
    </mat-card>

    <mat-card>
      <mat-card-header><mat-card-title>Lotlar</mat-card-title></mat-card-header>
      <mat-card-content>
        <table mat-table [dataSource]="lots()" class="full">
          <ng-container matColumnDef="type"><th mat-header-cell *matHeaderCellDef>Deri</th><td mat-cell *matCellDef="let l">{{ l.leatherType?.name }}</td></ng-container>
          <ng-container matColumnDef="lotNo"><th mat-header-cell *matHeaderCellDef>Lot</th><td mat-cell *matCellDef="let l">{{ l.lotNumber || '—' }}</td></ng-container>
          <ng-container matColumnDef="date"><th mat-header-cell *matHeaderCellDef>Tarih</th><td mat-cell *matCellDef="let l">{{ l.purchaseDate | date:'dd.MM.yyyy' }}</td></ng-container>
          <ng-container matColumnDef="remaining"><th mat-header-cell *matHeaderCellDef class="right">Kalan</th><td mat-cell *matCellDef="let l" class="right">{{ s.area(l.remainingDm2) }}</td></ng-container>
          <ng-container matColumnDef="cost"><th mat-header-cell *matHeaderCellDef class="right">Birim mlt.</th><td mat-cell *matCellDef="let l" class="right">{{ s.money(l.unitCostPerDm2) }}</td></ng-container>
          <tr mat-header-row *matHeaderRowDef="lotCols"></tr>
          <tr mat-row *matRowDef="let row; columns: lotCols"></tr>
        </table>
        @if (!lots().length) { <p class="muted">Henüz lot yok.</p> }
      </mat-card-content>
    </mat-card>
  `,
  styles: [`.right{text-align:right}`]
})
export class StockComponent implements OnInit {
  private api = inject(ApiService);
  private snack = inject(MatSnackBar);
  s = inject(SettingsStore);

  levels = signal<StockLevel[]>([]);
  types = signal<LeatherType[]>([]);
  suppliers = signal<Supplier[]>([]);
  lots = signal<LeatherLot[]>([]);

  levelCols = ['name', 'qty', 'value'];
  lotCols = ['type', 'lotNo', 'date', 'remaining', 'cost'];

  newType: Partial<LeatherType> = { name: '', lowStockThresholdDm2: 0 };
  lot: { leatherTypeId: string; supplierId?: string; lotNumber?: string; quantity: number; unit: UnitOfMeasure; unitCostPerDm2: number } =
    { leatherTypeId: '', quantity: 0, unit: 'Dm2', unitCostPerDm2: 0 };

  ngOnInit(): void { this.refresh(); }

  refresh(): void {
    this.api.getStockLevels().subscribe(v => this.levels.set(v));
    this.api.getLeatherTypes().subscribe(v => this.types.set(v));
    this.api.getSuppliers().subscribe(v => this.suppliers.set(v));
    this.api.getLots().subscribe(v => this.lots.set(v));
  }

  addType(): void {
    this.api.createLeatherType(this.newType).subscribe(() => {
      this.snack.open('Deri tipi eklendi', 'Tamam', { duration: 2500 });
      this.newType = { name: '', lowStockThresholdDm2: 0 };
      this.refresh();
    });
  }

  addLot(): void {
    this.api.addLot(this.lot).subscribe({
      next: () => {
        this.snack.open('Stok girişi yapıldı', 'Tamam', { duration: 2500 });
        this.lot = { leatherTypeId: '', quantity: 0, unit: 'Dm2', unitCostPerDm2: 0 };
        this.refresh();
      },
      error: (e) => this.snack.open(e?.error?.error ?? 'Hata', 'Kapat', { duration: 4000 })
    });
  }
}
