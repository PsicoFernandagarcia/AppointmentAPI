using Appointment.Domain.Entities;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System.Collections.Generic;

namespace Appointment.Application.AppointmentUseCases.GetAppointmentsByFilter
{
    public class GetAppointmentsByFilterQuery : IRequest<Result<IEnumerable<AppointmentDto>, ResultError>>
    {
        public int Year { get; set; }
        public int UserId { get; set; }
    }
}
