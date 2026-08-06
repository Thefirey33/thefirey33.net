using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace thefirey33_backend.Migrations.Question
{
    /// <inheritdoc />
    public partial class ChangeAvatarAuthorToUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarAuthor",
                table: "Questions");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Questions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Questions");

            migrationBuilder.AddColumn<string>(
                name: "AvatarAuthor",
                table: "Questions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
