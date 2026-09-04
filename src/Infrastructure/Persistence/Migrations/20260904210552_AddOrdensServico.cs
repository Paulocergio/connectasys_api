using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace connectasys_api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrdensServico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ordens_servico",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cliente_id = table.Column<int>(type: "integer", nullable: false),
                    veiculo_id = table.Column<int>(type: "integer", nullable: false),
                    tecnico_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    descricao_problema = table.Column<string>(type: "text", nullable: false),
                    diagnostico = table.Column<string>(type: "text", nullable: true),
                    solucao = table.Column<string>(type: "text", nullable: true),
                    data_abertura = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    previsao_termino = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    data_conclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    valor_mao_de_obra = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    desconto = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    aprovacao_cliente_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    aprovacao_cliente_nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ordens_servico", x => x.id);
                    table.ForeignKey(
                        name: "FK_ordens_servico_clientes_cliente_id",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ordens_servico_usuarios_tecnico_id",
                        column: x => x.tecnico_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ordens_servico_veiculos_veiculo_id",
                        column: x => x.veiculo_id,
                        principalTable: "veiculos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "itens_ordem_servico",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ordem_servico_id = table.Column<int>(type: "integer", nullable: false),
                    descricao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    quantidade = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    valor_unitario = table.Column<decimal>(type: "numeric(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_itens_ordem_servico", x => x.id);
                    table.ForeignKey(
                        name: "FK_itens_ordem_servico_ordens_servico_ordem_servico_id",
                        column: x => x.ordem_servico_id,
                        principalTable: "ordens_servico",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_itens_ordem_servico_ordem_servico_id",
                table: "itens_ordem_servico",
                column: "ordem_servico_id");

            migrationBuilder.CreateIndex(
                name: "IX_ordens_servico_cliente_id",
                table: "ordens_servico",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "IX_ordens_servico_tecnico_id",
                table: "ordens_servico",
                column: "tecnico_id");

            migrationBuilder.CreateIndex(
                name: "IX_ordens_servico_veiculo_id",
                table: "ordens_servico",
                column: "veiculo_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "itens_ordem_servico");

            migrationBuilder.DropTable(
                name: "ordens_servico");
        }
    }
}
