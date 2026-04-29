using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApexZenith.Migrations
{
    /// <inheritdoc />
    public partial class huyftyddytdfuyk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NewsNewsCategory");

            migrationBuilder.AlterColumn<string>(
                name: "PhotoUrl",
                table: "News",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_News_NewsCategoryId",
                table: "News",
                column: "NewsCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_News_NewsCategories_NewsCategoryId",
                table: "News",
                column: "NewsCategoryId",
                principalTable: "NewsCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_News_NewsCategories_NewsCategoryId",
                table: "News");

            migrationBuilder.DropIndex(
                name: "IX_News_NewsCategoryId",
                table: "News");

            migrationBuilder.AlterColumn<string>(
                name: "PhotoUrl",
                table: "News",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "NewsNewsCategory",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    NewsListId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsNewsCategory", x => new { x.CategoryId, x.NewsListId });
                    table.ForeignKey(
                        name: "FK_NewsNewsCategory_NewsCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "NewsCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NewsNewsCategory_News_NewsListId",
                        column: x => x.NewsListId,
                        principalTable: "News",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NewsNewsCategory_NewsListId",
                table: "NewsNewsCategory",
                column: "NewsListId");
        }
    }
}
