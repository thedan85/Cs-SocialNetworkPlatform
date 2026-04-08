using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialNetwork.Migrations
{
    /// <inheritdoc />
    public partial class ThirdCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "AcceptedAt",
                table: "UserFriendship",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 8, 16, 48, 16, 57, DateTimeKind.Local).AddTicks(1203),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 6, 7, 36, 58, 2, DateTimeKind.Local).AddTicks(8756));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "User",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 8, 16, 48, 16, 13, DateTimeKind.Local).AddTicks(3073),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 989, DateTimeKind.Local).AddTicks(4873));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "User",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 8, 16, 48, 16, 7, DateTimeKind.Local).AddTicks(3292),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 986, DateTimeKind.Local).AddTicks(8750));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Story",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 8, 16, 48, 16, 32, DateTimeKind.Local).AddTicks(7919),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 997, DateTimeKind.Local).AddTicks(8491));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "PostHashtag",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 8, 16, 48, 16, 23, DateTimeKind.Local).AddTicks(5837),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 995, DateTimeKind.Local).AddTicks(3738));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Post",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 8, 16, 48, 16, 18, DateTimeKind.Local).AddTicks(538),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 992, DateTimeKind.Local).AddTicks(2294));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Post",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 8, 16, 48, 16, 17, DateTimeKind.Local).AddTicks(9507),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 992, DateTimeKind.Local).AddTicks(1747));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Like",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 8, 16, 48, 16, 39, DateTimeKind.Local).AddTicks(3999),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 999, DateTimeKind.Local).AddTicks(1881));

            migrationBuilder.AddColumn<string>(
                name: "PostId1",
                table: "Like",
                type: "varchar(36)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Hashtag",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 8, 16, 48, 16, 21, DateTimeKind.Local).AddTicks(8244),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 994, DateTimeKind.Local).AddTicks(3652));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Friendship",
                type: "datetime(6)",
                nullable: true,
                defaultValue: new DateTime(2026, 4, 8, 16, 48, 16, 46, DateTimeKind.Local).AddTicks(5827),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldDefaultValue: new DateTime(2026, 4, 6, 7, 36, 58, 1, DateTimeKind.Local).AddTicks(1418));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Friendship",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 8, 16, 48, 16, 46, DateTimeKind.Local).AddTicks(1947),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 6, 7, 36, 58, 0, DateTimeKind.Local).AddTicks(9227));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Comment",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 8, 16, 48, 16, 26, DateTimeKind.Local).AddTicks(5646),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 996, DateTimeKind.Local).AddTicks(7924));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Comment",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 8, 16, 48, 16, 26, DateTimeKind.Local).AddTicks(4572),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 996, DateTimeKind.Local).AddTicks(7427));

            migrationBuilder.AddColumn<string>(
                name: "PostId1",
                table: "Comment",
                type: "varchar(36)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    NotificationId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RecipientUserId = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SenderUserId = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Content = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValue: new DateTime(2026, 4, 8, 16, 48, 16, 59, DateTimeKind.Local).AddTicks(6740)),
                    IsRead = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_Notification_User_RecipientUserId",
                        column: x => x.RecipientUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notification_User_SenderUserId",
                        column: x => x.SenderUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PostReport",
                columns: table => new
                {
                    PostReportId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PostId = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReporterUserId = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Reason = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostReport", x => x.PostReportId);
                    table.ForeignKey(
                        name: "FK_PostReport_Post_PostId",
                        column: x => x.PostId,
                        principalTable: "Post",
                        principalColumn: "PostId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostReport_User_ReporterUserId",
                        column: x => x.ReporterUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Like_PostId1",
                table: "Like",
                column: "PostId1");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Like_ExactlyOneTarget",
                table: "Like",
                sql: "(CASE WHEN PostId IS NOT NULL THEN 1 ELSE 0 END + CASE WHEN CommentId IS NOT NULL THEN 1 ELSE 0 END + CASE WHEN StoryId IS NOT NULL THEN 1 ELSE 0 END) = 1");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Friendship_DifferentUsers",
                table: "Friendship",
                sql: "UserId1 <> UserId2");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_PostId1",
                table: "Comment",
                column: "PostId1");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_RecipientUserId",
                table: "Notification",
                column: "RecipientUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_SenderUserId",
                table: "Notification",
                column: "SenderUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReport_PostId",
                table: "PostReport",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReport_ReporterUserId",
                table: "PostReport",
                column: "ReporterUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Post_PostId1",
                table: "Comment",
                column: "PostId1",
                principalTable: "Post",
                principalColumn: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Like_Post_PostId1",
                table: "Like",
                column: "PostId1",
                principalTable: "Post",
                principalColumn: "PostId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Post_PostId1",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Like_Post_PostId1",
                table: "Like");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "PostReport");

            migrationBuilder.DropIndex(
                name: "IX_Like_PostId1",
                table: "Like");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Like_ExactlyOneTarget",
                table: "Like");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Friendship_DifferentUsers",
                table: "Friendship");

            migrationBuilder.DropIndex(
                name: "IX_Comment_PostId1",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "PostId1",
                table: "Like");

            migrationBuilder.DropColumn(
                name: "PostId1",
                table: "Comment");

            migrationBuilder.AlterColumn<DateTime>(
                name: "AcceptedAt",
                table: "UserFriendship",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 6, 7, 36, 58, 2, DateTimeKind.Local).AddTicks(8756),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 8, 16, 48, 16, 57, DateTimeKind.Local).AddTicks(1203));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "User",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 989, DateTimeKind.Local).AddTicks(4873),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 8, 16, 48, 16, 13, DateTimeKind.Local).AddTicks(3073));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "User",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 986, DateTimeKind.Local).AddTicks(8750),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 8, 16, 48, 16, 7, DateTimeKind.Local).AddTicks(3292));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Story",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 997, DateTimeKind.Local).AddTicks(8491),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 8, 16, 48, 16, 32, DateTimeKind.Local).AddTicks(7919));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "PostHashtag",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 995, DateTimeKind.Local).AddTicks(3738),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 8, 16, 48, 16, 23, DateTimeKind.Local).AddTicks(5837));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Post",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 992, DateTimeKind.Local).AddTicks(2294),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 8, 16, 48, 16, 18, DateTimeKind.Local).AddTicks(538));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Post",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 992, DateTimeKind.Local).AddTicks(1747),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 8, 16, 48, 16, 17, DateTimeKind.Local).AddTicks(9507));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Like",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 999, DateTimeKind.Local).AddTicks(1881),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 8, 16, 48, 16, 39, DateTimeKind.Local).AddTicks(3999));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Hashtag",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 994, DateTimeKind.Local).AddTicks(3652),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 8, 16, 48, 16, 21, DateTimeKind.Local).AddTicks(8244));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Friendship",
                type: "datetime(6)",
                nullable: true,
                defaultValue: new DateTime(2026, 4, 6, 7, 36, 58, 1, DateTimeKind.Local).AddTicks(1418),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldDefaultValue: new DateTime(2026, 4, 8, 16, 48, 16, 46, DateTimeKind.Local).AddTicks(5827));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Friendship",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 6, 7, 36, 58, 0, DateTimeKind.Local).AddTicks(9227),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 8, 16, 48, 16, 46, DateTimeKind.Local).AddTicks(1947));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Comment",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 996, DateTimeKind.Local).AddTicks(7924),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 8, 16, 48, 16, 26, DateTimeKind.Local).AddTicks(5646));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Comment",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2026, 4, 6, 7, 36, 57, 996, DateTimeKind.Local).AddTicks(7427),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2026, 4, 8, 16, 48, 16, 26, DateTimeKind.Local).AddTicks(4572));
        }
    }
}
