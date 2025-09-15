using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Planty.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDefaultToLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "Locations",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            // Create default "Home" location for all existing users who don't have any locations
            migrationBuilder.Sql(@"
                INSERT INTO Locations (Id, Name, Description, IsDefault, UserId)
                SELECT 
                    lower(hex(randomblob(4))) || '-' || lower(hex(randomblob(2))) || '-' || '4' || substr(lower(hex(randomblob(2))),2) || '-' || 'a' || substr(lower(hex(randomblob(2))),2) || '-' || lower(hex(randomblob(6))),
                    'Home',
                    'Default home location',
                    1,
                    u.Id
                FROM Users u
                WHERE NOT EXISTS (
                    SELECT 1 FROM Locations l WHERE l.UserId = u.Id
                );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "Locations");
        }
    }
}
