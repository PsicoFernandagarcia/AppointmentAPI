namespace Appointment.Domain.ResultMessages
{
    public class UnauthorizedError : ResultError
    {
        public UnauthorizedError(string message) : base(message)
        {
        }
    }
}