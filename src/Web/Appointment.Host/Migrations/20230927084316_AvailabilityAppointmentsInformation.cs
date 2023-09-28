using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Appointment.Host.Migrations
{
    public partial class AvailabilityAppointmentsInformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppointmentId",
                table: "Availabilities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AppointmentWith",
                table: "Availabilities",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppointmentId",
                table: "Availabilities");

            migrationBuilder.DropColumn(
                name: "AppointmentWith",
                table: "Availabilities");
        }
    }
}
