using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farm_management.Data.Migrations
{
    /// <inheritdoc />
    public partial class m4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Feeding",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BarnId = table.Column<int>(type: "int", nullable: false),
                    AnimalsCount = table.Column<int>(type: "int", nullable: false),
                    FoodType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FoodQuantity = table.Column<double>(type: "float", nullable: false),
                    FeedingTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feeding", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Feeding_Barns_BarnId",
                        column: x => x.BarnId,
                        principalTable: "Barns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Feeding_BarnId",
                table: "Feeding",
                column: "BarnId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Feeding");
        }
    }
}
