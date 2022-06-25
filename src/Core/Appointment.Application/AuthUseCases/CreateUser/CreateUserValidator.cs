using FluentValidation;

namespace Appointment.Application.AuthUseCases.CreateUser
{
    public class CreateUserValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.Email).NotEmpty().NotNull().WithMessage("Email not valid");
            RuleFor(x => x.LastName).NotEmpty().NotNull().WithMessage("Last name not valid");
            RuleFor(x => x.Name).NotEmpty().NotNull().WithMessage("First name not valid");
            RuleFor(x => x.Password).NotEmpty().NotNull().WithMessage("Password not valid");
        }
    }
}
