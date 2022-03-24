using Appointment.Domain.Entities;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;

namespace Appointment.Application.AuthUseCases.CreateUser
{
    public class CreateUserCommand: IRequest<Result<User,ResultError>>
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsExternal { get; set; }
        public int TimezoneOffset { get; set; }
    }
}
