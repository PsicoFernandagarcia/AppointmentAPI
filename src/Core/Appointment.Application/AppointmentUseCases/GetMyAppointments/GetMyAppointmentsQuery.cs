using Appointment.Domain.Entities;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System;
using System.Collections.Generic;

namespace Appointment.Application.AppointmentUseCases.GetMyAppointment
{
    public class GetMyAppointmentsQuery : IRequest<Result<IEnumerable<AppointmentDto>, ResultError>>
    {
        public int UserId { get; set; }
        public DateTime DateFrom { get; set; }
        public GetMyAppointmentsQuery()
        {

        }

        public GetMyAppointmentsQuery(int userId, DateTime dateFrom)
        {
            UserId = userId;
            DateFrom = dateFrom;
        }
    }
}
