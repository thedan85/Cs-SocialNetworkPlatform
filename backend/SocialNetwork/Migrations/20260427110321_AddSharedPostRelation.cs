using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialNetwork.Migrations
{
    /// <inheritdoc />
    public partial class AddSharedPostRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SharedPostId",
                table: "Post",
                type: "varchar(36)",
                maxLength: 36,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Post_SharedPostId",
                table: "Post",
                column: "SharedPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Post_SharedPostId",
                table: "Post",
                column: "SharedPostId",
                principalTable: "Post",
                principalColumn: "PostId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_Post_SharedPostId",
                table: "Post");

            migrationBuilder.DropIndex(
                name: "IX_Post_SharedPostId",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "SharedPostId",
                table: "Post");
        }
    }
}
