import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { ApiService } from '../../core/api.service';
import { SettingsStore } from '../../core/settings.store';
import { CostBreakdown, PricingResult, Product, ProductPricing } from '../../core/models';

@Component({
  selector: 'app-pricing',
  standalone: true,
  imports: [
    CommonModule, FormsModule, MatCardModule, MatFormFieldModule, MatInputModule,
    MatSelectModule, MatButtonModule, MatDividerModule
  ],
  template: `
    <h1 class="page-title">Maliyet & Fiyat Hesaplama</h1>

    <div class="row mb">
      <mat-card>
        <mat-card-header><mat-card-title>Serbest Hesaplama</mat-card-title>
          <mat-card-subtitle>Fire net deri üzerine eklenir (brüt = net × (1 + fire))</mat-card-subtitle>
        </mat-card-header>
        <mat-card-content>
          <div class="row">
            <mat-form-field><mat-label>Net deri (dm²)</mat-label><input matInput type="number" [(ngModel)]="form.netLeatherDm2" (ngModelChange)="recalc()" /></mat-form-field>
            <mat-form-field><mat-label>Deri birim mlt. (/dm²)</mat-label><input matInput type="number" [(ngModel)]="form.unitCostPerDm2" (ngModelChange)="recalc()" /></mat-form-field>
          </div>
          <div class="row">
            <mat-form-field><mat-label>Fire oranı (0–1)</mat-label><input matInput type="number" step="0.01" [(ngModel)]="form.wasteRate" (ngModelChange)="recalc()" /></mat-form-field>
            <mat-form-field><mat-label>İşçilik</mat-label><input matInput type="number" [(ngModel)]="form.laborCost" (ngModelChange)="recalc()" /></mat-form-field>
            <mat-form-field><mat-label>Genel gider</mat-label><input matInput type="number" [(ngModel)]="form.overheadCost" (ngModelChange)="recalc()" /></mat-form-field>
          </div>
          <mat-divider></mat-divider>
          <div class="row">
            <mat-form-field><mat-label>Kâr marjı (0–1)</mat-label><input matInput type="number" step="0.01" [(ngModel)]="profitMargin" (ngModelChange)="recalc()" /></mat-form-field>
            <mat-form-field><mat-label>KDV oranı (0–1)</mat-label><input matInput type="number" step="0.01" [(ngModel)]="vatRate" (ngModelChange)="recalc()" /></mat-form-field>
          </div>
        </mat-card-content>
      </mat-card>

      <mat-card class="result">
        <mat-card-header><mat-card-title>Sonuç</mat-card-title></mat-card-header>
        <mat-card-content>
          @if (cost(); as c) {
            <div class="line"><span>Brüt deri (fire dahil)</span><b>{{ c.grossLeatherDm2 | number:'1.0-2' }} dm²</b></div>
            <div class="line"><span>Malzeme (net)</span><b>{{ s.money(c.netMaterialCost) }}</b></div>
            <div class="line"><span class="low">+ Fire maliyeti</span><b class="low">{{ s.money(c.wasteCost) }}</b></div>
            <div class="line"><span>İşçilik</span><b>{{ s.money(c.laborCost) }}</b></div>
            <div class="line"><span>Genel gider</span><b>{{ s.money(c.overheadCost) }}</b></div>
            <mat-divider></mat-divider>
            <div class="line total"><span>Birim Maliyet</span><b>{{ s.money(c.unitCost) }}</b></div>
          }
          @if (price(); as p) {
            <div class="line mt"><span>Kâr</span><b>{{ s.money(p.profitAmount) }}</b></div>
            <div class="line"><span>KDV hariç fiyat</span><b>{{ s.money(p.priceBeforeTax) }}</b></div>
            <div class="line"><span>KDV</span><b>{{ s.money(p.vatAmount) }}</b></div>
            <mat-divider></mat-divider>
            <div class="line total sale"><span>Satış Fiyatı (KDV dahil)</span><b>{{ s.money(p.salePrice) }}</b></div>
          }
        </mat-card-content>
      </mat-card>
    </div>

    <mat-card>
      <mat-card-header><mat-card-title>Ürün Reçetesinden Hesapla</mat-card-title>
        <mat-card-subtitle>Deri maliyeti, eldeki stoğun ağırlıklı ortalamasından alınır</mat-card-subtitle>
      </mat-card-header>
      <mat-card-content>
        <mat-form-field>
          <mat-label>Ürün</mat-label>
          <mat-select [(ngModel)]="selectedProductId" (ngModelChange)="loadProduct()">
            @for (p of products(); track p.id) { <mat-option [value]="p.id">{{ p.name }}</mat-option> }
          </mat-select>
        </mat-form-field>
        @if (productPricing(); as pp) {
          <div class="row mt">
            <div class="line"><span>Deri ort. maliyet</span><b>{{ s.money(pp.leatherAvgUnitCostPerDm2) }}/dm²</b></div>
            <div class="line"><span>Birim maliyet</span><b>{{ s.money(pp.cost.unitCost) }}</b></div>
            <div class="line sale"><span>Önerilen satış fiyatı</span><b>{{ s.money(pp.price.salePrice) }}</b></div>
          </div>
        }
      </mat-card-content>
    </mat-card>
  `,
  styles: [`
    .line { display: flex; justify-content: space-between; padding: 6px 0; }
    .total b { font-size: 18px; }
    .sale b { color: #2e7d32; }
    .result { background: #fafbff; }
  `]
})
export class PricingComponent implements OnInit {
  private api = inject(ApiService);
  s = inject(SettingsStore);

  form = { netLeatherDm2: 8, wasteRate: 0.15, unitCostPerDm2: 12.5, laborCost: 40, overheadCost: 15 };
  profitMargin = 0.4;
  vatRate = 0.2;

  cost = signal<CostBreakdown | null>(null);
  price = signal<PricingResult | null>(null);

  products = signal<Product[]>([]);
  selectedProductId = '';
  productPricing = signal<ProductPricing | null>(null);

  ngOnInit(): void {
    this.profitMargin = this.s.settings().defaultProfitMargin;
    this.vatRate = this.s.settings().vatRate;
    this.api.getProducts().subscribe(v => this.products.set(v));
    this.recalc();
  }

  recalc(): void {
    this.api.calcCost(this.form).subscribe(c => {
      this.cost.set(c);
      this.api.calcPrice({ unitCost: c.unitCost, profitMargin: this.profitMargin, vatRate: this.vatRate })
        .subscribe(p => this.price.set(p));
    });
  }

  loadProduct(): void {
    if (!this.selectedProductId) return;
    this.api.pricingForProduct(this.selectedProductId).subscribe(pp => this.productPricing.set(pp));
  }
}
