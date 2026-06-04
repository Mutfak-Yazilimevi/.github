import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  AppSettings, CostBreakdown, FinishedGoods, LeatherLot, LeatherType,
  PricingResult, Product, ProductionOrder, ProductPricing, ProductProfitability,
  Recipe, ReportSummary, StockLevel, Supplier, UnitOfMeasure
} from './models';

/** Tüm backend uçlarını saran tek API servisi. */
@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly api = environment.apiUrl;
  constructor(private http: HttpClient) {}

  // --- Ayarlar ---
  getSettings(): Observable<AppSettings> { return this.http.get<AppSettings>(`${this.api}/settings`); }
  updateSettings(s: AppSettings): Observable<AppSettings> { return this.http.put<AppSettings>(`${this.api}/settings`, s); }

  // --- Tedarikçi ---
  getSuppliers(): Observable<Supplier[]> { return this.http.get<Supplier[]>(`${this.api}/suppliers`); }
  createSupplier(s: Partial<Supplier>): Observable<Supplier> { return this.http.post<Supplier>(`${this.api}/suppliers`, s); }

  // --- Deri tipi ---
  getLeatherTypes(): Observable<LeatherType[]> { return this.http.get<LeatherType[]>(`${this.api}/leather-types`); }
  createLeatherType(t: Partial<LeatherType>): Observable<LeatherType> { return this.http.post<LeatherType>(`${this.api}/leather-types`, t); }

  // --- Stok ---
  getStockLevels(): Observable<StockLevel[]> { return this.http.get<StockLevel[]>(`${this.api}/stock/levels`); }
  getLowStock(): Observable<StockLevel[]> { return this.http.get<StockLevel[]>(`${this.api}/stock/low`); }
  getLots(leatherTypeId?: string): Observable<LeatherLot[]> {
    const q = leatherTypeId ? `?leatherTypeId=${leatherTypeId}` : '';
    return this.http.get<LeatherLot[]>(`${this.api}/stock/lots${q}`);
  }
  addLot(lot: { leatherTypeId: string; supplierId?: string; lotNumber?: string; purchaseDate?: string; quantity: number; unit: UnitOfMeasure; unitCostPerDm2: number; }): Observable<LeatherLot> {
    return this.http.post<LeatherLot>(`${this.api}/stock/lots`, lot);
  }

  // --- Ürün & reçete ---
  getProducts(): Observable<Product[]> { return this.http.get<Product[]>(`${this.api}/products`); }
  createProduct(p: Partial<Product>): Observable<Product> { return this.http.post<Product>(`${this.api}/products`, p); }
  setRecipe(productId: string, r: Recipe): Observable<Recipe> { return this.http.put<Recipe>(`${this.api}/products/${productId}/recipe`, r); }

  // --- Üretim ---
  getProductionOrders(): Observable<ProductionOrder[]> { return this.http.get<ProductionOrder[]>(`${this.api}/production`); }
  createProductionOrder(o: { productId: string; quantity: number; notes?: string }): Observable<ProductionOrder> {
    return this.http.post<ProductionOrder>(`${this.api}/production`, o);
  }
  confirmProduction(id: string): Observable<ProductionOrder> { return this.http.post<ProductionOrder>(`${this.api}/production/${id}/confirm`, {}); }
  getFinishedGoods(): Observable<FinishedGoods[]> { return this.http.get<FinishedGoods[]>(`${this.api}/finished-goods`); }

  // --- Fiyatlandırma ---
  calcCost(r: { netLeatherDm2: number; wasteRate: number; unitCostPerDm2: number; laborCost: number; overheadCost: number; }): Observable<CostBreakdown> {
    return this.http.post<CostBreakdown>(`${this.api}/pricing/cost`, r);
  }
  calcPrice(r: { unitCost: number; profitMargin: number; vatRate: number; }): Observable<PricingResult> {
    return this.http.post<PricingResult>(`${this.api}/pricing/price`, r);
  }
  pricingForProduct(productId: string): Observable<ProductPricing> {
    return this.http.get<ProductPricing>(`${this.api}/pricing/product/${productId}`);
  }

  // --- Raporlar ---
  reportSummary(): Observable<ReportSummary> { return this.http.get<ReportSummary>(`${this.api}/reports/summary`); }
  profitability(): Observable<ProductProfitability[]> { return this.http.get<ProductProfitability[]>(`${this.api}/reports/profitability`); }
}
