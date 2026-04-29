using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ApexZenith.Migrations
{
    /// <inheritdoc />
    public partial class K : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NumbersOviews",
                table: "News",
                newName: "NumberOfViews");

            migrationBuilder.RenameColumn(
                name: "NewsCategoriesId",
                table: "News",
                newName: "NewsCategoryId");

            migrationBuilder.RenameColumn(
                name: "Massage",
                table: "Contact",
                newName: "Message");

            migrationBuilder.CreateTable(
                name: "NewsComment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NewsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewsComment_News_NewsId",
                        column: x => x.NewsId,
                        principalTable: "News",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "NewsCommentReply",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NewsCommentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsCommentReply", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewsCommentReply_NewsComment_NewsCommentId",
                        column: x => x.NewsCommentId,
                        principalTable: "NewsComment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NewsComment_NewsId",
                table: "NewsComment",
                column: "NewsId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsCommentReply_NewsCommentId",
                table: "NewsCommentReply",
                column: "NewsCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsNewsCategory_NewsListId",
                table: "NewsNewsCategory",
                column: "NewsListId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NewsCommentReply");

            migrationBuilder.DropTable(
                name: "NewsNewsCategory");

            migrationBuilder.DropTable(
                name: "NewsComment");

            migrationBuilder.RenameColumn(
                name: "NumberOfViews",
                table: "News",
                newName: "NumbersOviews");

            migrationBuilder.RenameColumn(
                name: "NewsCategoryId",
                table: "News",
                newName: "NewsCategoriesId");

            migrationBuilder.RenameColumn(
                name: "Message",
                table: "Contact",
                newName: "Massage");
        }
    }
}
