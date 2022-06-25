using Appointment.Domain.Infrastructure;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.SendEmailUseCase.Reminder
{
    public class SendReminderEmailHandler : IRequestHandler<SendReminderEmailCommand, Result<bool, ResultError>>
    {
        
        private readonly EmailOptions _emailOptions;
        private readonly IEmailSender _emailSender;
        public SendReminderEmailHandler(IOptions<EmailOptions> emailOptions, IEmailSender emailSender)
        {
            _emailOptions = emailOptions.Value;
            _emailSender = emailSender;
        }
        public async Task<Result<bool, ResultError>> Handle(SendReminderEmailCommand request, CancellationToken cancellationToken)
        {
            if(!request.Appointments.Any()) return true;
            var sb = new StringBuilder();
            foreach(var appointment in request.Appointments)
            {
                var dateFromUtcCalendar = appointment.DateFrom.ToUniversalTime().ToString("yyyyMMddTHHmm00Z");
                var dateToUtcCalendar = appointment.DateFrom.AddHours(1).ToUniversalTime().ToString("yyyyMMddTHHmm00Z");
                var calendarHostDescription = $"{appointment.Patient.Name.Replace(" ", "%20")}%20{appointment.Patient.LastName.Replace(" ", "%20")}%20{appointment.Patient.Email.Replace(" ", "%20")}";
                sb.AppendLine($@"
                    <tr>
                        <td>{appointment.Patient.LastName} {appointment.Patient.Name}</td>
                        <td>{appointment.DateFrom.ToUniversalTime().AddMinutes(appointment.Host.TimezoneOffset).ToString("HH:mm")}</td>
                        <td>
                            <a href=""https://calendar.google.com/calendar/render?action=TEMPLATE&dates={dateFromUtcCalendar}%2F{dateToUtcCalendar}&text={calendarHostDescription}""
                                target = ""_blank""
                                style = ""color: #122a4b; text-decoration: underline;"" >
                                Agregar a Google Calendar
                            </ a >
                        </ td >
                    </ tr >
                    ");
            }


            var reminderBody = await File.ReadAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(), "Content/appointments_reminder.html"), cancellationToken);
            reminderBody = reminderBody.Replace("#_name_#", request.HostName)
                        .Replace("#_appointments_#", sb.ToString());
                       

            return this._emailSender.Send(request.HostEmail, $"Mis citas de hoy {DateTime.Now.ToString("dd-MM-yyyy")}", reminderBody, true);
        }
    }
    public class SendReminderEmailCommand : IRequest<Result<bool, ResultError>>
    {

        public string HostName { get; set; }
        public string HostEmail { get; set; }
        public IEnumerable<Domain.Entities.Appointment> Appointments{ get; set; }

    }
}
