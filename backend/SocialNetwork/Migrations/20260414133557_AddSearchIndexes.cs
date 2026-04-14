using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialNetwork.Migrations
{
    /// <inheritdoc />
    public partial class AddSearchIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "User",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_User_UserName",
                table: "User",
                column: "UserName");

            migrationBuilder.CreateIndex(
                name: "IX_Story_ExpiresAt_CreatedAt",
                table: "Story",
                columns: new[] { "ExpiresAt", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Story_UserId_ExpiresAt_CreatedAt",
                table: "Story",
                columns: new[] { "UserId", "ExpiresAt", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_PostReport_Status_CreatedAt",
                table: "PostReport",
                columns: new[] { "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Post_CreatedAt",
                table: "Post",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Post_UserId_CreatedAt",
                table: "Post",
                columns: new[] { "UserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Notification_RecipientUserId_IsRead_CreatedAt",
                table: "Notification",
                columns: new[] { "RecipientUserId", "IsRead", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Friendship_Status_UserId1_UpdatedAt",
                table: "Friendship",
                columns: new[] { "Status", "UserId1", "UpdatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Friendship_UserId2_Status_CreatedAt",
                table: "Friendship",
                columns: new[] { "UserId2", "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Comment_PostId_CreatedAt",
                table: "Comment",
                columns: new[] { "PostId", "CreatedAt" });

            migrationBuilder.DropIndex(
                name: "IX_Story_UserId",
                table: "Story");

            migrationBuilder.DropIndex(
                name: "IX_Post_UserId",
                table: "Post");

            migrationBuilder.DropIndex(
                name: "IX_Notification_RecipientUserId",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Friendship_UserId2",
                table: "Friendship");

            migrationBuilder.DropIndex(
                name: "IX_Comment_PostId",
                table: "Comment");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Story_UserId",
                table: "Story",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_UserId",
                table: "Post",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_RecipientUserId",
                table: "Notification",
                column: "RecipientUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendship_UserId2",
                table: "Friendship",
                column: "UserId2");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_PostId",
                table: "Comment",
                column: "PostId");

            migrationBuilder.DropIndex(
                name: "IX_User_Email",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_UserName",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_Story_ExpiresAt_CreatedAt",
                table: "Story");

            migrationBuilder.DropIndex(
                name: "IX_Story_UserId_ExpiresAt_CreatedAt",
                table: "Story");

            migrationBuilder.DropIndex(
                name: "IX_PostReport_Status_CreatedAt",
                table: "PostReport");

            migrationBuilder.DropIndex(
                name: "IX_Post_CreatedAt",
                table: "Post");

            migrationBuilder.DropIndex(
                name: "IX_Post_UserId_CreatedAt",
                table: "Post");

            migrationBuilder.DropIndex(
                name: "IX_Notification_RecipientUserId_IsRead_CreatedAt",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Friendship_Status_UserId1_UpdatedAt",
                table: "Friendship");

            migrationBuilder.DropIndex(
                name: "IX_Friendship_UserId2_Status_CreatedAt",
                table: "Friendship");

            migrationBuilder.DropIndex(
                name: "IX_Comment_PostId_CreatedAt",
                table: "Comment");
        }
    }
}
