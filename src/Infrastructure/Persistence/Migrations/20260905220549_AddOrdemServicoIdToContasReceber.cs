using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace connectasys_api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrdemServicoIdToContasReceber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ordem_servico_id",
                table: "contas_receber",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_contas_receber_ordem_servico_id",
                table: "contas_receber",
                column: "ordem_servico_id");

            migrationBuilder.AddForeignKey(
                name: "FK_contas_receber_ordens_servico_ordem_servico_id",
                table: "contas_receber",
                column: "ordem_servico_id",
                principalTable: "ordens_servico",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_contas_receber_ordens_servico_ordem_servico_id",
                table: "contas_receber");

            migrationBuilder.DropIndex(
                name: "IX_contas_receber_ordem_servico_id",
                table: "contas_receber");

            migrationBuilder.DropColumn(
                name: "ordem_servico_id",
                table: "contas_receber");
        }
    }
}
