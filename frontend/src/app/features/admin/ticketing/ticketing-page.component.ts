import { Component, signal } from '@angular/core';

interface Ticket {
  issueId: string;
  subject: string;
  status: 'Sent' | 'Error';
}

@Component({
  selector: 'app-ticketing-page',
  standalone: true,
  templateUrl: './ticketing-page.component.html',
  styleUrl: './ticketing-page.component.scss',
})
export class TicketingPageComponent {
  readonly tickets = signal<Ticket[]>([
    { issueId: 'ISSUE-88213', subject: 'Redis cache miss spike on Order.status filter', status: 'Sent' },
  ]);
}
