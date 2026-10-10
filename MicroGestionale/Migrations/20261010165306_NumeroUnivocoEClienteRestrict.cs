using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroGestionale.Migrations
{
    /// <inheritdoc />
    public partial class NumeroUnivocoEClienteRestrict : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Preventivi_Clienti_ClienteId",
                table: "Preventivi");

            migrationBuilder.CreateIndex(
                name: "IX_Preventivi_Numero",
                table: "Preventivi",
                column: "Numero",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Preventivi_Clienti_ClienteId",
                table: "Preventivi",
                column: "ClienteId",
                principalTable: "Clienti",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Preventivi_Clienti_ClienteId",
                table: "Preventivi");

            migrationBuilder.DropIndex(
                name: "IX_Preventivi_Numero",
                table: "Preventivi");

            migrationBuilder.AddForeignKey(
                name: "FK_Preventivi_Clienti_ClienteId",
                table: "Preventivi",
                column: "ClienteId",
                principalTable: "Clienti",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
