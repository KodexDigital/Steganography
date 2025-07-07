using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Anaconda.Migrations
{
    /// <inheritdoc />
    public partial class updated_model : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReferenceKey",
                table: "StegFiles",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferenceKey",
                table: "StegFiles");
        }
    }
}
