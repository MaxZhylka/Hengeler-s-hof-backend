using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hengeler.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomsManagment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RoomId",
                table: "Slides",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NameKey = table.Column<Guid>(type: "uuid", nullable: false),
                    DescriptionKey = table.Column<Guid>(type: "uuid", nullable: false),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    AdditionalPrice = table.Column<int>(type: "integer", nullable: false),
                    CheckIn = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    CheckOut = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    MaxGuestsKey = table.Column<Guid>(type: "uuid", nullable: false),
                    Size = table.Column<int>(type: "integer", nullable: false),
                    SlideIds = table.Column<List<Guid>>(type: "uuid[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Slides_RoomId",
                table: "Slides",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Slides_Rooms_RoomId",
                table: "Slides",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Slides_Rooms_RoomId",
                table: "Slides");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Slides_RoomId",
                table: "Slides");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "Slides");
        }
    }
}
