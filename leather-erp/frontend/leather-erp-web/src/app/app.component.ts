import { Component, OnInit, inject } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { AuthService } from './core/auth.service';
import { SettingsStore } from './core/settings.store';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule, RouterOutlet, RouterLink, RouterLinkActive,
    MatToolbarModule, MatSidenavModule, MatListModule, MatIconModule, MatButtonModule
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  auth = inject(AuthService);
  private settings = inject(SettingsStore);
  private router = inject(Router);

  readonly nav = [
    { path: '/dashboard', label: 'Panel', icon: 'dashboard' },
    { path: '/stock', label: 'Deri Stok', icon: 'inventory_2' },
    { path: '/products', label: 'Ürünler & Reçete', icon: 'category' },
    { path: '/pricing', label: 'Maliyet & Fiyat', icon: 'calculate' },
    { path: '/production', label: 'Üretim & Mamul', icon: 'precision_manufacturing' },
    { path: '/settings', label: 'Ayarlar', icon: 'settings' }
  ];

  ngOnInit(): void {
    if (this.auth.isLoggedIn()) this.settings.load();
  }

  logout(): void {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
