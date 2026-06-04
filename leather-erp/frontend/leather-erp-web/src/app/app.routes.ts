import { Routes } from '@angular/router';
import { authGuard } from './core/auth.guard';

export const routes: Routes = [
  { path: 'login', loadComponent: () => import('./features/login/login.component').then(m => m.LoginComponent) },
  {
    path: '',
    canActivate: [authGuard],
    children: [
      { path: 'dashboard', loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent) },
      { path: 'stock', loadComponent: () => import('./features/stock/stock.component').then(m => m.StockComponent) },
      { path: 'products', loadComponent: () => import('./features/products/products.component').then(m => m.ProductsComponent) },
      { path: 'pricing', loadComponent: () => import('./features/pricing/pricing.component').then(m => m.PricingComponent) },
      { path: 'production', loadComponent: () => import('./features/production/production.component').then(m => m.ProductionComponent) },
      { path: 'reports', loadComponent: () => import('./features/reports/reports.component').then(m => m.ReportsComponent) },
      { path: 'settings', loadComponent: () => import('./features/settings/settings.component').then(m => m.SettingsComponent) },
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
    ]
  },
  { path: '**', redirectTo: '' }
];
