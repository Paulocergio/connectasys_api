using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace connectasys_api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSenhaHashToUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "senha_hash",
                table: "usuarios",
                type: "character varying(60)",
                maxLength: 60,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "senha_hash",
                table: "usuarios");
        }
    }
}
