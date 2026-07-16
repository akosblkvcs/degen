# degen

A personal finance and investing tracker, built step by step as a learning project
(big-tech interview prep: scalability, clean code, common design patterns, AI integration).

Watch instruments (stocks, ETFs, crypto, forex), detect buy-the-dip signals,
track a multi-platform portfolio, and eventually add AI analysis and automated trading.

**Start here: [ROADMAP.md](ROADMAP.md)** — the project is rebuilt in small stages;
each stage lists the small steps to implement next.

## Stack

- **Backend:** .NET (ASP.NET Core Web API)
- **Frontend:** Angular + TradingView Lightweight Charts
- **Database:** PostgreSQL
- **AI service (later):** Python + Claude API
- **Deploy:** Docker containers on a VPS via Coolify

## History

The first, over-engineered attempt (clean architecture, CQRS, RabbitMQ, SignalR,
Kraken WebSocket worker) is preserved on the `archive/v1-first-attempt` branch.
It is reused as reference material in the later "grow the architecture" stage.
