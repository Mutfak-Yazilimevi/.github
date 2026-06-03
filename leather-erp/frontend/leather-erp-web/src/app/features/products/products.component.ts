import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ApiService } from '../../core/api.service';
import { SettingsStore } from '../../core/settings.store';
import { LeatherType, Product, Recipe } from '../../core/models';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [
    CommonModule, FormsModule, MatCardModule, MatFormFieldModule, MatInputModule,
    MatSelectModule, MatButtonModule, MatTableModule, MatExpansionModule, MatSnackBarModule
  ],
  template: `
    <h1 class="page-title">Ürünler & Reçete</h1>

    <mat-card class="mb">
      <mat-card-header><mat-card-title>Yeni Ürün</mat-card-title></mat-card-header>
      <mat-card-content>
        <div class="row">
          <mat-form-field><mat-label>Ad</mat-label><input matInput [(ngModel)]="newProduct.name" /></mat-form-field>
          <mat-form-field><mat-label>SKU</mat-label><input matInput [(ngModel)]="newProduct.sku" /></mat-form-field>
          <mat-form-field><mat-label>Kategori</mat-label><input matInput [(ngModel)]="newProduct.category" /></mat-form-field>
        </div>
        <button mat-raised-button color="primary" (click)="addProduct()" [disabled]="!newProduct.name">Ürün Ekle</button>
      </mat-card-content>
    </mat-card>

    @for (p of products(); track p.id) {
      <mat-expansion-panel class="mb">
        <mat-expansion-panel-header>
          <mat-panel-title>{{ p.name }}</mat-panel-title>
          <mat-panel-description>
            {{ p.sku || '—' }} · {{ p.recipe ? 'Reçete tanımlı' : 'Reçete yok' }}
          </mat-panel-description>
        </mat-expansion-panel-header>

        <div class="recipe">
          <p class="muted">Reçete: bir adet ürün için gereken deri, fire, işçilik ve genel gider.</p>
          <div class="row">
            <mat-form-field>
              <mat-label>Deri tipi</mat-label>
              <mat-select [(ngModel)]="recipeForms[p.id].leatherTypeId">
                @for (t of types(); track t.id) { <mat-option [value]="t.id">{{ t.name }}</mat-option> }
              </mat-select>
            </mat-form-field>
            <mat-form-field><mat-label>Net deri (dm²)</mat-label><input matInput type="number" [(ngModel)]="recipeForms[p.id].netLeatherDm2" /></mat-form-field>
            <mat-form-field><mat-label>Fire oranı (0–1)</mat-label><input matInput type="number" step="0.01" [(ngModel)]="recipeForms[p.id].wasteRate" /></mat-form-field>
          </div>
          <div class="row">
            <mat-form-field><mat-label>İşçilik</mat-label><input matInput type="number" [(ngModel)]="recipeForms[p.id].laborCost" /></mat-form-field>
            <mat-form-field><mat-label>Genel gider</mat-label><input matInput type="number" [(ngModel)]="recipeForms[p.id].overheadCost" /></mat-form-field>
          </div>
          <button mat-raised-button color="primary" (click)="saveRecipe(p)" [disabled]="!recipeForms[p.id].leatherTypeId">Reçeteyi Kaydet</button>

          @if (p.inventory) {
            <p class="mt muted">Eldeki stok: <b>{{ p.inventory.quantityOnHand }} adet</b> · değer {{ s.money(p.inventory.totalValue) }}</p>
          }
        </div>
      </mat-expansion-panel>
    } @empty { <p class="muted">Henüz ürün yok.</p> }
  `,
  styles: [`.recipe{padding:8px 4px}`]
})
export class ProductsComponent implements OnInit {
  private api = inject(ApiService);
  private snack = inject(MatSnackBar);
  s = inject(SettingsStore);

  products = signal<Product[]>([]);
  types = signal<LeatherType[]>([]);
  recipeForms: Record<string, Recipe> = {};

  newProduct: Partial<Product> = { name: '', isActive: true };

  ngOnInit(): void { this.refresh(); }

  refresh(): void {
    this.api.getLeatherTypes().subscribe(v => this.types.set(v));
    this.api.getProducts().subscribe(v => {
      this.products.set(v);
      for (const p of v) {
        this.recipeForms[p.id] = p.recipe
          ? { ...p.recipe }
          : { leatherTypeId: '', netLeatherDm2: 0, wasteRate: this.s.settings().defaultWasteRate, laborCost: 0, overheadCost: 0 };
      }
    });
  }

  addProduct(): void {
    this.api.createProduct(this.newProduct).subscribe(() => {
      this.snack.open('Ürün eklendi', 'Tamam', { duration: 2500 });
      this.newProduct = { name: '', isActive: true };
      this.refresh();
    });
  }

  saveRecipe(p: Product): void {
    this.api.setRecipe(p.id, this.recipeForms[p.id]).subscribe(() => {
      this.snack.open('Reçete kaydedildi', 'Tamam', { duration: 2500 });
      this.refresh();
    });
  }
}
