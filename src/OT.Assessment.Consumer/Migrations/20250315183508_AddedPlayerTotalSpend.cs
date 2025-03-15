using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OT.Assessment.Consumer.Migrations
{
    /// <inheritdoc />
    public partial class AddedPlayerTotalSpend : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalSpend",
                table: "Players",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            // Manually define the trigger 
            migrationBuilder.Sql(@"
        CREATE TRIGGER trg_UpdateTotalSpend
        ON CasinoWagers
        AFTER INSERT
        AS
        BEGIN
            SET NOCOUNT ON;
            UPDATE Players
            SET TotalSpend = TotalSpend + inserted.Amount
            FROM Players
            INNER JOIN inserted ON Players.AccountId = inserted.AccountId;
        END;
    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // manually define the removal of the trigger
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_UpdateTotalSpend;");

            migrationBuilder.DropColumn(
                name: "TotalSpend",
                table: "Players");
        }
    }
}
