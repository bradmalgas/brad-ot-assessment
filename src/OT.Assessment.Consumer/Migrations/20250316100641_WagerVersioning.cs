using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OT.Assessment.Consumer.Migrations
{
    /// <inheritdoc />
    public partial class WagerVersioning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "CasinoWagers",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "CasinoWagers");
        }
    }
}
