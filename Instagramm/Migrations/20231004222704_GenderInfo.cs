using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Instagramm.Migrations
{
    /// <inheritdoc />
    public partial class GenderInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GenderInfo",
                table: "AspNetUsers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GenderInfo",
                table: "AspNetUsers");
        }
    }
}
