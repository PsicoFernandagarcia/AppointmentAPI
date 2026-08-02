namespace Appointment.Domain.Infrastructure
{
    public class CalendarConfigOptions
    {
        public static readonly string SECTION = "CalendarConfig";
        public string[] CalendarIds { get; set; }
        public string ConfigPath { get; set; }
    }
}
