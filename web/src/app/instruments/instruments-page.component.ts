import { DatePipe, DecimalPipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Instrument, Quote } from './instrument.model';
import { InstrumentsService } from './instruments.service';

@Component({
  selector: 'app-instruments-page',
  imports: [FormsModule, DatePipe, DecimalPipe],
  templateUrl: './instruments-page.component.html',
  styleUrl: './instruments-page.component.css',
})
export class InstrumentsPageComponent {
  private readonly instrumentsService = inject(InstrumentsService);

  protected readonly assetTypes = ['stock', 'etf', 'crypto', 'forex', 'index'];

  protected readonly instruments = signal<Instrument[]>([]);
  protected readonly quotes = signal<Record<string, Quote>>({});
  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly error = signal<string | null>(null);

  protected readonly symbol = signal('');
  protected readonly name = signal('');
  protected readonly assetType = signal('stock');

  constructor() {
    this.load();
  }

  protected load(): void {
    this.loading.set(true);
    this.instrumentsService.list().subscribe({
      next: (instruments) => {
        this.instruments.set(instruments);
        this.loading.set(false);
        this.refreshQuotes(instruments);
      },
      error: () => {
        this.error.set('failed to load instruments — is the api running?');
        this.loading.set(false);
      },
    });
  }

  /** Live-ish price when a quote arrived, otherwise the last stored close. */
  protected priceOf(instrument: Instrument): number | null {
    return this.quotes()[instrument.id]?.price ?? instrument.lastClose;
  }

  /** Intraday change once a quote arrived, otherwise close-over-close from the DB. */
  protected changeOf(instrument: Instrument): number | null {
    const quote = this.quotes()[instrument.id];
    if (quote && quote.previousClose) {
      return ((quote.price - quote.previousClose) / quote.previousClose) * 100;
    }
    return instrument.changePercent;
  }

  protected add(): void {
    if (!this.symbol().trim() || !this.name().trim()) {
      this.error.set('symbol and name are required');
      return;
    }

    this.saving.set(true);
    this.error.set(null);
    this.instrumentsService
      .create({
        symbol: this.symbol().trim(),
        name: this.name().trim(),
        assetType: this.assetType(),
      })
      .subscribe({
        next: () => {
          this.symbol.set('');
          this.name.set('');
          this.saving.set(false);
          this.load();
        },
        error: (err: HttpErrorResponse) => {
          this.saving.set(false);
          this.error.set(this.messageFrom(err));
        },
      });
  }

  private refreshQuotes(instruments: Instrument[]): void {
    for (const instrument of instruments) {
      this.instrumentsService.getQuote(instrument.id).subscribe({
        next: (quote) =>
          this.quotes.update((existing) => ({ ...existing, [instrument.id]: quote })),
        // Quotes are best-effort decoration; stored closes remain on screen.
        error: () => undefined,
      });
    }
  }

  private messageFrom(err: HttpErrorResponse): string {
    if (err.status === 409) {
      return err.error?.detail ?? 'instrument is already on the list';
    }
    if (err.status === 400 && err.error?.errors) {
      return Object.values<string[]>(err.error.errors).flat().join(' ');
    }
    return `request failed (${err.status || 'network error'})`;
  }
}
