using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatRoomASP.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMessageTypeColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MessageType",
                table: "IMMessages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MessageType",
                table: "IMMessages",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
