using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VendePraMim.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NomeCompleto = table.Column<string>(type: "TEXT", nullable: false),
                    CPF = table.Column<string>(type: "TEXT", nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Cidade = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Tipo = table.Column<string>(type: "TEXT", nullable: true),
                    Discriminator = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    CNPJ = table.Column<string>(type: "TEXT", nullable: true),
                    ContaBancaria = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Eventos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Categoria = table.Column<string>(type: "TEXT", nullable: false),
                    Data = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Local = table.Column<string>(type: "TEXT", nullable: false),
                    OrganizadorId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Eventos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Eventos_Usuarios_OrganizadorId",
                        column: x => x.OrganizadorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Ingressos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EventoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Tipo = table.Column<string>(type: "TEXT", nullable: false),
                    Setor = table.Column<string>(type: "TEXT", nullable: false),
                    PrecoVenda = table.Column<decimal>(type: "TEXT", nullable: false),
                    PrecoMinimo = table.Column<decimal>(type: "TEXT", nullable: true),
                    PrecoMaximo = table.Column<decimal>(type: "TEXT", nullable: true),
                    VendedorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Vendido = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingressos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ingressos_Eventos_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ingressos_Usuarios_VendedorId",
                        column: x => x.VendedorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OfertasCompra",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EventoId = table.Column<int>(type: "INTEGER", nullable: false),
                    TipoIngresso = table.Column<string>(type: "TEXT", nullable: false),
                    PrecoMaximo = table.Column<decimal>(type: "TEXT", nullable: false),
                    CompradorId = table.Column<int>(type: "INTEGER", nullable: false),
                    DataOferta = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Ativa = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfertasCompra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfertasCompra_Eventos_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OfertasCompra_Usuarios_CompradorId",
                        column: x => x.CompradorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IngressoId = table.Column<int>(type: "INTEGER", nullable: false),
                    CompradorId = table.Column<int>(type: "INTEGER", nullable: false),
                    DataCompra = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ValorPago = table.Column<decimal>(type: "TEXT", nullable: false),
                    Validado = table.Column<bool>(type: "INTEGER", nullable: false),
                    TaxaPlataforma = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transacoes_Ingressos_IngressoId",
                        column: x => x.IngressoId,
                        principalTable: "Ingressos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transacoes_Usuarios_CompradorId",
                        column: x => x.CompradorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_OrganizadorId",
                table: "Eventos",
                column: "OrganizadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingressos_EventoId",
                table: "Ingressos",
                column: "EventoId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingressos_VendedorId",
                table: "Ingressos",
                column: "VendedorId");

            migrationBuilder.CreateIndex(
                name: "IX_OfertasCompra_CompradorId",
                table: "OfertasCompra",
                column: "CompradorId");

            migrationBuilder.CreateIndex(
                name: "IX_OfertasCompra_EventoId",
                table: "OfertasCompra",
                column: "EventoId");

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_CompradorId",
                table: "Transacoes",
                column: "CompradorId");

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_IngressoId",
                table: "Transacoes",
                column: "IngressoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OfertasCompra");

            migrationBuilder.DropTable(
                name: "Transacoes");

            migrationBuilder.DropTable(
                name: "Ingressos");

            migrationBuilder.DropTable(
                name: "Eventos");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
