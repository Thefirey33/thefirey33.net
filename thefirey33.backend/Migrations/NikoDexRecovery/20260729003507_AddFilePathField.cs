#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace thefirey33_backend.Migrations.NikoDexRecovery;

/// <inheritdoc />
public partial class AddFilePathField : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            "ImagePath",
            "NikoTypeRecoveryDb",
            "character varying(256)",
            maxLength: 256,
            defaultValue: "");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            "ImagePath",
            "NikoTypeRecoveryDb");
    }
}