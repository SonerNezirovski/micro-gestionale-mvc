using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroGestionale.Migrations
{
    /// <inheritdoc />
    public partial class CreazioneIniziale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clienti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    PartitaIva = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    CodiceFiscale = table.Column<string>(type: "TEXT", maxLength: 16, nullable: true),
                    Indirizzo = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Cap = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    Citta = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Provincia = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Nazione = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    Telefono = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    Bloccato = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clienti", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Preventivi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Numero = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Data = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ValidoFinoAl = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ClienteId = table.Column<int>(type: "INTEGER", nullable: false),
                    Note = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Preventivi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Preventivi_Clienti_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clienti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RighePreventivo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PreventivoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Posizione = table.Column<int>(type: "INTEGER", nullable: false),
                    Descrizione = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Quantita = table.Column<decimal>(type: "TEXT", nullable: false),
                    PrezzoUnitario = table.Column<decimal>(type: "TEXT", nullable: false),
                    ScontoPercentuale = table.Column<decimal>(type: "TEXT", nullable: false),
                    AliquotaIva = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RighePreventivo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RighePreventivo_Preventivi_PreventivoId",
                        column: x => x.PreventivoId,
                        principalTable: "Preventivi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Preventivi_ClienteId",
                table: "Preventivi",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_RighePreventivo_PreventivoId",
                table: "RighePreventivo",
                column: "PreventivoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RighePreventivo");

            migrationBuilder.DropTable(
                name: "Preventivi");

            migrationBuilder.DropTable(
                name: "Clienti");
        }
    }
}
