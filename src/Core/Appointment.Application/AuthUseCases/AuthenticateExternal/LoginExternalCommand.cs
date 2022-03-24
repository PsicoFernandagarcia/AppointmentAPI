using System.ComponentModel.DataAnnotations;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;

namespace Appointment.Application.AuthUseCases.AuthenticateExternal
{
    public class LoginExternalCommand : IRequest<Result<LoginExternalResult, UnauthorizedError>>
    {
        [Required]
        public string Email { get; set; }
        public string  FirstName { get; set; }
        public string LastName { get; set; }
        public string IdToken { get; set; }
        public string Provider { get; set; }
        public int TimezoneOffset { get; set; }
    }
}
