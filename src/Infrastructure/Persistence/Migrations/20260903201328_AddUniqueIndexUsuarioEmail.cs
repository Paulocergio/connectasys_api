using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace connectasys_api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexUsuarioEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_usuarios_email",
                table: "usuarios",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_usuarios_email",
                table: "usuarios");
        }
    }
}
