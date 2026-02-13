using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GardenApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Plants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartMonth = table.Column<int>(type: "int", nullable: false),
                    EndMonth = table.Column<int>(type: "int", nullable: false),
                    Instructions = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plants", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Plants",
                columns: new[] { "Id", "EndMonth", "Instructions", "Name", "StartMonth" },
                values: new object[,]
                {
                    { 1, 5, "Use special fertilizer.", "Eggplant", 4 },
                    { 2, 3, "Use deep planters.", "Carrot", 2 },
                    { 3, 11, "Use special fertilizer.", "Tulip", 9 },
                    { 4, 11, "Use special fertilizer.", "Daffodils", 9 },
                    { 5, 11, "Use big planters.", "Garlic", 10 },
                    { 6, 11, "Use big planters.", "Onions", 10 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Plants");
        }
    }
}
