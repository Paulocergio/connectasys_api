using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace connectasys_api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTipoToVeiculos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "tipo",
                table: "veiculos",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Carro");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tipo",
                table: "veiculos");
        }
    }
}
