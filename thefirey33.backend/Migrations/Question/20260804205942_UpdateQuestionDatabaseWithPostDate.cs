using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace thefirey33_backend.Migrations.Question
{
    /// <inheritdoc />
    public partial class UpdateQuestionDatabaseWithPostDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "QuestionPostTime",
                table: "Questions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuestionPostTime",
                table: "Questions");
        }
    }
}
