using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ChatRoomASP.Migrations
{
    /// <inheritdoc />
    public partial class InitCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IMUsers",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ProfilePicture = table.Column<byte[]>(type: "BLOB", maxLength: 10240, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    LastActiveTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IMUsers", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "RelationTypes",
                columns: table => new
                {
                    RelationTypeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RelationTypeName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelationTypes", x => x.RelationTypeId);
                });

            migrationBuilder.CreateTable(
                name: "IMGroups",
                columns: table => new
                {
                    GroupId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GroupName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    CreatorId = table.Column<long>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    AvatarUrl = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IMGroups", x => x.GroupId);
                    table.ForeignKey(
                        name: "FK_IMGroups_IMUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "IMUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRelations",
                columns: table => new
                {
                    UserRelationId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InitiatorUserId = table.Column<long>(type: "INTEGER", nullable: false),
                    TargetUserId = table.Column<long>(type: "INTEGER", nullable: false),
                    RelationTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    RemarkName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRelations", x => x.UserRelationId);
                    table.ForeignKey(
                        name: "FK_UserRelations_IMUsers_InitiatorUserId",
                        column: x => x.InitiatorUserId,
                        principalTable: "IMUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRelations_IMUsers_TargetUserId",
                        column: x => x.TargetUserId,
                        principalTable: "IMUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRelations_RelationTypes_RelationTypeId",
                        column: x => x.RelationTypeId,
                        principalTable: "RelationTypes",
                        principalColumn: "RelationTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IMGroupMembers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GroupId = table.Column<long>(type: "INTEGER", nullable: false),
                    UserId = table.Column<long>(type: "INTEGER", nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    IsAdmin = table.Column<bool>(type: "INTEGER", nullable: false),
                    MuteUntil = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    JoinedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IMGroupMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IMGroupMembers_IMGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "IMGroups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IMGroupMembers_IMUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "IMUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IMMessages",
                columns: table => new
                {
                    MessageId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    SenderId = table.Column<long>(type: "INTEGER", nullable: false),
                    ReceiverId = table.Column<long>(type: "INTEGER", nullable: true),
                    GroupId = table.Column<long>(type: "INTEGER", nullable: true),
                    Content = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    FileName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    SentAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IMMessages", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_IMMessages_IMGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "IMGroups",
                        principalColumn: "GroupId");
                    table.ForeignKey(
                        name: "FK_IMMessages_IMUsers_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "IMUsers",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_IMMessages_IMUsers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "IMUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "IMUsers",
                columns: new[] { "UserId", "CreatedAt", "Email", "LastActiveTime", "PasswordHash", "ProfilePicture", "Status", "UpdatedAt", "UserName" },
                values: new object[] { 10000000L, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), "initial@example.com", new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), "hashedpassword", new byte[0], 0, new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), "InitialUser" });

            migrationBuilder.InsertData(
                table: "RelationTypes",
                columns: new[] { "RelationTypeId", "RelationTypeName" },
                values: new object[,]
                {
                    { 1, "Friend" },
                    { 2, "SpecialCare" },
                    { 3, "Blacklist" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_IMGroupMembers_GroupId_UserId",
                table: "IMGroupMembers",
                columns: new[] { "GroupId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IMGroupMembers_UserId",
                table: "IMGroupMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_IMGroups_CreatorId",
                table: "IMGroups",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_IMMessages_GroupId_SentAt",
                table: "IMMessages",
                columns: new[] { "GroupId", "SentAt" });

            migrationBuilder.CreateIndex(
                name: "IX_IMMessages_ReceiverId_SentAt",
                table: "IMMessages",
                columns: new[] { "ReceiverId", "SentAt" });

            migrationBuilder.CreateIndex(
                name: "IX_IMMessages_SenderId",
                table: "IMMessages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRelations_InitiatorUserId",
                table: "UserRelations",
                column: "InitiatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRelations_RelationTypeId",
                table: "UserRelations",
                column: "RelationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRelations_TargetUserId",
                table: "UserRelations",
                column: "TargetUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IMGroupMembers");

            migrationBuilder.DropTable(
                name: "IMMessages");

            migrationBuilder.DropTable(
                name: "UserRelations");

            migrationBuilder.DropTable(
                name: "IMGroups");

            migrationBuilder.DropTable(
                name: "RelationTypes");

            migrationBuilder.DropTable(
                name: "IMUsers");
        }
    }
}
