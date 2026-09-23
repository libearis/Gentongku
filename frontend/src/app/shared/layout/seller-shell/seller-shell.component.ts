import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthStore } from '../../../core/auth/auth-store.service';
import { WordmarkComponent } from '../../ui/wordmark/wordmark.component';

// "Shopping" tab links into /storefront/** (BuyerShell) rather than duplicating that UI here.
@Component({
  selector: 'app-seller-shell',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, RouterOutlet, WordmarkComponent],
  templateUrl: './seller-shell.component.html',
  styleUrl: './seller-shell.component.scss',
})
export class SellerShellComponent {
  readonly auth = inject(AuthStore);

  logout(): void {
    this.auth.logout();
  }
}
