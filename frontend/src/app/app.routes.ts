import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
  {
    path: 'dashboard',
    loadComponent: () => import('./dashboard/dashboard-page.component').then(m => m.DashboardPageComponent)
  },
  {
    path: 'destinations',
    loadComponent: () => import('./destinations/destinations-page.component').then(m => m.DestinationsPageComponent)
  },
  {
    path: 'destinations/new',
    loadComponent: () => import('./destinations/create-destination-page.component').then(m => m.CreateDestinationPageComponent)
  },
  {
    path: 'destinations/:id',
    loadComponent: () => import('./destinations/destination-detail-page.component').then(m => m.DestinationDetailPageComponent)
  },
  { path: '**', redirectTo: 'dashboard' }
];
