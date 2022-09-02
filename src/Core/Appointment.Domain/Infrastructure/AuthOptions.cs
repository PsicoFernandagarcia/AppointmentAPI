namespace Appointment.Domain.Infrastructure
{
    public class AuthOptions
    {
        public static readonly string SECTION = "Auth";
        public string Secret { get; set; }
        public string HashValue { get; set; }
    }
}