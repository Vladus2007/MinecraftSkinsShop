using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Presentation.Migrations
{
    /// <inheritdoc />
    public partial class ChangeUserIdToText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Ensure UserId is text for string storage (handles any previous integer attempts)
            migrationBuilder.Sql("ALTER TABLE \"Purchases\" ALTER COLUMN \"UserId\" TYPE text USING (\"UserId\"::text)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert to text (no-op, already text by design)
            migrationBuilder.Sql("ALTER TABLE \"Purchases\" ALTER COLUMN \"UserId\" TYPE text");
        }
    }
}
