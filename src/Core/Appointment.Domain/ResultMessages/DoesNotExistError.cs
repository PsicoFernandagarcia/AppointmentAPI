namespace Appointment.Domain.ResultMessages
{
    public class DoesNotExistError : ResultError
    {
        public DoesNotExistError(string message) : base(message)
        {
        }
    }
}
