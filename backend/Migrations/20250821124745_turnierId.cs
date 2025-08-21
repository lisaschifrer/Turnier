using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class turnierId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TurnierId",
                table: "Groups",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Groups_TurnierId",
                table: "Groups",
                column: "TurnierId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Turniere_TurnierId",
                table: "Groups",
                column: "TurnierId",
                principalTable: "Turniere",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Turniere_TurnierId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_TurnierId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "TurnierId",
                table: "Groups");
        }
    }
}
