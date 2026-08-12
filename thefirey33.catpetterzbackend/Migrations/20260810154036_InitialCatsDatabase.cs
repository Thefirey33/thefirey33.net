using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace thefirey33.catpetterzBackend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCatsDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Health = table.Column<double>(type: "double precision", nullable: false),
                    Hunger = table.Column<byte>(type: "smallint", nullable: false),
                    Thirst = table.Column<byte>(type: "smallint", nullable: false),
                    ImagePath = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    TheCatWentOnSomeAdventures = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cats", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cats");
        }
    }
}
