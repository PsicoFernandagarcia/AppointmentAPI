using Appointment.Domain;
using Appointment.Domain.Entities;
using Appointment.Domain.ResultMessages;
using Appointment.Infrastructure.Configuration;
using CSharpFunctionalExtensions;
using MediatR;
using System;
using System.Collections.Generic;

namespace Appointment.Application.GCalendar.GetEvents
{
    public class GetEventsQuery : IRequest<Result<IEnumerable<CalendarEvent>, ResultError>>
    {
        public DateTime EventsFromDate { get; set; }
    }
}
