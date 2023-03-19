using Appointment.Domain.Entities;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System;
using System.Collections.Generic;

namespace Appointment.Application.AppointmentUseCases.GetLastAppointment
{
    public class GetLastAppointmentsQuery : IRequest<Result<IEnumerable<AppointmentDto>, ResultError>>
    {
        public int PatientId { get; set; }
        public int HostId { get; set; }
        public int TotalCount { get; set; }
    }
}
