using Appointment.Application.SendEmailUseCase.Reminder;
using Appointment.Domain.Entities;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Appointment.Application.SendEmailUseCase.EndOfMonthMissingPayments
{
    public record MissingPayment(string FullName, DateTime AppointmentDate);
    public class SendMissingPaymentsEmailCommand : IRequest<Result<bool, ResultError>>
    {
        public IEnumerable<MissingPayment> MissingPayments;
        public User Host { get; set; }
    }

    public class SendMissingPaymentsEmailHandler : IRequestHandler<SendMissingPaymentsEmailCommand, Result<bool, ResultError>>
    {
        private readonly IEmailSender _emailSender;
        public SendMissingPaymentsEmailHandler(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task<Result<bool, ResultError>> Handle(SendMissingPaymentsEmailCommand request, CancellationToken cancellationToken)
        {
            if (!request.MissingPayments.Any()) return true;
            var sb = new StringBuilder();
            var payments = request.MissingPayments.GroupBy(x => x.FullName).Select(g => new {FullName = g.Key, Dates = g.Select(x => x.AppointmentDate) });
            foreach (var mp in payments)
            {

                sb.AppendLine($@"
                    <tr>
                        <td><strong>{mp.FullName}</strong></td>
                        <td>
                    ");
                foreach(var date in mp.Dates)
                {
                    sb.AppendLine($"{date.ToUniversalTime().AddMinutes(request.Host.TimezoneOffset).ToString("dd-MM-yyyy HH:mm")}<br>");
                }
                sb.AppendLine("</td></tr>");
            }


            var missingPaymentsBody = await File.ReadAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(), "Content/missing_payments_reminder.html"), cancellationToken);
            missingPaymentsBody = missingPaymentsBody.Replace("#_name_#", request.Host.Name)
                                    .Replace("#_appointments_#", sb.ToString());


            return this._emailSender.Send(request.Host.Email, $"Mis cobros pendientes {DateTime.Now.ToString("dd-MM-yyyy")}", missingPaymentsBody, true, true);
        }
    }
}
