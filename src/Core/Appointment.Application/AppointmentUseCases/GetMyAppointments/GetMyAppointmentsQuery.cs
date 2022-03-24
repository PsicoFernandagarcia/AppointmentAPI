using System;
using System.Collections.Generic;
using Appointment.Domain.Entities;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;

namespace Appointment.Application.AppointmentUseCases.GetMyAppointment
{
    public class GetMyAppointmentsQuery : IRequest<Result<IEnumerable<AppointmentDto>,ResultError>>
    {
        public int UserId { get; set; }
        public DateTime DateFrom { get; set; }
    }
}
