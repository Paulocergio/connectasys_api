using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace connectasys_api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAgendamentos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "agendamentos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tecnico_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cliente_id = table.Column<int>(type: "integer", nullable: true),
                    veiculo_id = table.Column<int>(type: "integer", nullable: true),
                    ordem_servico_id = table.Column<int>(type: "integer", nullable: true),
                    data_hora_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_hora_fim = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    observacao = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_agendamentos", x => x.id);
                    table.ForeignKey(
                        name: "FK_agendamentos_clientes_cliente_id",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_agendamentos_ordens_servico_ordem_servico_id",
                        column: x => x.ordem_servico_id,
                        principalTable: "ordens_servico",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_agendamentos_usuarios_tecnico_id",
                        column: x => x.tecnico_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_agendamentos_veiculos_veiculo_id",
                        column: x => x.veiculo_id,
                        principalTable: "veiculos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_agendamentos_cliente_id",
                table: "agendamentos",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "IX_agendamentos_ordem_servico_id",
                table: "agendamentos",
                column: "ordem_servico_id");

            migrationBuilder.CreateIndex(
                name: "IX_agendamentos_tecnico_id_data_hora_inicio",
                table: "agendamentos",
                columns: new[] { "tecnico_id", "data_hora_inicio" });

            migrationBuilder.CreateIndex(
                name: "IX_agendamentos_veiculo_id",
                table: "agendamentos",
                column: "veiculo_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "agendamentos");
        }
    }
}
