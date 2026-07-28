using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace thefirey33_backend.Migrations.NikoDexRecovery
{
    /// <inheritdoc />
    public partial class ModifyNikoDatabaseRecoverySystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "NikoDexRecovery");

            migrationBuilder.CreateTable(
                name: "NikoTypeRecoveryDb",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    AuthorName = table.Column<string>(type: "text", nullable: true),
                    FullDescription = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    IsBlacklisted = table.Column<bool>(type: "boolean", nullable: false),
                    NikoDexRecoveryDbTypeId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NikoTypeRecoveryDb", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NikoTypeRecoveryDb_NikoDexRecovery_NikoDexRecoveryDbTypeId",
                        column: x => x.NikoDexRecoveryDbTypeId,
                        principalTable: "NikoDexRecovery",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AbilityTypeRecoveryDb",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NikoTypeRecoveryDbId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbilityTypeRecoveryDb", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbilityTypeRecoveryDb_NikoTypeRecoveryDb_NikoTypeRecoveryDb~",
                        column: x => x.NikoTypeRecoveryDbId,
                        principalTable: "NikoTypeRecoveryDb",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbilityTypeRecoveryDb_NikoTypeRecoveryDbId",
                table: "AbilityTypeRecoveryDb",
                column: "NikoTypeRecoveryDbId");

            migrationBuilder.CreateIndex(
                name: "IX_NikoTypeRecoveryDb_NikoDexRecoveryDbTypeId",
                table: "NikoTypeRecoveryDb",
                column: "NikoDexRecoveryDbTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AbilityTypeRecoveryDb");

            migrationBuilder.DropTable(
                name: "NikoTypeRecoveryDb");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "NikoDexRecovery",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }
    }
}
