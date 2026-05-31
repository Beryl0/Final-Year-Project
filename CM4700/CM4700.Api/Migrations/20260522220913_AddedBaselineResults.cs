using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CM4700.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedBaselineResults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BaselineFindings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScanRequestId = table.Column<int>(type: "int", nullable: false),
                    RuleId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Impact = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Help = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HelpUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ElementHtml = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Target = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaselineFindings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BaselineFindings_ScanRequests_ScanRequestId",
                        column: x => x.ScanRequestId,
                        principalTable: "ScanRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaselineFindings_ScanRequestId",
                table: "BaselineFindings",
                column: "ScanRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BaselineFindings");
        }
    }
}
