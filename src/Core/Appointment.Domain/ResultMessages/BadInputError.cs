namespace Appointment.Domain.ResultMessages
{
    public class BadInputError : ResultError
    {
        public BadInputError(string message) : base(message)
        {
        }
    }
}