import { Component, Input } from '@angular/core';

// Deliberately a gentong/jug glyph, not a generic cart icon — keep this as the single source for it everywhere.
@Component({
  selector: 'app-gentong-icon',
  standalone: true,
  template: `
    <svg [attr.width]="size" [attr.height]="size" viewBox="0 0 32 32" fill="none" aria-hidden="true">
      <rect x="11" y="3" width="10" height="3" rx="1" fill="currentColor" />
      <path
        d="M12 6.5h8c2.8 1 4.6 3.9 4.6 8 0 6.5-4 11.5-8.6 11.5s-8.6-5-8.6-11.5c0-4.1 1.8-7 4.6-8Z"
        fill="currentColor"
        fill-opacity="0.16"
        stroke="currentColor"
        stroke-width="1.6"
      />
      <path d="M9 13.2c1.9.9 4.3 1.4 7 1.4s5.1-.5 7-1.4" stroke="currentColor" stroke-width="1.4" stroke-linecap="round" />
    </svg>
  `,
})
export class GentongIconComponent {
  @Input() size = 22;
}
