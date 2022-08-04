using FluentValidation;

namespace Appointment.Application.PaymentUseCases.AddPayment
{
    public class CreatePaymentValidator : AbstractValidator<CreatePaymentCommand>
    {
        public CreatePaymentValidator()
        {
            RuleFor(x => x.PatientId).GreaterThan(0).WithMessage("Patient not valid");
            RuleFor(x => x.HostId).GreaterThan(0).WithMessage("Host not valid");
            RuleFor(x => x.SessionsPaid).GreaterThan(0).WithMessage("Sessions not valid");
        }
    }
}
