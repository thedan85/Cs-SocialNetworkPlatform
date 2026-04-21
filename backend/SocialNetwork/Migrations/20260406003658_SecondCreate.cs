using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialNetwork.Migrations
{
    /// <inheritdoc />
    public partial class SecondCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "User",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 989, DateTimeKind.Local).AddTicks(4873),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 3, 30, 7, 41, 31, 469, DateTimeKind.Local).AddTicks(968));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "User",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 986, DateTimeKind.Local).AddTicks(8750),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 3, 30, 7, 41, 31, 466, DateTimeKind.Local).AddTicks(1232));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Story",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 997, DateTimeKind.Local).AddTicks(8491),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 3, 30, 7, 41, 31, 477, DateTimeKind.Local).AddTicks(9323));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "PostHashtag",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 995, DateTimeKind.Local).AddTicks(3738),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 3, 30, 7, 41, 31, 475, DateTimeKind.Local).AddTicks(3499));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Post",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 992, DateTimeKind.Local).AddTicks(2294),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 3, 30, 7, 41, 31, 471, DateTimeKind.Local).AddTicks(9978));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Post",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 992, DateTimeKind.Local).AddTicks(1747),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 3, 30, 7, 41, 31, 471, DateTimeKind.Local).AddTicks(9364));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Like",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 999, DateTimeKind.Local).AddTicks(1881),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 3, 30, 7, 41, 31, 479, DateTimeKind.Local).AddTicks(2944));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Hashtag",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 994, DateTimeKind.Local).AddTicks(3652),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 3, 30, 7, 41, 31, 474, DateTimeKind.Local).AddTicks(3026));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Comment",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 996, DateTimeKind.Local).AddTicks(7924),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 3, 30, 7, 41, 31, 476, DateTimeKind.Local).AddTicks(8304));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Comment",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 996, DateTimeKind.Local).AddTicks(7427),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 3, 30, 7, 41, 31, 476, DateTimeKind.Local).AddTicks(7740));

            migrationBuilder.CreateTable(
                name: "Friendship",
                columns: table => new
                {
                    FriendshipId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId1 = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId2 = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValue: "Pending")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValue: new DateTime(2026, 4, 6, 7, 36, 58, 0, DateTimeKind.Local).AddTicks(9227)),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true, defaultValue: new DateTime(2026, 4, 6, 7, 36, 58, 1, DateTimeKind.Local).AddTicks(1418))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friendship", x => x.FriendshipId);
                    table.ForeignKey(
                        name: "FK_Friendship_User_UserId1",
                        column: x => x.UserId1,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Friendship_User_UserId2",
                        column: x => x.UserId2,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserFriendship",
                columns: table => new
                {
                    FriendshipId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AcceptedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValue: new DateTime(2026, 4, 6, 7, 36, 58, 2, DateTimeKind.Local).AddTicks(8756))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFriendship", x => new { x.FriendshipId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserFriendship_Friendship_FriendshipId",
                        column: x => x.FriendshipId,
                        principalTable: "Friendship",
                        principalColumn: "FriendshipId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFriendship_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Friendship_UserId1",
                table: "Friendship",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Friendship_UserId2",
                table: "Friendship",
                column: "UserId2");

            migrationBuilder.CreateIndex(
                name: "IX_UserFriendship_UserId",
                table: "UserFriendship",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFriendship");

            migrationBuilder.DropTable(
                name: "Friendship");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "User",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 30, 7, 41, 31, 469, DateTimeKind.Local).AddTicks(968),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 989, DateTimeKind.Local).AddTicks(4873));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "User",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 30, 7, 41, 31, 466, DateTimeKind.Local).AddTicks(1232),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 986, DateTimeKind.Local).AddTicks(8750));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Story",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 30, 7, 41, 31, 477, DateTimeKind.Local).AddTicks(9323),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 997, DateTimeKind.Local).AddTicks(8491));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "PostHashtag",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 30, 7, 41, 31, 475, DateTimeKind.Local).AddTicks(3499),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 995, DateTimeKind.Local).AddTicks(3738));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Post",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 30, 7, 41, 31, 471, DateTimeKind.Local).AddTicks(9978),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 992, DateTimeKind.Local).AddTicks(2294));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Post",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 30, 7, 41, 31, 471, DateTimeKind.Local).AddTicks(9364),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 992, DateTimeKind.Local).AddTicks(1747));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Like",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 30, 7, 41, 31, 479, DateTimeKind.Local).AddTicks(2944),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 999, DateTimeKind.Local).AddTicks(1881));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Hashtag",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 30, 7, 41, 31, 474, DateTimeKind.Local).AddTicks(3026),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 994, DateTimeKind.Local).AddTicks(3652));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Comment",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 30, 7, 41, 31, 476, DateTimeKind.Local).AddTicks(8304),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 996, DateTimeKind.Local).AddTicks(7924));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Comment",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 30, 7, 41, 31, 476, DateTimeKind.Local).AddTicks(7740),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 996, DateTimeKind.Local).AddTicks(7427));
        }
    }
}
