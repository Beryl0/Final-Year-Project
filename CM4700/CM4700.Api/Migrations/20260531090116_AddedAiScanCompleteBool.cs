using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CM4700.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedAiScanCompleteBool : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsCompleted",
                table: "ScanRequests",
                newName: "BaselineScanIsCompleted");

            migrationBuilder.RenameColumn(
                name: "DateTimeCompleted",
                table: "ScanRequests",
                newName: "BaselineScanDateTimeCompleted");

            migrationBuilder.AddColumn<DateTime>(
                name: "AIScanDateTimeCompleted",
                table: "ScanRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AIScanIsCompleted",
                table: "ScanRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AIScanDateTimeCompleted",
                table: "ScanRequests");

            migrationBuilder.DropColumn(
                name: "AIScanIsCompleted",
                table: "ScanRequests");

            migrationBuilder.RenameColumn(
                name: "BaselineScanIsCompleted",
                table: "ScanRequests",
                newName: "IsCompleted");

            migrationBuilder.RenameColumn(
                name: "BaselineScanDateTimeCompleted",
                table: "ScanRequests",
                newName: "DateTimeCompleted");
        }
    }
}
