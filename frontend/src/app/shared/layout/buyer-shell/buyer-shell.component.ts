import { Component, inject } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { AuthStore } from '../../../core/auth/auth-store.service';
import { CartStore } from '../../../features/storefront/data/cart-store.service';
import { GentongIconComponent } from '../../ui/gentong-icon/gentong-icon.component';
import { WordmarkComponent } from '../../ui/wordmark/wordmark.component';

// Also used by Admin (read-only) and Seller when they land on /storefront/**, not just Buyer.
@Component({
  selector: 'app-buyer-shell',
  standalone: true,
  imports: [RouterLink, RouterOutlet, WordmarkComponent, GentongIconComponent],
  templateUrl: './buyer-shell.component.html',
  styleUrl: './buyer-shell.component.scss',
})
export class BuyerShellComponent {
  readonly auth = inject(AuthStore);
  readonly cart = inject(CartStore);

  readonly canShop = () => this.auth.role() === 'Buyer' || this.auth.role() === 'Seller';

  logout(): void {
    this.auth.logout();
  }
}
