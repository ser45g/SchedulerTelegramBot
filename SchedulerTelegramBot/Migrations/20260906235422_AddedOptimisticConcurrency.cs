using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchedulerTelegramBot.Migrations
{
    /// <inheritdoc />
    public partial class AddedOptimisticConcurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "RowVersion",
                table: "Subscription",
                type: "INTEGER",
                rowVersion: true,
                nullable: false,
                defaultValueSql: "0");

            migrationBuilder.AlterColumn<long>(
                name: "RowVersion",
                table: "OutboxState",
                type: "INTEGER",
                rowVersion: true,
                nullable: true,
                defaultValueSql: "0",
                oldClrType: typeof(byte[]),
                oldType: "BLOB",
                oldRowVersion: true,
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RowVersion",
                table: "Notifications",
                type: "INTEGER",
                rowVersion: true,
                nullable: false,
                defaultValueSql: "0");

            migrationBuilder.AlterColumn<long>(
                name: "RowVersion",
                table: "InboxState",
                type: "INTEGER",
                rowVersion: true,
                nullable: true,
                defaultValueSql: "0",
                oldClrType: typeof(byte[]),
                oldType: "BLOB",
                oldRowVersion: true,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Subscription");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Notifications");

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "OutboxState",
                type: "BLOB",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(long),
                oldType: "INTEGER",
                oldRowVersion: true,
                oldNullable: true,
                oldDefaultValueSql: "0");

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "InboxState",
                type: "BLOB",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(long),
                oldType: "INTEGER",
                oldRowVersion: true,
                oldNullable: true,
                oldDefaultValueSql: "0");
        }
    }
}
