using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using Appointment.Infrastructure.Configuration;
using CSharpFunctionalExtensions;
using MediatR;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.GCalendar.GetEvents
{
    public class GetEventsHandler : IRequestHandler<GetEventsQuery, Result<IEnumerable<CalendarEvent>, ResultError>>
    {
        private readonly IServiceAccountSingleton _serviceAccount;
        public GetEventsHandler(IServiceAccountSingleton serviceAccount)
        {
            _serviceAccount = serviceAccount;
        }

        public async Task<Result<IEnumerable<CalendarEvent>, ResultError>> Handle(GetEventsQuery request, CancellationToken cancellationToken)
        {
            var calendar = await _serviceAccount.GetEventsFromMonth(request.EventsFromDate.Year, request.EventsFromDate.Month);
            return Result.Success<IEnumerable<CalendarEvent>, ResultError>(calendar);
        }
    }
}
