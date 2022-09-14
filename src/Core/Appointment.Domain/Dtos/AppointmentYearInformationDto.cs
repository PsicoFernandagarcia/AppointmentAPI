namespace Appointment.Domain.Entities
{
    public class AppointmentYearInformationDto
    {
        public int Month { get; set; }
        public int TotalAppointments { get; set; }
        public int TotalCanceled { get; set; }
    }
}