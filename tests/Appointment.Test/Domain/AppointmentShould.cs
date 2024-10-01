using Appointment.Domain;
using FluentAssertions;
using Xunit;
using Entities = Appointment.Domain.Entities;

namespace Appointment.Test.Domain
{
    public class AppointmentShould
    {

        [InlineData(1, "New Appointment", "3022/10/25 10:00", "3022/10/25 11:00", "Joaquin Ferroni", 1, "", false, 1, 10, AppointmentStatus.CREATED)]
        [InlineData(0, "New Appointment", "3022/10/25 10:00", "3022/10/25 11:00", "Joaquin Ferroni", 1, "", false, 1, 10, AppointmentStatus.CREATED)]
        [InlineData(0, "New Appointment", "1022/10/25 10:00", "1022/10/25 11:00", "Joaquin Ferroni", 1, "", false, 1, 10, AppointmentStatus.UPDATED)]
        [Theory]
        public void Be_Created_Because_Of_Valid_Properties(int id, string title, DateTime dateFrom, DateTime dateTo, string with,
            int createdById, string color, bool isDeleted, int hostId, int patientId, AppointmentStatus status)
        {
            var appointmentResult = Entities.Appointment.Create(id, title, dateFrom, dateTo, with,
                                                        createdById, color, isDeleted, hostId, patientId, status, DateTime.Now,null);
            appointmentResult.IsSuccess.Should().BeTrue();
            appointmentResult.Value.Should().BeOfType<Entities.Appointment>();
        }

        [InlineData(-1, "New Appointment", "3022/10/25 10:00", "3022/10/25 11:00", "Joaquin Ferroni", 1, "", false, 1, 10, AppointmentStatus.CREATED)]
        [InlineData(0, "", "3022/10/25 10:00", "3022/10/25 11:00", "Joaquin Ferroni", 1, "", false, 1, 10, AppointmentStatus.CREATED)]
        [InlineData(0, null, "3022/10/25 10:00", "3022/10/25 11:00", "Joaquin Ferroni", 1, "", false, 1, 10, AppointmentStatus.CREATED)]
        [InlineData(0, "New Appointment", "1022/10/25 10:00", "1022/10/25 11:00", "", 1, "", false, 1, 10, AppointmentStatus.UPDATED)]
        [InlineData(0, "New Appointment", "1022/10/25 10:00", "1022/10/25 11:00", null, 1, "", false, 1, 10, AppointmentStatus.UPDATED)]
        [InlineData(0, "New Appointment", "1022/10/25 10:00", "1022/10/25 11:00", null, -1, "", false, 1, 10, AppointmentStatus.UPDATED)]
        [InlineData(0, "New Appointment", "1022/10/25 10:00", "1022/10/25 11:00", null, 0, "", false, 1, 10, AppointmentStatus.UPDATED)]
        [InlineData(0, "New Appointment", "1022/10/25 10:00", "1022/10/25 11:00", null, 0, "", false, -1, 10, AppointmentStatus.UPDATED)]
        [InlineData(0, "New Appointment", "1022/10/25 10:00", "1022/10/25 11:00", null, 0, "", false, 0, 10, AppointmentStatus.UPDATED)]
        [InlineData(0, "New Appointment", "1022/10/25 10:00", "1022/10/25 11:00", null, 0, "", false, 1, -1, AppointmentStatus.UPDATED)]
        [InlineData(0, "New Appointment", "1022/10/25 10:00", "1022/10/25 11:00", null, 0, "", false, 1, 0, AppointmentStatus.UPDATED)]
        [InlineData(0, "New Appointment", "1022/10/25 10:00", "1022/10/25 09:00", null, 0, "", false, 1, 0, AppointmentStatus.UPDATED)]
        [Theory]
        public void Not_Be_Created_Because_Of_Inalid_Properties(int id, string title, DateTime dateFrom, DateTime dateTo, string with,
            int createdById, string color, bool isDeleted, int hostId, int patientId, AppointmentStatus status)
        {
            var appointmentResult = Entities.Appointment.Create(id, title, dateFrom, dateTo, with,
                                                        createdById, color, isDeleted, hostId, patientId, status, DateTime.Now, null);
            appointmentResult.IsSuccess.Should().BeFalse();
            appointmentResult.Error.Should().NotBeNullOrWhiteSpace();
        }

    }
}
