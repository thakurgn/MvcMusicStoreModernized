using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcMusicStoreModernized.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderUsername : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Orders",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Username",
                table: "Orders");
        }
    }
}
