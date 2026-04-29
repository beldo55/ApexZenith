using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApexZenith.Migrations
{
    /// <inheritdoc />
    public partial class hhyiu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Order",
                table: "Resources",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IconClass",
                table: "Resources",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ParentId1",
                table: "Resources",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Resources_ParentId1",
                table: "Resources",
                column: "ParentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Resources_Resources_ParentId1",
                table: "Resources",
                column: "ParentId1",
                principalTable: "Resources",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Resources_Resources_ParentId1",
                table: "Resources");

            migrationBuilder.DropIndex(
                name: "IX_Resources_ParentId1",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "IconClass",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "ParentId1",
                table: "Resources");

            migrationBuilder.AlterColumn<int>(
                name: "Order",
                table: "Resources",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
