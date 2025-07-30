using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartDesk.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIsReminderSent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReminderSent",
                table: "TodoItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReminderSent",
                table: "TodoItems");
        }
    }
}
