import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateInstrumentRequest, Instrument } from './instrument.model';

@Injectable({ providedIn: 'root' })
export class InstrumentsService {
  private readonly http = inject(HttpClient);

  list(): Observable<Instrument[]> {
    return this.http.get<Instrument[]>('/api/instruments');
  }

  create(request: CreateInstrumentRequest): Observable<Instrument> {
    return this.http.post<Instrument>('/api/instruments', request);
  }
}
