export interface Instrument {
  id: string;
  symbol: string;
  name: string;
  assetType: string;
  userId: string;
  createdAt: string;
}

export interface CreateInstrumentRequest {
  symbol: string;
  name: string;
  assetType: string;
}
