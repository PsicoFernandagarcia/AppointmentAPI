namespace Appointment.Application.AuthUseCases.AuthenticateExternal
{
    public class LoginExternalResult
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public int Id { get; set; }
    }
}