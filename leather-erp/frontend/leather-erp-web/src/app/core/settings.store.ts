import { Injectable, signal } from '@angular/core';
import { ApiService } from './api.service';
import { AppSettings, UnitOfMeasure } from './models';

const SQFT_TO_DM2 = 9.2903;

/** Uygulama ayarlarını tutan ve birim/para birimi gösterimini sağlayan paylaşımlı durum. */
@Injectable({ providedIn: 'root' })
export class SettingsStore {
  readonly settings = signal<AppSettings>({
    displayUnit: 'Dm2', currencyCode: 'TRY', vatRate: 0.2, defaultWasteRate: 0.15, defaultProfitMargin: 0.4
  });

  constructor(private api: ApiService) {}

  load(): void {
    this.api.getSettings().subscribe((s) => this.settings.set(s));
  }

  set(s: AppSettings): void { this.settings.set(s); }

  get unit(): UnitOfMeasure { return this.settings().displayUnit; }
  get unitLabel(): string { return this.settings().displayUnit === 'SquareFoot' ? 'ayak²' : 'dm²'; }

  /** Kanonik dm² değerini gösterim birimine çevirir. */
  fromDm2(valueDm2: number): number {
    return this.unit === 'SquareFoot' ? valueDm2 / SQFT_TO_DM2 : valueDm2;
  }

  /** Para birimini biçimlendirir. */
  money(value: number): string {
    return new Intl.NumberFormat('tr-TR', { style: 'currency', currency: this.settings().currencyCode })
      .format(value ?? 0);
  }

  /** Gösterim birimine çevirip etiketiyle biçimlendirir. */
  area(valueDm2: number): string {
    return `${this.fromDm2(valueDm2).toLocaleString('tr-TR', { maximumFractionDigits: 2 })} ${this.unitLabel}`;
  }
}
