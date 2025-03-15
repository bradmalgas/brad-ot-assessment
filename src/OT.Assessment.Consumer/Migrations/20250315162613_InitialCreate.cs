using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OT.Assessment.Consumer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.AccountId);
                });

            migrationBuilder.CreateTable(
                name: "CasinoWagers",
                columns: table => new
                {
                    WagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WagerDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CasinoWagers", x => x.WagerId);
                    table.ForeignKey(
                        name: "FK_CasinoWagers_Players_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Players",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CasinoWagers_AccountId",
                table: "CasinoWagers",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CasinoWagers_AccountId_WagerDateTime",
                table: "CasinoWagers",
                columns: new[] { "AccountId", "WagerDateTime" });

            migrationBuilder.CreateIndex(
                name: "IX_CasinoWagers_Amount",
                table: "CasinoWagers",
                column: "Amount");

            migrationBuilder.CreateIndex(
                name: "IX_CasinoWagers_WagerDateTime",
                table: "CasinoWagers",
                column: "WagerDateTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CasinoWagers");

            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
