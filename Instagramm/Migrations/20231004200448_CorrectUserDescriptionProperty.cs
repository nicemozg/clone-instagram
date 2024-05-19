using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Instagramm.Migrations
{
    /// <inheritdoc />
    public partial class CorrectUserDescriptionProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Descrition",
                table: "AspNetUsers",
                newName: "Description");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "AspNetUsers",
                newName: "Descrition");
        }
    }
}
