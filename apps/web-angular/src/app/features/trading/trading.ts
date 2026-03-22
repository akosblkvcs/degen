import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  DestroyRef,
  inject,
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import type { OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import type { InstrumentDto } from '../../core/services/api';
import { ApiService } from '../../core/services/api';
import type { ChartCandle } from '../../shared/components/candlestick-chart/candlestick-chart';
import { CandlestickChartComponent } from '../../shared/components/candlestick-chart/candlestick-chart';

@Component({
  selector: 'app-trading',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, FormsModule, CandlestickChartComponent],
  template: `
    <div class="trading-layout">
      <!-- Top bar -->
      <div class="top-bar">
        <div class="instrument-selector">
          <input
            type="text"
            [(ngModel)]="searchQuery"
            (input)="onSearch()"
            placeholder="Search symbol..."
            class="search-input"
          />
          @if (searchResults.length > 0 && searchQuery) {
            <div class="search-dropdown">
              @for (instrument of searchResults; track instrument.id) {
                <div class="search-item" (click)="selectInstrument(instrument)">
                  <span class="search-symbol">{{ instrument.symbol }}</span>
                  <span class="search-exchange">{{ instrument.exchange }}</span>
                </div>
              }
            </div>
          }
        </div>

        @if (selectedInstrument) {
          <div class="selected-info">
            <span class="selected-symbol">{{ selectedInstrument.symbol }}</span>
            <span class="selected-exchange">{{ selectedInstrument.exchange }}</span>
          </div>
        }

        <div class="interval-selector">
          @for (tf of timeframes; track tf.value) {
            <button
              [class.active]="selectedInterval === tf.value"
              (click)="selectInterval(tf.value)"
              class="tf-btn"
            >
              {{ tf.label }}
            </button>
          }
        </div>
      </div>

      <!-- Chart area -->
      <div class="chart-area">
        @if (selectedInstrument) {
          <app-candlestick-chart [candles]="candles" [symbol]="selectedInstrument.symbol" />
        } @else {
          <div class="empty-state">Select an instrument to start charting</div>
        }
      </div>
    </div>
  `,
  styles: [
    `
      .trading-layout {
        display: flex;
        flex-direction: column;
        height: 100%;
      }

      .top-bar {
        display: flex;
        align-items: center;
        gap: 16px;
        padding: 10px 16px;
        background: #0f0f23;
        border-bottom: 1px solid #1a1a3e;
        flex-shrink: 0;
      }

      .instrument-selector {
        position: relative;
      }

      .search-input {
        background: #1a1a2e;
        border: 1px solid #2a2a4e;
        color: #e0e0e0;
        padding: 8px 12px;
        border-radius: 4px;
        font-size: 13px;
        width: 200px;
        outline: none;
      }

      .search-input:focus {
        border-color: #00d4aa;
      }

      .search-dropdown {
        position: absolute;
        top: 100%;
        left: 0;
        width: 280px;
        background: #1a1a2e;
        border: 1px solid #2a2a4e;
        border-radius: 4px;
        margin-top: 4px;
        max-height: 300px;
        overflow-y: auto;
        z-index: 100;
      }

      .search-item {
        display: flex;
        justify-content: space-between;
        padding: 10px 12px;
        cursor: pointer;
        font-size: 13px;
      }

      .search-item:hover {
        background: #25254a;
      }

      .search-symbol {
        color: #e0e0e0;
        font-weight: 500;
      }

      .search-exchange {
        color: #6666aa;
        font-size: 11px;
        text-transform: uppercase;
      }

      .selected-info {
        display: flex;
        align-items: center;
        gap: 8px;
      }

      .selected-symbol {
        font-size: 16px;
        font-weight: 600;
        color: #e0e0e0;
      }

      .selected-exchange {
        font-size: 11px;
        color: #6666aa;
        text-transform: uppercase;
        background: #1a1a3e;
        padding: 2px 6px;
        border-radius: 3px;
      }

      .interval-selector {
        display: flex;
        gap: 2px;
        margin-left: auto;
      }

      .tf-btn {
        background: transparent;
        border: none;
        color: #6666aa;
        padding: 6px 10px;
        cursor: pointer;
        font-size: 12px;
        border-radius: 3px;
      }

      .tf-btn:hover {
        color: #c0c0d0;
        background: #1a1a3e;
      }

      .tf-btn.active {
        color: #00d4aa;
        background: #1a1a3e;
      }

      .chart-area {
        flex: 1;
        min-height: 0;
      }

      .empty-state {
        display: flex;
        align-items: center;
        justify-content: center;
        height: 100%;
        color: #4a4a6e;
        font-size: 16px;
      }
    `,
  ],
})
export class TradingComponent implements OnInit, OnDestroy {
  private api = inject(ApiService);
  private cdr = inject(ChangeDetectorRef);
  private destroyRef = inject(DestroyRef);

  searchQuery = '';
  searchResults: InstrumentDto[] = [];
  selectedInstrument: InstrumentDto | null = null;
  selectedInterval = '1m';
  candles: ChartCandle[] = [];

  timeframes = [
    { label: '1m', value: '1m' },
    { label: '5m', value: '5m' },
    { label: '15m', value: '15m' },
    { label: '1H', value: '1h' },
    { label: '4H', value: '4h' },
    { label: '1D', value: '1d' },
  ];

  private searchTimeout: ReturnType<typeof setTimeout> | null = null;

  ngOnInit(): void {
    this.api
      .searchInstruments('BTC/USD')
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: results => {
          const btcUsd = results.find(i => i.symbol === 'BTC/USD' && i.exchange === 'Kraken');
          if (btcUsd) {
            this.selectInstrument(btcUsd);
          }
          this.cdr.markForCheck();
        },
        error: err => console.error('Failed to load default instrument', err),
      });
  }

  ngOnDestroy(): void {
    if (this.searchTimeout) clearTimeout(this.searchTimeout);
  }

  onSearch(): void {
    if (this.searchTimeout) clearTimeout(this.searchTimeout);

    if (this.searchQuery.length < 1) {
      this.searchResults = [];
      this.cdr.markForCheck();
      return;
    }

    this.searchTimeout = setTimeout(() => {
      this.api
        .searchInstruments(this.searchQuery)
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe({
          next: results => {
            this.searchResults = results;
            this.cdr.markForCheck();
          },
          error: err => console.error('Search failed', err),
        });
    }, 300);
  }

  selectInstrument(instrument: InstrumentDto): void {
    this.selectedInstrument = instrument;
    this.searchQuery = '';
    this.searchResults = [];
    this.cdr.markForCheck();
    this.loadCandles();
  }

  selectInterval(interval: string): void {
    this.selectedInterval = interval;
    this.cdr.markForCheck();
    this.loadCandles();
  }

  private loadCandles(): void {
    if (!this.selectedInstrument) return;

    const to = new Date().toISOString();
    const from = this.getFromDate().toISOString();

    this.api
      .getCandles(this.selectedInstrument.id, this.selectedInterval, from, to)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: candles => {
          this.candles = candles.map(c => ({
            time: c.timestamp,
            open: c.open,
            high: c.high,
            low: c.low,
            close: c.close,
            volume: c.volume,
          }));
          this.cdr.markForCheck();
        },
        error: err => console.error('Failed to load candles', err),
      });
  }

  private getFromDate(): Date {
    const now = new Date();
    const intervalHours: Record<string, number> = {
      '1m': 6,
      '5m': 24,
      '15m': 72,
      '1h': 168,
      '4h': 720,
      '1d': 4320,
    };
    const hours = intervalHours[this.selectedInterval] ?? 168;
    return new Date(now.getTime() - hours * 60 * 60 * 1000);
  }
}
