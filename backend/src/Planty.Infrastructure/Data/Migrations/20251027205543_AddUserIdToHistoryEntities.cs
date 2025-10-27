using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Planty.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToHistoryEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Add UserId columns as nullable
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Waterings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "PlantPictures",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Fertilizations",
                type: "TEXT",
                nullable: true);

            // Step 2: Update existing records to set UserId from the plant's owner
            migrationBuilder.Sql(@"
                UPDATE Waterings 
                SET UserId = (SELECT UserId FROM Plants WHERE Plants.Id = Waterings.PlantId)
                WHERE UserId IS NULL;
            ");

            migrationBuilder.Sql(@"
                UPDATE Fertilizations 
                SET UserId = (SELECT UserId FROM Plants WHERE Plants.Id = Fertilizations.PlantId)
                WHERE UserId IS NULL;
            ");

            migrationBuilder.Sql(@"
                UPDATE PlantPictures 
                SET UserId = (SELECT UserId FROM Plants WHERE Plants.Id = PlantPictures.PlantId)
                WHERE UserId IS NULL;
            ");

            // Step 3: Make columns NOT NULL (SQLite requires recreating the table)
            // For SQLite, we'll keep them nullable and handle it in the application
            // Alternatively, we could recreate tables, but that's complex

            // Step 4: Create indexes
            migrationBuilder.CreateIndex(
                name: "IX_Waterings_UserId",
                table: "Waterings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlantPictures_UserId",
                table: "PlantPictures",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Fertilizations_UserId",
                table: "Fertilizations",
                column: "UserId");

            // Step 5: Add foreign keys (SQLite will allow nullable foreign keys)
            migrationBuilder.AddForeignKey(
                name: "FK_Fertilizations_Users_UserId",
                table: "Fertilizations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlantPictures_Users_UserId",
                table: "PlantPictures",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Waterings_Users_UserId",
                table: "Waterings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fertilizations_Users_UserId",
                table: "Fertilizations");

            migrationBuilder.DropForeignKey(
                name: "FK_PlantPictures_Users_UserId",
                table: "PlantPictures");

            migrationBuilder.DropForeignKey(
                name: "FK_Waterings_Users_UserId",
                table: "Waterings");

            migrationBuilder.DropIndex(
                name: "IX_Waterings_UserId",
                table: "Waterings");

            migrationBuilder.DropIndex(
                name: "IX_PlantPictures_UserId",
                table: "PlantPictures");

            migrationBuilder.DropIndex(
                name: "IX_Fertilizations_UserId",
                table: "Fertilizations");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Waterings");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PlantPictures");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Fertilizations");
        }
    }
}
