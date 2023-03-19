using FluentValidation;

namespace Appointment.Application.AppointmentUseCases.GetLastAppointment
{
    public class GetLastAppointmentQueryValidator : AbstractValidator<GetLastAppointmentsQuery>
    {
        public GetLastAppointmentQueryValidator()
        {
        }
    }
}
