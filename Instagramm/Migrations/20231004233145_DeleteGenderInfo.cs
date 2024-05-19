using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Instagramm.Migrations
{
    /// <inheritdoc />
    public partial class DeleteGenderInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GenderInfo",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GenderInfo",
                table: "AspNetUsers",
                type: "text",
                nullable: true);
        }
    }
}
