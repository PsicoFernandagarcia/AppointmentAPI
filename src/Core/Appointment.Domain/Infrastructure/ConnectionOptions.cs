namespace Appointment.Domain.Infrastructure
{
    public class ConnectionOptions
    {
        public static readonly string SECTION = "ConnectionStrings";
        public string Database { get; set; }
    }
}
