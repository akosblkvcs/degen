# Roadmap

Rules of the game:

- One stage at a time. Steps inside a stage are small enough for one sitting.
- Every stage ends with something **working and deployed** — no half-built layers.
- Start simple, add "corporate" patterns only when a stage actually needs them
  (that *is* the interview lesson: knowing when a pattern earns its complexity).
- Model everything with a `UserId` from day one (hardcoded `"default"` user for now),
  so multi-user later is a data change, not a rewrite.

Data sources (free):

- **Crypto:** Kraken public REST API (no key needed)
- **Stocks / ETFs / indices:** Stooq CSV (no key) or Twelve Data / Alpha Vantage free tier
- **Forex:** frankfurter.app (no key)
- **Options / futures:** no good free data exists — deferred until a paid need is proven

---

## Stage 0 — Clean slate ✅

- [x] Archive the first attempt on `archive/v1-first-attempt`
- [x] Reset `master` to README + ROADMAP + .gitignore

## Stage 1 — Walking skeleton, deployed

Goal: the thinnest possible slice of every layer, live on the VPS.

- [ ] 1.1 New solution: **one** ASP.NET Core Web API project (`apps/api`), `/health` endpoint
- [ ] 1.2 `docker-compose.yml` with Postgres; connection string via `.env`
- [ ] 1.3 EF Core with one table (`instruments`), one migration, applied on startup
- [ ] 1.4 One GET + one POST endpoint for instruments
- [ ] 1.5 New Angular app (`apps/web`): one page that lists and adds instruments
- [ ] 1.6 Dockerfiles for api and web (nginx)
- [ ] 1.7 Deploy both containers + Postgres to Coolify — skeleton is live

## Stage 2 — Watchlist & price data

Goal: instruments you care about, with real daily prices stored locally.

- [ ] 2.1 Instrument gets asset type (stock/etf/crypto/forex/index) and data source
- [ ] 2.2 Price fetcher as an `IHostedService` **inside the API** (no separate worker yet):
      pulls daily OHLC candles per instrument on a schedule
- [ ] 2.3 Stooq adapter (stocks/ETFs/indices) + Kraken adapter (crypto), behind one
      `IPriceProvider` interface (strategy pattern, `HttpClientFactory`)
- [ ] 2.4 `candles` table with upsert; backfill ~2 years of history on instrument add
- [ ] 2.5 Watchlist UI: add/remove tickers, show last price and daily change

## Stage 3 — Charts

Goal: the TradingView experience for your own data.

- [ ] 3.1 `GET /instruments/{id}/candles?range=...` endpoint
- [ ] 3.2 Candlestick component with **Lightweight Charts**
- [ ] 3.3 Range switcher (1M / 6M / 1Y / all) + SMA(50/200) overlay toggle

## Stage 4 — Drop detector (your first strategy)

Goal: "S&P 500 dropped 1% this week → I go buy manually."

- [ ] 4.1 Metrics per instrument computed from candles: change over 1d / 5d / 20d,
      and % below 52-week high
- [ ] 4.2 `alert_rules` table: instrument + metric + threshold
      (e.g. `SPY, change_5d, ≤ -1%`)
- [ ] 4.3 Evaluate rules after each fetch; write triggered `signals` (no duplicates
      while a signal is still active)
- [ ] 4.4 Signals page + badge in the UI
- [ ] 4.5 Telegram bot notification (free, ~20 lines of code)
- [ ] 4.6 First real unit tests: metric math and rule evaluation
- More simple strategies to add as rules later: % below 52w high ("buy the dip"),
  price crossing below SMA200, RSI(14) < 30 (oversold), fixed DCA-day reminder

## Stage 5 — Portfolio tracking

Goal: all positions from all platforms in one place, entered manually.

- [ ] 5.1 `accounts` (platform: Kraken, IBKR, …) and `transactions`
      (buy/sell/deposit/fee) tables + entry forms
- [ ] 5.2 Positions derived from transactions: quantity, average cost basis
- [ ] 5.3 Unrealized P/L using stored candle prices; `decimal` everywhere, currency-aware
- [ ] 5.4 Allocation view: by asset class and by platform (donut chart), portfolio total

## Stage 6 — Grow the architecture (the interview-prep stage)

Goal: scale patterns applied to a *real* working product. The
`archive/v1-first-attempt` branch is your reference here.

- [ ] 6.1 Extract the price fetcher into a separate worker container
- [ ] 6.2 Worker → API communication: start with the transactional outbox pattern in
      Postgres; upgrade to RabbitMQ and discuss the trade-off
- [ ] 6.3 SignalR: live price/signal updates pushed to the UI
- [ ] 6.4 Refactor the API toward clean architecture + CQRS where the code actually
      hurts — compare with the archive branch's version
- [ ] 6.5 GitHub Actions CI: build, test, publish images; Coolify auto-deploy on push
- [ ] 6.6 Integration tests with Testcontainers; health checks; structured logging
- [ ] 6.7 (Optional) Real multi-user: authentication, per-user data isolation

## Stage 7 — AI analysis

Goal: the market explained to you daily.

- [ ] 7.1 New Python FastAPI service container (`apps/ai`) calling the Claude API
- [ ] 7.2 Daily brief: summarize watchlist moves + active signals into a short report
- [ ] 7.3 "Explain this signal" button: candles + metrics in, plain-language analysis out
- [ ] 7.4 Persist analyses; show history in the UI

## Stage 8 — Trading automation

Goal: orders placed through APIs, safely. Paper trading first, always.

- [ ] 8.1 Alpaca paper-trading account; order service behind an `IBroker` interface
- [ ] 8.2 One-click: a signal shows a "place order" button, human confirms every order
- [ ] 8.3 Rule-driven auto orders with hard safeguards: max order size, daily budget,
      kill switch, dry-run mode
- [ ] 8.4 Kraken as a second `IBroker` implementation

## Stage 9 — AI-assisted trading (the very last step)

- [ ] 9.1 AI proposes trades from signals; rules constrain size/frequency; human approves
- [ ] 9.2 Only after a long paper-trading track record: tiny real budget, full audit log
