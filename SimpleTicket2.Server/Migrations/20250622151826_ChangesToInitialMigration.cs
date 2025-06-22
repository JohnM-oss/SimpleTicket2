using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SimpleTicket2.Server.Migrations
{
    /// <inheritdoc />
    public partial class ChangesToInitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Tickets",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "Priority", "Status", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 6, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", "First test ticket", 0, 0, "Test Ticket 1", null },
                    { 2, new DateTime(2025, 6, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", "Second test ticket", 0, 3, "Test Ticket 2", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
