using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace connectasys_api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFormaPagamentoContasPagarEContasReceber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "forma_pagamento",
                table: "contas_receber",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "forma_pagamento",
                table: "contas_pagar",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "forma_pagamento",
                table: "contas_receber");

            migrationBuilder.DropColumn(
                name: "forma_pagamento",
                table: "contas_pagar");
        }
    }
}
