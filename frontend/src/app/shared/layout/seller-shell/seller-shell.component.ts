import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthStore } from '../../../core/auth/auth-store.service';
import { WordmarkComponent } from '../../ui/wordmark/wordmark.component';

/**
 * SellerShell: thin layout with a My Store / Shopping tab switcher.
 * "Shopping" is just a link into /storefront/** (BuyerShell + buyer
 * components) — no duplicate implementation, per AGENTS.md section 4/5.
 */
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
