using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Appointment.Host.Migrations
{
    public partial class addTimezoneOffsetToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TimezoneOffset",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimezoneOffset",
                table: "Users");
        }
    }
}
