using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialNetwork.Migrations
{
    /// <inheritdoc />
    public partial class FourthCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Post_PostId1",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Like_Post_PostId1",
                table: "Like");

            migrationBuilder.DropIndex(
                name: "IX_Like_PostId1",
                table: "Like");

            migrationBuilder.DropIndex(
                name: "IX_Comment_PostId1",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "PostId1",
                table: "Like");

            migrationBuilder.DropColumn(
                name: "PostId1",
                table: "Comment");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PostId1",
                table: "Like",
                type: "varchar(36)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PostId1",
                table: "Comment",
                type: "varchar(36)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Like_PostId1",
                table: "Like",
                column: "PostId1");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_PostId1",
                table: "Comment",
                column: "PostId1");

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
    }
}
