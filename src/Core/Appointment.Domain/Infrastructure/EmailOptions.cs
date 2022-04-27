namespace Appointment.Domain.Infrastructure
{
    public class EmailOptions
    {
        public static readonly string SECTION = "Email";
        public string GmailUsername { get; set; }
        public string GmailPassword { get; set; }
        public string GmailHost { get; set; }
        public int    GmailPort { get; set; }
        public bool   GmailSSL { get; set; }
        public bool SendEmail { get; set; }
        public bool SendEmailToUsers { get; set; }
    }
}