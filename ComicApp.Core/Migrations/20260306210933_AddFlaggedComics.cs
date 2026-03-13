using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComicApp.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddFlaggedComics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
            name: "FlaggedComics",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
            .Annotation("SqlServer:Identity", "1, 1"),
                ComicId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                FlaggedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                FlaggedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FlaggedComics", x => x.Id);
            });
        }


    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlaggedComics");
        }
    }


}
