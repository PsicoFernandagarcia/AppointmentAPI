using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Appointment.Application.AuthUseCases.AuthenticateExternal
{
    public class LoginExternalValidator:AbstractValidator<LoginExternalCommand>
    {
        public LoginExternalValidator()
        {
            RuleFor(x => x.Email).NotEmpty().NotNull().WithMessage("Email not valid");
            RuleFor(x => x.IdToken).NotEmpty().NotNull().WithMessage("Token not valid");
            RuleFor(x => x.FirstName).NotEmpty().NotNull().WithMessage("Name not valid");
            RuleFor(x => x.LastName).NotEmpty().NotNull().WithMessage("LastName not valid");
        }
    }
}
