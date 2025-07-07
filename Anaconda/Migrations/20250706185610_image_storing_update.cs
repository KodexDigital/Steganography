using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Anaconda.Migrations
{
    /// <inheritdoc />
    public partial class image_storing_update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DecodeCount",
                table: "UserStats");

            migrationBuilder.RenameColumn(
                name: "EncodeCount",
                table: "UserStats",
                newName: "Action");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Action",
                table: "UserStats",
                newName: "EncodeCount");

            migrationBuilder.AddColumn<int>(
                name: "DecodeCount",
                table: "UserStats",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
