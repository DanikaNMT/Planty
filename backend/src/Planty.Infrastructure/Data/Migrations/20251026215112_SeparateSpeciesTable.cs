using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Planty.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeparateSpeciesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Add SpeciesId column to Plants (before dropping old columns)
            migrationBuilder.AddColumn<Guid>(
                name: "SpeciesId",
                table: "Plants",
                type: "TEXT",
                nullable: true);

            // Step 2: Create Species table
            migrationBuilder.CreateTable(
                name: "Species",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    WateringIntervalDays = table.Column<int>(type: "INTEGER", nullable: true),
                    FertilizationIntervalDays = table.Column<int>(type: "INTEGER", nullable: true),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Species", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Species_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Step 3: Migrate existing plant data to create species
            // Create unique species from existing plants (grouped by Species name, WateringIntervalDays, FertilizationIntervalDays, UserId)
            migrationBuilder.Sql(@"
                INSERT INTO Species (Id, Name, WateringIntervalDays, FertilizationIntervalDays, UserId)
                SELECT 
                    upper(hex(randomblob(4))) || '-' || upper(hex(randomblob(2))) || '-' || '4' || substr(upper(hex(randomblob(2))),2) || '-' || 'A' || substr(upper(hex(randomblob(2))),2) || '-' || upper(hex(randomblob(6))) as Id,
                    COALESCE(Species, 'Unknown') as Name,
                    WateringIntervalDays,
                    FertilizationIntervalDays,
                    UserId
                FROM (
                    SELECT DISTINCT 
                        Species,
                        WateringIntervalDays,
                        FertilizationIntervalDays,
                        UserId
                    FROM Plants
                    WHERE Species IS NOT NULL AND Species != ''
                )
            ");

            // Step 4: Update plants to link to their species
            migrationBuilder.Sql(@"
                UPDATE Plants
                SET SpeciesId = (
                    SELECT s.Id
                    FROM Species s
                    WHERE s.Name = Plants.Species
                    AND s.UserId = Plants.UserId
                    AND (s.WateringIntervalDays = Plants.WateringIntervalDays OR (s.WateringIntervalDays IS NULL AND Plants.WateringIntervalDays IS NULL))
                    AND (s.FertilizationIntervalDays = Plants.FertilizationIntervalDays OR (s.FertilizationIntervalDays IS NULL AND Plants.FertilizationIntervalDays IS NULL))
                    LIMIT 1
                )
                WHERE Species IS NOT NULL AND Species != ''
            ");

            // Step 5: Create indexes
            migrationBuilder.CreateIndex(
                name: "IX_Plants_SpeciesId",
                table: "Plants",
                column: "SpeciesId");

            migrationBuilder.CreateIndex(
                name: "IX_Species_UserId",
                table: "Species",
                column: "UserId");

            // Step 6: Add foreign key constraint
            migrationBuilder.AddForeignKey(
                name: "FK_Plants_Species_SpeciesId",
                table: "Plants",
                column: "SpeciesId",
                principalTable: "Species",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            // Step 7: Drop old columns (now that data is migrated)
            migrationBuilder.DropColumn(
                name: "FertilizationIntervalDays",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "Species",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "WateringIntervalDays",
                table: "Plants");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plants_Species_SpeciesId",
                table: "Plants");

            migrationBuilder.DropTable(
                name: "Species");

            migrationBuilder.DropIndex(
                name: "IX_Plants_SpeciesId",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "SpeciesId",
                table: "Plants");

            migrationBuilder.AddColumn<int>(
                name: "FertilizationIntervalDays",
                table: "Plants",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Species",
                table: "Plants",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WateringIntervalDays",
                table: "Plants",
                type: "INTEGER",
                nullable: true);
        }
    }
}
