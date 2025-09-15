using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Planty.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateLocationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Create the Locations table first
            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Step 2: Add LocationId column to Plants table
            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                table: "Plants",
                type: "TEXT",
                nullable: true);

            // Step 3: Migrate existing location data
            // Create Location records for each unique location name per user and update Plants
            migrationBuilder.Sql(@"
                INSERT INTO Locations (Id, Name, UserId)
                SELECT 
                    lower(hex(randomblob(4))) || '-' || lower(hex(randomblob(2))) || '-' || '4' || substr(lower(hex(randomblob(2))),2) || '-' || 'a' || substr(lower(hex(randomblob(2))),2) || '-' || lower(hex(randomblob(6))),
                    Location,
                    UserId
                FROM (
                    SELECT DISTINCT Location, UserId 
                    FROM Plants 
                    WHERE Location IS NOT NULL AND Location != ''
                );
            ");

            // Step 4: Update Plants table to reference the new Location records
            migrationBuilder.Sql(@"
                UPDATE Plants 
                SET LocationId = (
                    SELECT l.Id 
                    FROM Locations l 
                    WHERE l.Name = Plants.Location 
                    AND l.UserId = Plants.UserId
                )
                WHERE Location IS NOT NULL AND Location != '';
            ");

            // Step 5: Drop the old Location column
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Plants");

            // Step 6: Create indexes and foreign keys
            migrationBuilder.CreateIndex(
                name: "IX_Plants_LocationId",
                table: "Plants",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_UserId",
                table: "Locations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Plants_Locations_LocationId",
                table: "Plants",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Step 1: Add back the Location column to Plants
            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Plants",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            // Step 2: Migrate location data back from Locations table to Plants
            migrationBuilder.Sql(@"
                UPDATE Plants 
                SET Location = (
                    SELECT l.Name 
                    FROM Locations l 
                    WHERE l.Id = Plants.LocationId
                )
                WHERE LocationId IS NOT NULL;
            ");

            // Step 3: Drop foreign key and indexes
            migrationBuilder.DropForeignKey(
                name: "FK_Plants_Locations_LocationId",
                table: "Plants");

            migrationBuilder.DropIndex(
                name: "IX_Plants_LocationId",
                table: "Plants");

            migrationBuilder.DropIndex(
                name: "IX_Locations_UserId",
                table: "Locations");

            // Step 4: Drop the Locations table
            migrationBuilder.DropTable(
                name: "Locations");

            // Step 5: Drop the LocationId column
            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Plants");
        }
    }
}
