using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPlacementBracketFinalMAtch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IndexInRound",
                table: "FinalMatches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RoundNumber",
                table: "FinalMatches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FinalMatches_PlacementBracketId",
                table: "FinalMatches",
                column: "PlacementBracketId");

            migrationBuilder.CreateIndex(
                name: "IX_FinalMatches_TeamAId",
                table: "FinalMatches",
                column: "TeamAId");

            migrationBuilder.CreateIndex(
                name: "IX_FinalMatches_TeamBId",
                table: "FinalMatches",
                column: "TeamBId");

            migrationBuilder.AddForeignKey(
                name: "FK_FinalMatches_PlacementBrackets_PlacementBracketId",
                table: "FinalMatches",
                column: "PlacementBracketId",
                principalTable: "PlacementBrackets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FinalMatches_Teams_TeamAId",
                table: "FinalMatches",
                column: "TeamAId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FinalMatches_Teams_TeamBId",
                table: "FinalMatches",
                column: "TeamBId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FinalMatches_PlacementBrackets_PlacementBracketId",
                table: "FinalMatches");

            migrationBuilder.DropForeignKey(
                name: "FK_FinalMatches_Teams_TeamAId",
                table: "FinalMatches");

            migrationBuilder.DropForeignKey(
                name: "FK_FinalMatches_Teams_TeamBId",
                table: "FinalMatches");

            migrationBuilder.DropIndex(
                name: "IX_FinalMatches_PlacementBracketId",
                table: "FinalMatches");

            migrationBuilder.DropIndex(
                name: "IX_FinalMatches_TeamAId",
                table: "FinalMatches");

            migrationBuilder.DropIndex(
                name: "IX_FinalMatches_TeamBId",
                table: "FinalMatches");

            migrationBuilder.DropColumn(
                name: "IndexInRound",
                table: "FinalMatches");

            migrationBuilder.DropColumn(
                name: "RoundNumber",
                table: "FinalMatches");
        }
    }
}
