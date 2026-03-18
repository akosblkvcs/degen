import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface AssetDto {
  id: string;
  symbol: string;
  name: string;
  type: string;
  decimals: number;
}

export interface InstrumentDto {
  id: string;
  symbol: string;
  exchangeSymbol: string;
  exchange: string;
  baseAsset: AssetDto;
  quoteAsset: AssetDto;
  priceDecimals: number;
  quantityDecimals: number;
  minOrderSize: number | null;
  isActive: boolean;
}

export interface CandleDto {
  timestamp: string;
  open: number;
  high: number;
  low: number;
  close: number;
  volume: number;
}

export interface WatchlistDto {
  id: string;
  name: string;
  items: WatchlistItemDto[];
}

export interface WatchlistItemDto {
  instrumentId: string;
  symbol: string;
  exchange: string;
  sortOrder: number;
}

@Injectable({ providedIn: 'root' })
export class ApiService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;

  // Instruments
  searchInstruments(query: string): Observable<InstrumentDto[]> {
    return this.http.get<InstrumentDto[]>(`${this.baseUrl}/instruments/search`, {
      params: new HttpParams().set('q', query),
    });
  }

  getInstrument(id: string): Observable<InstrumentDto> {
    return this.http.get<InstrumentDto>(`${this.baseUrl}/instruments/${id}`);
  }

  getInstrumentsByExchange(exchange: string): Observable<InstrumentDto[]> {
    return this.http.get<InstrumentDto[]>(`${this.baseUrl}/instruments/exchange/${exchange}`);
  }

  // Candles
  getCandles(
    instrumentId: string,
    interval: string,
    from: string,
    to: string,
  ): Observable<CandleDto[]> {
    const params = new HttpParams().set('interval', interval).set('from', from).set('to', to);

    return this.http.get<CandleDto[]>(`${this.baseUrl}/instruments/${instrumentId}/candles`, {
      params,
    });
  }

  // Watchlists
  getWatchlists(): Observable<WatchlistDto[]> {
    return this.http.get<WatchlistDto[]>(`${this.baseUrl}/watchlists`);
  }

  createWatchlist(name: string): Observable<WatchlistDto> {
    return this.http.post<WatchlistDto>(`${this.baseUrl}/watchlists`, { name });
  }

  addToWatchlist(watchlistId: string, instrumentId: string): Observable<WatchlistDto> {
    return this.http.post<WatchlistDto>(
      `${this.baseUrl}/watchlists/${watchlistId}/instruments/${instrumentId}`,
      {},
    );
  }

  removeFromWatchlist(watchlistId: string, instrumentId: string): Observable<void> {
    return this.http.delete<void>(
      `${this.baseUrl}/watchlists/${watchlistId}/instruments/${instrumentId}`,
    );
  }
}
