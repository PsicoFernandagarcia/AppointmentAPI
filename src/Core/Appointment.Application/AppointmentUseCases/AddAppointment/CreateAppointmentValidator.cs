using Appointment.Application.AuthUseCases.CreateUser;
using FluentValidation;

namespace Appointment.Application.AppointmentUseCases.AddAppointment
{
    public class CreateAppointmentValidator:AbstractValidator<CreateAppointmentCommand>
    {
        public CreateAppointmentValidator()
        {
            RuleFor(x => x.Title).NotEmpty().NotNull().WithMessage("Title not valid");
            RuleFor(x => x.With).NotEmpty().NotNull().WithMessage("With not valid");
            RuleFor(x => x.DateTo).GreaterThan(x=> x.DateFrom).WithMessage("Date from should be lower than date to");
            RuleFor(x => x.CreatedById).GreaterThan(0).WithMessage("UserId not valid");
            RuleFor(x => x.HostId).GreaterThan(0).WithMessage("Host Id not valid");
            RuleFor(x => x.PatientId).GreaterThan(0).WithMessage("Patient Id not valid");
        }
    }
}
