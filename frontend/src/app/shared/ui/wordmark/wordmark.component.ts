import { Component, Input } from '@angular/core';

/**
 * Brand wordmark: "Gentongku" with the "o" in "gentong" rendered as a small
 * pot glyph (AGENTS.md section 11). Inline SVG, `currentColor` fill so it
 * inherits text color per theme (light storefront vs dark admin). Reuse
 * this component anywhere the brand mark appears rather than re-deriving
 * the glyph markup.
 */
@Component({
  selector: 'app-wordmark',
  standalone: true,
  template: `
    <span class="wordmark" [style.fontSize.px]="size">
      <span class="wordmark__text">Gent</span>
      <svg class="wordmark__pot" viewBox="0 0 24 24" [attr.width]="size * 0.62" [attr.height]="size * 0.62" fill="none" aria-hidden="true">
        <path
          d="M12 3.4c-1.6 0-2.9.6-2.9 1.5 0 .3.1.5.4.8-2.6.9-4.4 3-4.4 6.4 0 4.1 3.1 7.1 6.9 7.1s6.9-3 6.9-7.1c0-3.4-1.8-5.5-4.4-6.4.3-.3.4-.5.4-.8 0-.9-1.3-1.5-2.9-1.5Z"
          fill="currentColor"
        />
        <rect x="8.6" y="2.2" width="6.8" height="1.6" rx="0.8" fill="currentColor" />
      </svg>
      <span class="wordmark__text">ngku</span>
    </span>
  `,
  styles: [
    `
      .wordmark {
        display: inline-flex;
        align-items: baseline;
        font-family: var(--font-display);
        font-weight: 700;
        line-height: 1;
        color: inherit;
      }
      .wordmark__pot {
        position: relative;
        top: 0.1em;
        margin: 0 -0.05em;
      }
    `,
  ],
})
export class WordmarkComponent {
  @Input() size = 28;
}
