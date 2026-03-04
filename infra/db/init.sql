-- TimescaleDB extension must exist before EF Core migrations run.
-- Everything else (schemas, tables, hypertables) is managed by EF Core.
CREATE EXTENSION IF NOT EXISTS timescaledb;
