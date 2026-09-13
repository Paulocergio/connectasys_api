using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace connectasys_api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEstoqueMinimoDeEstoque : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "estoque_minimo",
                table: "estoque");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "estoque_minimo",
                table: "estoque",
                type: "numeric(10,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
