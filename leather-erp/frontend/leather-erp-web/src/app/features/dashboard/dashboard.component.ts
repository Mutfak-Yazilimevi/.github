import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatButtonModule } from '@angular/material/button';
import { ApiService } from '../../core/api.service';
import { SettingsStore } from '../../core/settings.store';
import { FinishedGoods, ProductionOrder, StockLevel } from '../../core/models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, MatCardModule, MatIconModule, MatListModule, MatButtonModule],
  template: `
    <h1 class="page-title">Panel</h1>
    <div class="cards mb">
      <mat-card>
        <mat-card-content>
          <div class="kpi"><mat-icon color="primary">inventory_2</mat-icon>
            <div><div class="value">{{ s.area(totalStockDm2()) }}</div><div class="muted">Toplam deri stoğu</div></div>
          </div>
        </mat-card-content>
      </mat-card>
      <mat-card>
        <mat-card-content>
          <div class="kpi"><mat-icon style="color:#2e7d32">payments</mat-icon>
            <div><div class="value">{{ s.money(stockValue()) }}</div><div class="muted">Deri stok değeri</div></div>
          </div>
        </mat-card-content>
      </mat-card>
      <mat-card>
        <mat-card-content>
          <div class="kpi"><mat-icon style="color:#6d4c41">category</mat-icon>
            <div><div class="value">{{ s.money(finishedValue()) }}</div><div class="muted">Mamul stok değeri</div></div>
          </div>
        </mat-card-content>
      </mat-card>
      <mat-card>
        <mat-card-content>
          <div class="kpi"><mat-icon [style.color]="lowStock().length ? '#d32f2f' : '#9e9e9e'">warning</mat-icon>
            <div><div class="value">{{ lowStock().length }}</div><div class="muted">Düşük stok uyarısı</div></div>
          </div>
        </mat-card-content>
      </mat-card>
    </div>

    <div class="row">
      <mat-card>
        <mat-card-header><mat-card-title>Deri Stok Seviyeleri</mat-card-title></mat-card-header>
        <mat-card-content>
          @for (lvl of levels(); track lvl.leatherTypeId) {
            <div class="line">
              <span>{{ lvl.leatherTypeName }}</span>
              <span [class.low]="lvl.isLow">{{ s.area(lvl.remainingDm2) }}</span>
            </div>
          } @empty { <p class="muted">Henüz deri girişi yok. <a routerLink="/stock">Stok ekle</a></p> }
        </mat-card-content>
      </mat-card>

      <mat-card>
        <mat-card-header><mat-card-title>Son Üretim Emirleri</mat-card-title></mat-card-header>
        <mat-card-content>
          @for (o of recentOrders(); track o.id) {
            <div class="line">
              <span>{{ o.product?.name }} × {{ o.quantity }}</span>
              <span class="muted">{{ o.status }}</span>
            </div>
          } @empty { <p class="muted">Henüz üretim yok. <a routerLink="/production">Üretim emri oluştur</a></p> }
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .kpi { display: flex; align-items: center; gap: 12px; }
    .kpi mat-icon { font-size: 36px; height: 36px; width: 36px; }
    .value { font-size: 20px; font-weight: 600; }
    .line { display: flex; justify-content: space-between; padding: 8px 0; border-bottom: 1px solid #eee; }
  `]
})
export class DashboardComponent implements OnInit {
  private api = inject(ApiService);
  s = inject(SettingsStore);

  levels = signal<StockLevel[]>([]);
  finished = signal<FinishedGoods[]>([]);
  orders = signal<ProductionOrder[]>([]);

  lowStock = computed(() => this.levels().filter(l => l.isLow));
  totalStockDm2 = computed(() => this.levels().reduce((a, l) => a + l.remainingDm2, 0));
  stockValue = computed(() => this.levels().reduce((a, l) => a + l.stockValue, 0));
  finishedValue = computed(() => this.finished().reduce((a, f) => a + f.totalValue, 0));
  recentOrders = computed(() => this.orders().slice(0, 6));

  ngOnInit(): void {
    this.api.getStockLevels().subscribe(v => this.levels.set(v));
    this.api.getFinishedGoods().subscribe(v => this.finished.set(v));
    this.api.getProductionOrders().subscribe(v => this.orders.set(v));
  }
}
