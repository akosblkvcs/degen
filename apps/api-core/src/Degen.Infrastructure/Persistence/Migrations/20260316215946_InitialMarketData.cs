using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Degen.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialMarketData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE SCHEMA IF NOT EXISTS market_data;");

            migrationBuilder.EnsureSchema(name: "market_data");

            migrationBuilder.CreateTable(
                name: "assets",
                schema: "market_data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    symbol = table.Column<string>(
                        type: "character varying(20)",
                        maxLength: 20,
                        nullable: false
                    ),
                    name = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    type = table.Column<string>(
                        type: "character varying(20)",
                        maxLength: 20,
                        nullable: false
                    ),
                    decimals = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_assets", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "candles",
                schema: "market_data",
                columns: table => new
                {
                    instrument_id = table.Column<Guid>(type: "uuid", nullable: false),
                    interval = table.Column<string>(
                        type: "character varying(5)",
                        maxLength: 5,
                        nullable: false
                    ),
                    timestamp = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    open = table.Column<decimal>(
                        type: "numeric(18,8)",
                        precision: 18,
                        scale: 8,
                        nullable: false
                    ),
                    high = table.Column<decimal>(
                        type: "numeric(18,8)",
                        precision: 18,
                        scale: 8,
                        nullable: false
                    ),
                    low = table.Column<decimal>(
                        type: "numeric(18,8)",
                        precision: 18,
                        scale: 8,
                        nullable: false
                    ),
                    close = table.Column<decimal>(
                        type: "numeric(18,8)",
                        precision: 18,
                        scale: 8,
                        nullable: false
                    ),
                    volume = table.Column<decimal>(
                        type: "numeric(18,8)",
                        precision: 18,
                        scale: 8,
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "pk_candles",
                        x => new
                        {
                            x.instrument_id,
                            x.interval,
                            x.timestamp,
                        }
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "watchlists",
                schema: "market_data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_watchlists", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "instruments",
                schema: "market_data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    symbol = table.Column<string>(
                        type: "character varying(30)",
                        maxLength: 30,
                        nullable: false
                    ),
                    exchange_symbol = table.Column<string>(
                        type: "character varying(30)",
                        maxLength: 30,
                        nullable: false
                    ),
                    exchange = table.Column<string>(
                        type: "character varying(30)",
                        maxLength: 30,
                        nullable: false
                    ),
                    base_asset_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quote_asset_id = table.Column<Guid>(type: "uuid", nullable: false),
                    price_decimals = table.Column<int>(type: "integer", nullable: false),
                    quantity_decimals = table.Column<int>(type: "integer", nullable: false),
                    min_order_size = table.Column<decimal>(
                        type: "numeric(18,8)",
                        precision: 18,
                        scale: 8,
                        nullable: true
                    ),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_instruments", x => x.id);
                    table.ForeignKey(
                        name: "fk_instruments_assets_base_asset_id",
                        column: x => x.base_asset_id,
                        principalSchema: "market_data",
                        principalTable: "assets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "fk_instruments_assets_quote_asset_id",
                        column: x => x.quote_asset_id,
                        principalSchema: "market_data",
                        principalTable: "assets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "watchlist_items",
                schema: "market_data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    watchlist_id = table.Column<Guid>(type: "uuid", nullable: false),
                    instrument_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_watchlist_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_watchlist_items_instruments_instrument_id",
                        column: x => x.instrument_id,
                        principalSchema: "market_data",
                        principalTable: "instruments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "fk_watchlist_items_watchlists_watchlist_id",
                        column: x => x.watchlist_id,
                        principalSchema: "market_data",
                        principalTable: "watchlists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "ix_assets_symbol",
                schema: "market_data",
                table: "assets",
                column: "symbol",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_candles_instrument_id_interval_timestamp",
                schema: "market_data",
                table: "candles",
                columns: new[] { "instrument_id", "interval", "timestamp" }
            );

            migrationBuilder.CreateIndex(
                name: "ix_instruments_base_asset_id",
                schema: "market_data",
                table: "instruments",
                column: "base_asset_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_instruments_exchange_symbol_exchange",
                schema: "market_data",
                table: "instruments",
                columns: new[] { "exchange_symbol", "exchange" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_instruments_quote_asset_id",
                schema: "market_data",
                table: "instruments",
                column: "quote_asset_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_instruments_symbol_exchange",
                schema: "market_data",
                table: "instruments",
                columns: new[] { "symbol", "exchange" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "ix_watchlist_items_instrument_id",
                schema: "market_data",
                table: "watchlist_items",
                column: "instrument_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_watchlist_items_watchlist_id",
                schema: "market_data",
                table: "watchlist_items",
                column: "watchlist_id"
            );

            migrationBuilder.Sql(
                @"
                SELECT create_hypertable(
                    'market_data.candles',
                    by_range('timestamp')
                );
            "
            );

            migrationBuilder.Sql(
                @"
                SELECT set_chunk_time_interval('market_data.candles', INTERVAL '1 day');
            "
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "candles", schema: "market_data");

            migrationBuilder.DropTable(name: "watchlist_items", schema: "market_data");

            migrationBuilder.DropTable(name: "instruments", schema: "market_data");

            migrationBuilder.DropTable(name: "watchlists", schema: "market_data");

            migrationBuilder.DropTable(name: "assets", schema: "market_data");

            migrationBuilder.Sql("DROP SCHEMA IF EXISTS market_data CASCADE;");
        }
    }
}
