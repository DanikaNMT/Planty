using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Planty.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFertilizationTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FertilizationIntervalDays",
                table: "Plants",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Fertilizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FertilizedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    PlantId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fertilizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fertilizations_Plants_PlantId",
                        column: x => x.PlantId,
                        principalTable: "Plants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fertilizations_PlantId_FertilizedAt",
                table: "Fertilizations",
                columns: new[] { "PlantId", "FertilizedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Fertilizations");

            migrationBuilder.DropColumn(
                name: "FertilizationIntervalDays",
                table: "Plants");
        }
    }
}
