using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace thefirey33_backend.Migrations.Question
{
    /// <inheritdoc />
    public partial class UserIdMustBeULONG : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "UserId",
                table: "Questions",
                type: "numeric(20,0)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Questions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)");
        }
    }
}
