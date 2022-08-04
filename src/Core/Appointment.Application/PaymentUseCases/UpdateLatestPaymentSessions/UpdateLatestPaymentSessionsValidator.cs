using FluentValidation;

namespace Appointment.Application.PaymentUseCases.UpdateLatestPaymentSessions
{
    public class UpdateLatestPaymentSessionsValidator : AbstractValidator<UpdateLatestPaymentSessionsCommand>
    {
        public UpdateLatestPaymentSessionsValidator()
        {
            RuleFor(x => x.PatientId).GreaterThan(0).WithMessage("Patient not valid");
            RuleFor(x => x.HostId).GreaterThan(0).WithMessage("Host not valid");
        }
    }
}
