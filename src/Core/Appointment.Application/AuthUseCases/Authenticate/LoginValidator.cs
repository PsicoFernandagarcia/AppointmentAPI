using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Appointment.Application.AuthUseCases.Authenticate
{
    public class LoginValidator:AbstractValidator<LoginCommand>
    {
        public LoginValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().NotNull().WithMessage("User not valid");
            RuleFor(x => x.Password).NotEmpty().NotNull().WithMessage("Password not valid");
        }
    }
}
