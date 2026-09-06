using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace connectasys_api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEstoque : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "estoque_id",
                table: "itens_ordem_servico",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "estoque",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    quantidade = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    preco_compra = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    preco_venda = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    estoque_minimo = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estoque", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_itens_ordem_servico_estoque_id",
                table: "itens_ordem_servico",
                column: "estoque_id");

            migrationBuilder.AddForeignKey(
                name: "FK_itens_ordem_servico_estoque_estoque_id",
                table: "itens_ordem_servico",
                column: "estoque_id",
                principalTable: "estoque",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_itens_ordem_servico_estoque_estoque_id",
                table: "itens_ordem_servico");

            migrationBuilder.DropTable(
                name: "estoque");

            migrationBuilder.DropIndex(
                name: "IX_itens_ordem_servico_estoque_id",
                table: "itens_ordem_servico");

            migrationBuilder.DropColumn(
                name: "estoque_id",
                table: "itens_ordem_servico");
        }
    }
}
