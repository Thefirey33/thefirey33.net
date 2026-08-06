using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace thefirey33_backend.Migrations.Question
{
    /// <inheritdoc />
    public partial class AddAuthorFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorName",
                table: "Questions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AvatarAuthor",
                table: "Questions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorName",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "AvatarAuthor",
                table: "Questions");
        }
    }
}
