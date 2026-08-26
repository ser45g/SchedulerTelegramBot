using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchedulerTelegramBot.Migrations
{
    /// <inheritdoc />
    public partial class AddedSubscripitons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NotifyDateTime",
                table: "Notifications",
                newName: "NotifyAtUtc");

            migrationBuilder.RenameColumn(
                name: "LastUpdatedDateTime",
                table: "Notifications",
                newName: "LastUpdatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "AddedDateTime",
                table: "Notifications",
                newName: "AddedAtUtc");

            migrationBuilder.CreateTable(
                name: "Subscription",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ChatId = table.Column<long>(type: "INTEGER", nullable: false),
                    AddedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndsAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastUpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscription", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Subscription");

            migrationBuilder.RenameColumn(
                name: "NotifyAtUtc",
                table: "Notifications",
                newName: "NotifyDateTime");

            migrationBuilder.RenameColumn(
                name: "LastUpdatedAtUtc",
                table: "Notifications",
                newName: "LastUpdatedDateTime");

            migrationBuilder.RenameColumn(
                name: "AddedAtUtc",
                table: "Notifications",
                newName: "AddedDateTime");
        }
    }
}
