using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialNetwork.Migrations
{
    /// <inheritdoc />
    public partial class PostPrivacy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Privacy",
                table: "Post",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Public")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Privacy",
                table: "Post");
        }
    }
}
