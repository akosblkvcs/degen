export interface Instrument {
  id: string;
  symbol: string;
  name: string;
  assetType: string;
  createdAt: string;
  lastClose: number | null;
  changePercent: number | null;
}

export interface Quote {
  symbol: string;
  price: number;
  previousClose: number | null;
  currency: string | null;
}

export interface CreateInstrumentRequest {
  symbol: string;
  name: string;
  assetType: string;
}
