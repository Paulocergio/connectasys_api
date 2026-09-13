using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace connectasys_api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiTenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_clientes_cnpj",
                table: "clientes");

            migrationBuilder.DropIndex(
                name: "IX_clientes_cpf",
                table: "clientes");

            migrationBuilder.AddColumn<Guid>(
                name: "empresa_id",
                table: "veiculos",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "empresa_id",
                table: "usuarios",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "empresa_id",
                table: "ordens_servico",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "empresa_id",
                table: "itens_ordem_servico",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "empresa_id",
                table: "estoque",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "empresa_id",
                table: "contas_receber",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "empresa_id",
                table: "contas_pagar",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "empresa_id",
                table: "clientes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "empresa_id",
                table: "agendamentos",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "empresas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    trial_expira_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_empresas", x => x.id);
                });

            // Empresa "legada" — todo dado que já existia antes do
            // multitenant (empresa_id = 00000000-0000-0000-0000-000000000000,
            // o default usado nos AddColumn acima) passa a pertencer a ela.
            // Trial bem no futuro pra ninguém que já usava o sistema ficar
            // bloqueado pela regra de teste de 3 dias (essa regra vale só
            // pra empresa nova, criada via cadastro self-service).
            migrationBuilder.Sql(@"
                INSERT INTO empresas (id, nome, data_cadastro, trial_expira_em)
                VALUES (
                    '00000000-0000-0000-0000-000000000000',
                    'ConnectaSys (dados anteriores ao multitenant)',
                    now(),
                    now() + interval '100 years'
                );
            ");

            migrationBuilder.CreateIndex(
                name: "IX_veiculos_empresa_id",
                table: "veiculos",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_empresa_id",
                table: "usuarios",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "IX_ordens_servico_empresa_id",
                table: "ordens_servico",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "IX_itens_ordem_servico_empresa_id",
                table: "itens_ordem_servico",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "IX_estoque_empresa_id",
                table: "estoque",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "IX_contas_receber_empresa_id",
                table: "contas_receber",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "IX_contas_pagar_empresa_id",
                table: "contas_pagar",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "IX_clientes_empresa_id_cnpj",
                table: "clientes",
                columns: new[] { "empresa_id", "cnpj" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_clientes_empresa_id_cpf",
                table: "clientes",
                columns: new[] { "empresa_id", "cpf" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_agendamentos_empresa_id",
                table: "agendamentos",
                column: "empresa_id");

            migrationBuilder.AddForeignKey(
                name: "FK_agendamentos_empresas_empresa_id",
                table: "agendamentos",
                column: "empresa_id",
                principalTable: "empresas",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_clientes_empresas_empresa_id",
                table: "clientes",
                column: "empresa_id",
                principalTable: "empresas",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_contas_pagar_empresas_empresa_id",
                table: "contas_pagar",
                column: "empresa_id",
                principalTable: "empresas",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_contas_receber_empresas_empresa_id",
                table: "contas_receber",
                column: "empresa_id",
                principalTable: "empresas",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_estoque_empresas_empresa_id",
                table: "estoque",
                column: "empresa_id",
                principalTable: "empresas",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_itens_ordem_servico_empresas_empresa_id",
                table: "itens_ordem_servico",
                column: "empresa_id",
                principalTable: "empresas",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ordens_servico_empresas_empresa_id",
                table: "ordens_servico",
                column: "empresa_id",
                principalTable: "empresas",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_usuarios_empresas_empresa_id",
                table: "usuarios",
                column: "empresa_id",
                principalTable: "empresas",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_veiculos_empresas_empresa_id",
                table: "veiculos",
                column: "empresa_id",
                principalTable: "empresas",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_agendamentos_empresas_empresa_id",
                table: "agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_clientes_empresas_empresa_id",
                table: "clientes");

            migrationBuilder.DropForeignKey(
                name: "FK_contas_pagar_empresas_empresa_id",
                table: "contas_pagar");

            migrationBuilder.DropForeignKey(
                name: "FK_contas_receber_empresas_empresa_id",
                table: "contas_receber");

            migrationBuilder.DropForeignKey(
                name: "FK_estoque_empresas_empresa_id",
                table: "estoque");

            migrationBuilder.DropForeignKey(
                name: "FK_itens_ordem_servico_empresas_empresa_id",
                table: "itens_ordem_servico");

            migrationBuilder.DropForeignKey(
                name: "FK_ordens_servico_empresas_empresa_id",
                table: "ordens_servico");

            migrationBuilder.DropForeignKey(
                name: "FK_usuarios_empresas_empresa_id",
                table: "usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_veiculos_empresas_empresa_id",
                table: "veiculos");

            migrationBuilder.DropTable(
                name: "empresas");

            migrationBuilder.DropIndex(
                name: "IX_veiculos_empresa_id",
                table: "veiculos");

            migrationBuilder.DropIndex(
                name: "IX_usuarios_empresa_id",
                table: "usuarios");

            migrationBuilder.DropIndex(
                name: "IX_ordens_servico_empresa_id",
                table: "ordens_servico");

            migrationBuilder.DropIndex(
                name: "IX_itens_ordem_servico_empresa_id",
                table: "itens_ordem_servico");

            migrationBuilder.DropIndex(
                name: "IX_estoque_empresa_id",
                table: "estoque");

            migrationBuilder.DropIndex(
                name: "IX_contas_receber_empresa_id",
                table: "contas_receber");

            migrationBuilder.DropIndex(
                name: "IX_contas_pagar_empresa_id",
                table: "contas_pagar");

            migrationBuilder.DropIndex(
                name: "IX_clientes_empresa_id_cnpj",
                table: "clientes");

            migrationBuilder.DropIndex(
                name: "IX_clientes_empresa_id_cpf",
                table: "clientes");

            migrationBuilder.DropIndex(
                name: "IX_agendamentos_empresa_id",
                table: "agendamentos");

            migrationBuilder.DropColumn(
                name: "empresa_id",
                table: "veiculos");

            migrationBuilder.DropColumn(
                name: "empresa_id",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "empresa_id",
                table: "ordens_servico");

            migrationBuilder.DropColumn(
                name: "empresa_id",
                table: "itens_ordem_servico");

            migrationBuilder.DropColumn(
                name: "empresa_id",
                table: "estoque");

            migrationBuilder.DropColumn(
                name: "empresa_id",
                table: "contas_receber");

            migrationBuilder.DropColumn(
                name: "empresa_id",
                table: "contas_pagar");

            migrationBuilder.DropColumn(
                name: "empresa_id",
                table: "clientes");

            migrationBuilder.DropColumn(
                name: "empresa_id",
                table: "agendamentos");

            migrationBuilder.CreateIndex(
                name: "IX_clientes_cnpj",
                table: "clientes",
                column: "cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_clientes_cpf",
                table: "clientes",
                column: "cpf",
                unique: true);
        }
    }
}
