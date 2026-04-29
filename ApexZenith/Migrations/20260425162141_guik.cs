using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApexZenith.Migrations
{
    /// <inheritdoc />
    public partial class guik : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IconClass",
                table: "Resources",
                type: "text",
                nullable: true,
                defaultValue: "bi bi-circle",
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "bi bi-circle");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IconClass",
                table: "Resources",
                type: "text",
                nullable: false,
                defaultValue: "bi bi-circle",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true,
                oldDefaultValue: "bi bi-circle");
        }
    }
}
