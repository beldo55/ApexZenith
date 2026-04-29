using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApexZenith.Migrations
{
    /// <inheritdoc />
    public partial class hyu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IconClass",
                table: "Resources",
                type: "text",
                nullable: false,
                defaultValue: "bi bi-circle",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<bool>(
                name: "IsAction",
                table: "Resources",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAction",
                table: "Resources");

            migrationBuilder.AlterColumn<string>(
                name: "IconClass",
                table: "Resources",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "bi bi-circle");
        }
    }
}
