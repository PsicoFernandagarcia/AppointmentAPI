using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Appointment.Application.AuthUseCases.Authenticate
{
    public class LoginCommand : IRequest<Result<LoginResult, UnauthorizedError>>
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }

        public int TimezoneOffset { get; set; }
    }
}
