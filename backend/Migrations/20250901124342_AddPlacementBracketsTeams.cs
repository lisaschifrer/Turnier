using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPlacementBracketsTeams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FinalMatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamAId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamBId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WinnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PlacementBracketId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinalMatches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "placementBracketTeams",
                columns: table => new
                {
                    PlacementBracketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Seed = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_placementBracketTeams", x => new { x.PlacementBracketId, x.TeamId });
                    table.ForeignKey(
                        name: "FK_placementBracketTeams_PlacementBrackets_PlacementBracketId",
                        column: x => x.PlacementBracketId,
                        principalTable: "PlacementBrackets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_placementBracketTeams_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_placementBracketTeams_PlacementBracketId_Seed",
                table: "placementBracketTeams",
                columns: new[] { "PlacementBracketId", "Seed" },
                unique: true,
                filter: "[Seed] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_placementBracketTeams_TeamId",
                table: "placementBracketTeams",
                column: "TeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FinalMatches");

            migrationBuilder.DropTable(
                name: "placementBracketTeams");
        }
    }
}
