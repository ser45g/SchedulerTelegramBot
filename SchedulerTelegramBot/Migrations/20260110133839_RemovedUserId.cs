using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchedulerTelegramBot.Migrations
{
    /// <inheritdoc />
    public partial class RemovedUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Notifications");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "Notifications",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
