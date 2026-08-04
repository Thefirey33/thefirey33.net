using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace thefirey33_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddLikesField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LikesDbType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Origin = table.Column<string>(type: "text", nullable: false),
                    ArtDbTypeId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LikesDbType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LikesDbType_Arts_ArtDbTypeId",
                        column: x => x.ArtDbTypeId,
                        principalTable: "Arts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LikesDbType_ArtDbTypeId",
                table: "LikesDbType",
                column: "ArtDbTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LikesDbType");
        }
    }
}
