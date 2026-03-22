import type { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'dashboard',
    loadComponent: () => import('./features/dashboard/dashboard').then(m => m.DashboardComponent),
  },
  {
    path: 'trading',
    loadComponent: () => import('./features/trading/trading').then(m => m.TradingComponent),
  },
  {
    path: 'portfolio',
    loadComponent: () => import('./features/portfolio/portfolio').then(m => m.PortfolioComponent),
  },
  {
    path: 'signals',
    loadComponent: () => import('./features/signals/signals').then(m => m.SignalsComponent),
  },
  {
    path: '',
    redirectTo: 'trading',
    pathMatch: 'full',
  },
];
