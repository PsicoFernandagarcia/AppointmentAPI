using System;
using System.Collections.Generic;
using Appointment.Domain.Entities;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;

namespace Appointment.Application.AppointmentUseCases.GetAppointmentsByFilter
{
    public class GetAppointmentsByFilterQuery : IRequest<Result<IEnumerable<AppointmentDto>,ResultError>>
    {
        public int Year { get; set; }
        public int UserId { get; set; }
    }
}
