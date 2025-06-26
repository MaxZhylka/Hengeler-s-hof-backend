using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hengeler.Migrations
{
    /// <inheritdoc />
    public partial class AddSlides : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Slides",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    TitleKey = table.Column<Guid>(type: "uuid", nullable: false),
                    DescriptionKey = table.Column<Guid>(type: "uuid", nullable: false),
                    Price = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Slides", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Slides");
        }
    }
}
