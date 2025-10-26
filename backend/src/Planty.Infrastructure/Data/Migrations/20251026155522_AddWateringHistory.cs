using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Planty.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWateringHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create the Waterings table first
            migrationBuilder.CreateTable(
                name: "Waterings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    WateredAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    PlantId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Waterings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Waterings_Plants_PlantId",
                        column: x => x.PlantId,
                        principalTable: "Plants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Migrate existing LastWatered data to Waterings table
            migrationBuilder.Sql(@"
                INSERT INTO Waterings (Id, PlantId, WateredAt, Notes)
                SELECT 
                    lower(hex(randomblob(16))),
                    Id,
                    LastWatered,
                    NULL
                FROM Plants
                WHERE LastWatered IS NOT NULL;
            ");

            // Now drop the LastWatered column
            migrationBuilder.DropColumn(
                name: "LastWatered",
                table: "Plants");

            migrationBuilder.CreateIndex(
                name: "IX_Waterings_PlantId_WateredAt",
                table: "Waterings",
                columns: new[] { "PlantId", "WateredAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Waterings");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastWatered",
                table: "Plants",
                type: "TEXT",
                nullable: true);
        }
    }
}
