using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace thefirey33_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthorField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Arts",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "Arts");
        }
    }
}
