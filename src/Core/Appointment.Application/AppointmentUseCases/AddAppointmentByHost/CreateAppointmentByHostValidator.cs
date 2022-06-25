using FluentValidation;

namespace Appointment.Application.AppointmentUseCases.AddAppointmentByHost
{
    public class CreateAppointmentByHostValidator : AbstractValidator<CreateAppointmentByHostCommand>
    {
        public CreateAppointmentByHostValidator()
        {
            RuleFor(x => x.Title).NotEmpty().NotNull().WithMessage("Title not valid");
            RuleFor(x => x.PatientEmail).NotEmpty().NotNull().WithMessage("With not valid");
            RuleFor(x => x.DateTo).GreaterThan(x => x.DateFrom).WithMessage("Date from should be lower than date to");
            RuleFor(x => x.HostId).GreaterThan(0).WithMessage("Host Id not valid");
            RuleFor(x => x.PatientId).GreaterThan(-1).WithMessage("Patient Id not valid");
        }
    }
}
