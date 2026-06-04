import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../core/api.service';
import { SettingsStore } from '../../core/settings.store';
import { ProductionTrendPoint, ProductProfitability, ReportSummary } from '../../core/models';

interface Bar { x: number; y: number; w: number; h: number; cls: string; tip: string; }
interface Group { labelX: number; label: string; bars: Bar[]; }

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule, FormsModule, MatCardModule, MatIconModule, MatTableModule, MatButtonToggleModule],
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
          <div class="value">{{ s.money(sm.totalRevenue) }}</div><div class="muted">Toplam ciro ({{ sm.unitsSold }} adet satış)</div>
        </mat-card-content></mat-card>
        <mat-card><mat-card-content>
          <div class="value" [class.low]="sm.totalProfit < 0">{{ s.money(sm.totalProfit) }}</div><div class="muted">Toplam satış kârı</div>
        </mat-card-content></mat-card>
        <mat-card><mat-card-content>
          <div class="value" [class.low]="sm.lowStockCount > 0">{{ sm.lowStockCount }}</div><div class="muted">Düşük stok uyarısı</div>
        </mat-card-content></mat-card>
      </div>
    }

    <mat-card class="mb">
      <mat-card-header>
        <mat-card-title>Üretim & Satış Trendi (aylık)</mat-card-title>
        <mat-card-subtitle>
          <span class="legend"><span class="sw prod"></span>Üretim</span>
          <span class="legend"><span class="sw sales"></span>Satış</span>
          — çubuğun üzerine gelince detay görünür
        </mat-card-subtitle>
        <span class="spacer"></span>
        <mat-button-toggle-group [(ngModel)]="months" (change)="loadTrend()" aria-label="Dönem">
          <mat-button-toggle [value]="6">6 ay</mat-button-toggle>
          <mat-button-toggle [value]="12">12 ay</mat-button-toggle>
        </mat-button-toggle-group>
      </mat-card-header>
      <mat-card-content>
        @if (hasData()) {
          <svg class="chart" [attr.viewBox]="'0 0 ' + chartWidth() + ' ' + chartH" preserveAspectRatio="xMidYMid meet" role="img">
            <line [attr.x1]="0" [attr.y1]="baseY" [attr.x2]="chartWidth()" [attr.y2]="baseY" stroke="#ddd" />
            @for (g of groups(); track g.label) {
              <g>
                @for (b of g.bars; track $index) {
                  <rect [attr.x]="b.x" [attr.y]="b.y" [attr.width]="b.w" [attr.height]="b.h" rx="2" [attr.class]="'bar ' + b.cls">
                    <title>{{ b.tip }}</title>
                  </rect>
                }
                <text [attr.x]="g.labelX" [attr.y]="baseY + 18" text-anchor="middle" class="bar-lbl">{{ g.label }}</text>
              </g>
            }
          </svg>
        } @else {
          <p class="muted">Seçilen dönemde üretim veya satış yok.</p>
        }
      </mat-card-content>
    </mat-card>

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
    .chart { width: 100%; height: 260px; }
    .bar.prod { fill: #3f51b5; }
    .bar.sales { fill: #2e7d32; }
    .bar:hover { opacity: .8; }
    .bar-lbl { font-size: 11px; fill: #777; }
    .legend { margin-right: 14px; display: inline-flex; align-items: center; gap: 6px; }
    .sw { width: 12px; height: 12px; border-radius: 2px; display: inline-block; }
    .sw.prod { background: #3f51b5; }
    .sw.sales { background: #2e7d32; }
  `]
})
export class ReportsComponent implements OnInit {
  private api = inject(ApiService);
  s = inject(SettingsStore);

  summary = signal<ReportSummary | null>(null);
  rows = signal<ProductProfitability[]>([]);
  trend = signal<ProductionTrendPoint[]>([]);
  months = 6;
  cols = ['name', 'cost', 'price', 'profit', 'margin', 'producible'];

  // SVG geometri: her ay için yan yana iki çubuk (üretim + satış).
  readonly slot = 72;
  readonly barW = 26;
  readonly gap = 4;
  readonly chartH = 230;
  readonly baseY = 196;
  readonly topPad = 24;

  chartWidth = computed(() => Math.max(this.trend().length * this.slot, this.slot));
  hasData = computed(() => this.trend().some(p => p.units > 0 || p.salesUnits > 0));

  groups = computed<Group[]>(() => {
    const data = this.trend();
    const maxUnits = Math.max(...data.flatMap(p => [p.units, p.salesUnits]), 1);
    const usableH = this.baseY - this.topPad;
    const pairW = this.barW * 2 + this.gap;
    return data.map((p, i) => {
      const slotStart = i * this.slot + (this.slot - pairW) / 2;
      const mk = (units: number, idx: number, cls: string, tip: string): Bar => {
        const h = (units / maxUnits) * usableH;
        return { x: slotStart + idx * (this.barW + this.gap), y: this.baseY - h, w: this.barW, h, cls, tip };
      };
      return {
        labelX: i * this.slot + this.slot / 2,
        label: p.label,
        bars: [
          mk(p.units, 0, 'prod', `${p.label} — Üretim: ${p.units} adet · ${this.s.money(p.value)}`),
          mk(p.salesUnits, 1, 'sales', `${p.label} — Satış: ${p.salesUnits} adet · ${this.s.money(p.revenue)}`)
        ]
      };
    });
  });

  ngOnInit(): void {
    this.api.reportSummary().subscribe(v => this.summary.set(v));
    this.api.profitability().subscribe(v => this.rows.set(v));
    this.loadTrend();
  }

  loadTrend(): void {
    this.api.productionTrend(this.months).subscribe(v => this.trend.set(v));
  }
}
