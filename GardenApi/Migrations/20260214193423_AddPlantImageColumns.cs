using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GardenApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPlantImageColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageFileName",
                table: "Plants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isImageApproved",
                table: "Plants",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Plants",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ImageFileName", "isImageApproved" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Plants",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ImageFileName", "isImageApproved" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Plants",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ImageFileName", "isImageApproved" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Plants",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ImageFileName", "isImageApproved" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Plants",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ImageFileName", "isImageApproved" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Plants",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ImageFileName", "isImageApproved" },
                values: new object[] { null, false });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageFileName",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "isImageApproved",
                table: "Plants");
        }
    }
}
