using Appointment.Domain;
using Appointment.Domain.Entities;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appointment.Application.UsersUseCase.GetUserByRole
{
    public class GetUserByRoleQuery : IRequest<Result<IEnumerable<User>, ResultError>>
    {
        public RolesEnum Role { get; set; }

        public GetUserByRoleQuery()
        {

        }
        
        public GetUserByRoleQuery(RolesEnum role)
        {
            Role = role;
        }
    }
}
