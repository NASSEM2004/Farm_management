using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farm_management.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixHatcheryCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Hatcheries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaleAnimalId = table.Column<int>(type: "int", nullable: false),
                    FemaleAnimalId = table.Column<int>(type: "int", nullable: false),
                    ProductionBarnId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpectedEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hatcheries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hatcheries_Animals_FemaleAnimalId",
                        column: x => x.FemaleAnimalId,
                        principalTable: "Animals",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Hatcheries_Animals_MaleAnimalId",
                        column: x => x.MaleAnimalId,
                        principalTable: "Animals",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Hatcheries_Barns_ProductionBarnId",
                        column: x => x.ProductionBarnId,
                        principalTable: "Barns",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Hatcheries_FemaleAnimalId",
                table: "Hatcheries",
                column: "FemaleAnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_Hatcheries_MaleAnimalId",
                table: "Hatcheries",
                column: "MaleAnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_Hatcheries_ProductionBarnId",
                table: "Hatcheries",
                column: "ProductionBarnId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Hatcheries");
        }
    }
}
