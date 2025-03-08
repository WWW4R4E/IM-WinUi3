using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatRoomASP.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIMMessageModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SentTime",
                table: "IMMessages",
                newName: "SentAt");

            migrationBuilder.RenameColumn(
                name: "PostId",
                table: "IMMessages",
                newName: "MessageId");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "IMMessages",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SentAt",
                table: "IMMessages",
                newName: "SentTime");

            migrationBuilder.RenameColumn(
                name: "MessageId",
                table: "IMMessages",
                newName: "PostId");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "IMMessages",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }
    }
}
