import math
from datetime import UTC, date, datetime
from typing import Annotated, cast

import pandas as pd
import yfinance as yf
from fastapi import FastAPI, HTTPException, Query
from pydantic import BaseModel

app = FastAPI(title="degen marketdata")

ALLOWED_INTERVALS = {"1d", "1wk", "1mo"}


class Health(BaseModel):
    status: str


class Candle(BaseModel):
    ts: datetime
    open: float
    high: float
    low: float
    close: float
    volume: int


class CandlesResponse(BaseModel):
    symbol: str
    interval: str
    candles: list[Candle]


class QuoteResponse(BaseModel):
    symbol: str
    price: float
    previousClose: float | None
    currency: str | None


@app.get("/healthz")
def health() -> Health:
    return Health(status="ok")


@app.get("/candles")
def candles(
    symbol: str,
    start: Annotated[date, Query(description="inclusive first day, ISO-8601")],
    interval: str = "1d",
) -> CandlesResponse:
    if interval not in ALLOWED_INTERVALS:
        raise HTTPException(422, f"interval must be one of {sorted(ALLOWED_INTERVALS)}")

    try:
        frame = yf.Ticker(symbol).history(start=start, interval=interval, auto_adjust=True)
    except Exception as exc:  # yfinance raises a zoo of exception types
        raise HTTPException(502, f"upstream error for '{symbol}': {exc}") from exc

    if frame.empty:
        raise HTTPException(404, f"no data for symbol '{symbol}'")

    ohlc = ["Open", "High", "Low", "Close"]
    frame = frame.replace([math.inf, -math.inf], pd.NA).dropna(subset=ohlc)

    if frame.empty:
        raise HTTPException(404, f"no usable data for symbol '{symbol}'")

    rows: list[Candle] = []
    for raw_ts, row in frame.iterrows():
        ts = cast(pd.Timestamp, raw_ts)
        volume = row["Volume"]
        rows.append(
            Candle(
                ts=ts.tz_convert(UTC) if ts.tzinfo else ts.tz_localize(UTC),
                open=float(row["Open"]),
                high=float(row["High"]),
                low=float(row["Low"]),
                close=float(row["Close"]),
                volume=int(volume) if pd.notna(volume) and math.isfinite(volume) else 0,
            )
        )
    return CandlesResponse(symbol=symbol.upper(), interval=interval, candles=rows)


@app.get("/quote")
def quote(symbol: str) -> QuoteResponse:
    try:
        info = yf.Ticker(symbol).fast_info
        price = info.last_price
        previous_close = info.previous_close
        currency = info.currency
    except KeyError as exc:
        raise HTTPException(404, f"no quote for symbol '{symbol}'") from exc
    except Exception as exc:
        raise HTTPException(502, f"upstream error for '{symbol}': {exc}") from exc

    if price is None or not math.isfinite(price):
        raise HTTPException(404, f"no quote for symbol '{symbol}'")

    return QuoteResponse(
        symbol=symbol.upper(),
        price=float(price),
        previousClose=(
            float(previous_close)
            if previous_close is not None and math.isfinite(previous_close)
            else None
        ),
        currency=currency,
    )
