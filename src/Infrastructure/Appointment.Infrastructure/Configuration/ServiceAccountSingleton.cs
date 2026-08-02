using Appointment.Domain.Infrastructure;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Appointment.Infrastructure.Configuration
{
    public interface IServiceAccountSingleton
    {
        Task<IEnumerable<CalendarEvent>> GetEventsFromMonth(int year, int month);
        Task<IEnumerable<Event>> GetNext30DaysEvents();
        Task UpdateEvent(Event e);
    }
    public class ServiceAccountSingleton : IServiceAccountSingleton
    {
        private readonly ServiceAccountCredential _serviceAccountCredential;
        private readonly CalendarService _calendarService;
        private readonly CalendarConfigOptions _calendarConfig;

        public ServiceAccountSingleton(IOptions<CalendarConfigOptions> calendarOptions)
        {
            _calendarConfig = calendarOptions.Value;

            string[] Scopes = {
                CalendarService.Scope.Calendar,
                CalendarService.Scope.CalendarEvents,
                CalendarService.Scope.CalendarReadonly,
                CalendarService.Scope.CalendarSettingsReadonly,
            };

            using (var stream =
                new FileStream(_calendarConfig.ConfigPath, FileMode.Open, FileAccess.Read))
            {
                var confg = Google.Apis.Json.NewtonsoftJsonSerializer.Instance.Deserialize<JsonCredentialParameters>(stream);
                _serviceAccountCredential = new ServiceAccountCredential(
                   new ServiceAccountCredential.Initializer(confg.ClientEmail)
                   {
                       Scopes = Scopes
                   }.FromPrivateKey(confg.PrivateKey));
            }
            _calendarService = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = _serviceAccountCredential,
                ApplicationName = "Appointment",
            });

        }


        public async Task<IEnumerable<CalendarEvent>> GetEventsFromMonth(int year, int month)
        {
            List<Event> events = [];
            foreach (var calendarId in _calendarConfig.CalendarIds)
            {
                var listRequest = _calendarService.Events.List(calendarId);
                listRequest.TimeMin = new DateTime(year, month, 1);
                listRequest.TimeMax = new DateTime(year, month, DateTime.DaysInMonth(year: year, month: month));
                listRequest.ShowDeleted = false;
                listRequest.SingleEvents = true;
                listRequest.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
                Events e = await listRequest.ExecuteAsync();
                events.AddRange(e.Items);
            }

            return events.Where(i => i.Start.DateTime.HasValue)
                               .Select(i => new CalendarEvent(i.Summary, i.Start.DateTime.Value.ToUniversalTime(), i.ColorId))
                               .OrderBy(i => i.date)
                               .AsEnumerable();
        }

        public async Task<IEnumerable<Event>> GetNext30DaysEvents()
        {
            List<Event> events = [];
            foreach (var calendarId in _calendarConfig.CalendarIds)
            {
                var listRequest = _calendarService.Events.List(calendarId);
                listRequest.TimeMin = DateTime.Now;
                listRequest.TimeMax = DateTime.Now.AddDays(30);
                listRequest.ShowDeleted = false;
                listRequest.SingleEvents = true;
                listRequest.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
                Events e = await listRequest.ExecuteAsync();
                events.AddRange(e.Items);
            }

            return events.Where(i => i.Start.DateTime.HasValue)
                               .OrderBy(i => i.Start.DateTime)
                               .AsEnumerable(); ;
        }

        public async Task CreateEvent()
        {
            var @event = new Event
            {
                Summary = "test created from api",
                Location = "Psicofer Online",
                Description = "This is a test event from api",
                Start = new EventDateTime
                {
                    DateTime = DateTime.Now.AddDays(1)
                },
                End = new EventDateTime
                {
                    DateTime = DateTime.Now.AddDays(1).AddHours(1)
                },
                //ETag = "PsicoFer",
            };
            var result = _calendarService.Events.Insert(@event, _calendarConfig.CalendarIds.Where(id => !id.Contains("family")).FirstOrDefault());
            result.SendNotifications = true;
            await result.ExecuteAsync();
        }

        public async Task UpdateEvent(Event e)
        {
            var request = _calendarService.Events.Update(e, _calendarConfig.CalendarIds.Where(id => !id.Contains("family")).FirstOrDefault(), e.Id);
            await request.ExecuteAsync();
        }

    }



    public record CalendarEvent(string Description, DateTime date, string Color);
}
