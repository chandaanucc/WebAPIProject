using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace villaa.Migrations
{
    /// <inheritdoc />
    public partial class First123 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VideoId",
                table: "MediaUploads");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "MediaUploads",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "MediaUploads");

            migrationBuilder.AddColumn<int>(
                name: "VideoId",
                table: "MediaUploads",
                type: "int",
                nullable: true);
        }
    }
}
