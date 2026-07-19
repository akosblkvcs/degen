using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Degen.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCandles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "candles",
                columns: table => new
                {
                    instrument_id = table.Column<Guid>(type: "uuid", nullable: false),
                    interval = table.Column<string>(
                        type: "character varying(8)",
                        maxLength: 8,
                        nullable: false
                    ),
                    ts = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    open = table.Column<decimal>(
                        type: "numeric(18,6)",
                        precision: 18,
                        scale: 6,
                        nullable: false
                    ),
                    high = table.Column<decimal>(
                        type: "numeric(18,6)",
                        precision: 18,
                        scale: 6,
                        nullable: false
                    ),
                    low = table.Column<decimal>(
                        type: "numeric(18,6)",
                        precision: 18,
                        scale: 6,
                        nullable: false
                    ),
                    close = table.Column<decimal>(
                        type: "numeric(18,6)",
                        precision: 18,
                        scale: 6,
                        nullable: false
                    ),
                    volume = table.Column<long>(type: "bigint", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "pk_candles",
                        x => new
                        {
                            x.instrument_id,
                            x.interval,
                            x.ts,
                        }
                    );
                    table.ForeignKey(
                        name: "fk_candles_instruments_instrument_id",
                        column: x => x.instrument_id,
                        principalTable: "instruments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "candles");
        }
    }
}
