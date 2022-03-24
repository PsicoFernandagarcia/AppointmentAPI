using FluentValidation;

namespace Appointment.Application.AppointmentUseCases.GetMyAppointment
{
    public class GetMyAppointmentQueryValidator : AbstractValidator<GetMyAppointmentsQuery>
    {
        public GetMyAppointmentQueryValidator()
        {
        }
    }
}
