namespace Appointment.Domain.Infrastructure
{
    public class CalendarConfigOptions
    {
        public static readonly string SECTION = "CalendarConfig";
        public string CalendarId { get; set; }
        public string ConfigPath { get; set; }
    }
}
