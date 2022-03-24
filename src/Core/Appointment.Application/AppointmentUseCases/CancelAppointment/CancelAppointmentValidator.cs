using FluentValidation;

namespace Appointment.Application.AppointmentUseCases.CancelAppointment
{
    public class CancelAppointmentQueryValidator : AbstractValidator<CancelAppointmentsCommand>
    {
        public CancelAppointmentQueryValidator()
        {
        }
    }
}
