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
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Infrastructure.Configuration
{
    public interface IServiceAccountSingleton
    {
        Task<IEnumerable<CalendarEvent>> GetEventsFromMonth(int year, int month);
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
                CalendarService.Scope.CalendarEvents
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
           // await CreateEvent();
            var listRequest = _calendarService.Events.List(_calendarConfig.CalendarId);
            listRequest.TimeMin = new DateTime(year, month, 1);
            listRequest.TimeMax = new DateTime(year, month, DateTime.DaysInMonth(year: year, month: month));
            listRequest.ShowDeleted = false;
            listRequest.SingleEvents = true;
            listRequest.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
            Events events = await listRequest.ExecuteAsync();

            return events.Items.Where(i => i.Start.DateTime.HasValue)
                               .Select(i => new CalendarEvent(i.Summary, i.Start.DateTime.Value.ToUniversalTime(), i.ColorId))
                               .AsEnumerable();
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
            var result = _calendarService.Events.Insert(@event, _calendarConfig.CalendarId);
            result.SendNotifications = true;
            await result.ExecuteAsync();
        }

    }



    public record CalendarEvent(string Description, DateTime? date, string Color);
}
