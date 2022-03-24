namespace Appointment.Application.AuthUseCases.Authenticate
{
    public class LoginResult
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public int Id { get; set; }
    }
}