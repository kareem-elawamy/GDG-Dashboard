using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GDG_DashBoard.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProfileContactAndProjectPeriod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "UserProfiles",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "UserProfiles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Period",
                table: "Projects",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Period",
                table: "Projects");
        }
    }
}
