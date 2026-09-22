import { Routes } from '@angular/router';

export const SELLER_ROUTES: Routes = [
  {
    path: 'my-store',
    loadComponent: () =>
      import('./my-store/my-store-page/my-store-page.component').then((m) => m.MyStorePageComponent),
  },
  { path: '', pathMatch: 'full', redirectTo: 'my-store' },
];
