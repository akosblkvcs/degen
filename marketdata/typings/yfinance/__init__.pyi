# Minimal local stub for the slice of yfinance this service uses.
# yfinance ships untyped; this keeps Pylance/mypy strict without in-code suppressions.

from datetime import date

import pandas as pd

class FastInfo:
    last_price: float | None
    previous_close: float | None
    currency: str | None

class Ticker:
    def __init__(self, ticker: str) -> None: ...
    @property
    def fast_info(self) -> FastInfo: ...
    def history(
        self,
        period: str = ...,
        interval: str = ...,
        start: date | str | None = ...,
        end: date | str | None = ...,
        auto_adjust: bool = ...,
    ) -> pd.DataFrame: ...
