import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { ApiService } from '../../core/api.service';
import { SettingsStore } from '../../core/settings.store';
import { ProductProfitability, ReportSummary } from '../../core/models';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatIconModule, MatTableModule],
  template: `
    <h1 class="page-title">Raporlar</h1>

    @if (summary(); as sm) {
      <div class="cards mb">
        <mat-card><mat-card-content>
          <div class="value">{{ s.area(sm.totalLeatherStockDm2) }}</div><div class="muted">Toplam deri stoğu</div>
        </mat-card-content></mat-card>
        <mat-card><mat-card-content>
          <div class="value">{{ s.money(sm.leatherStockValue) }}</div><div class="muted">Deri stok değeri</div>
        </mat-card-content></mat-card>
        <mat-card><mat-card-content>
          <div class="value">{{ s.money(sm.finishedGoodsValue) }}</div><div class="muted">Mamul değeri ({{ sm.finishedGoodsUnits }} adet)</div>
        </mat-card-content></mat-card>
        <mat-card><mat-card-content>
          <div class="value">{{ sm.unitsProduced }}</div><div class="muted">Üretilen adet ({{ sm.productionOrderCount }} emir)</div>
        </mat-card-content></mat-card>
        <mat-card><mat-card-content>
          <div class="value" [class.low]="sm.lowStockCount > 0">{{ sm.lowStockCount }}</div><div class="muted">Düşük stok uyarısı</div>
        </mat-card-content></mat-card>
      </div>
    }

    <mat-card>
      <mat-card-header><mat-card-title>Ürün Kârlılığı</mat-card-title>
        <mat-card-subtitle>Eldeki deri stoğunun ağırlıklı ortalama maliyetine göre</mat-card-subtitle>
      </mat-card-header>
      <mat-card-content>
        <table mat-table [dataSource]="rows()" class="full">
          <ng-container matColumnDef="name"><th mat-header-cell *matHeaderCellDef>Ürün</th><td mat-cell *matCellDef="let r">{{ r.productName }}</td></ng-container>
          <ng-container matColumnDef="cost"><th mat-header-cell *matHeaderCellDef class="right">Birim maliyet</th><td mat-cell *matCellDef="let r" class="right">{{ s.money(r.unitCost) }}</td></ng-container>
          <ng-container matColumnDef="price"><th mat-header-cell *matHeaderCellDef class="right">Satış fiyatı</th><td mat-cell *matCellDef="let r" class="right">{{ s.money(r.salePrice) }}</td></ng-container>
          <ng-container matColumnDef="profit"><th mat-header-cell *matHeaderCellDef class="right">Birim kâr</th><td mat-cell *matCellDef="let r" class="right">{{ s.money(r.unitProfit) }}</td></ng-container>
          <ng-container matColumnDef="margin"><th mat-header-cell *matHeaderCellDef class="right">Marj</th><td mat-cell *matCellDef="let r" class="right">%{{ (r.profitMargin * 100) | number:'1.0-0' }}</td></ng-container>
          <ng-container matColumnDef="producible"><th mat-header-cell *matHeaderCellDef class="right">Üretilebilir</th><td mat-cell *matCellDef="let r" class="right">{{ r.producibleUnits }} adet</td></ng-container>
          <tr mat-header-row *matHeaderRowDef="cols"></tr>
          <tr mat-row *matRowDef="let row; columns: cols"></tr>
        </table>
        @if (!rows().length) { <p class="muted">Reçetesi tanımlı aktif ürün yok.</p> }
      </mat-card-content>
    </mat-card>
  `,
  styles: [`
    .value { font-size: 20px; font-weight: 600; }
    .right { text-align: right; }
  `]
})
export class ReportsComponent implements OnInit {
  private api = inject(ApiService);
  s = inject(SettingsStore);

  summary = signal<ReportSummary | null>(null);
  rows = signal<ProductProfitability[]>([]);
  cols = ['name', 'cost', 'price', 'profit', 'margin', 'producible'];

  ngOnInit(): void {
    this.api.reportSummary().subscribe(v => this.summary.set(v));
    this.api.profitability().subscribe(v => this.rows.set(v));
  }
}
