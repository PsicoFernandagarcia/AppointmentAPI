using Appointment.Domain.Infrastructure;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Appointment.Application.SendEmailUseCase.AppointmentConfirmation
{
    public class SendAppointmentConfirmationEmailHandler : IRequestHandler<SendAppointmentConfirmationEmailCommand, Result<bool, ResultError>>
    {
        private readonly IEmailSender _emailSender;
        private readonly IUserRepository _userRepository;
        private readonly EmailOptions _emailOptions;
        public SendAppointmentConfirmationEmailHandler(IEmailSender emailSender, IUserRepository userRepository, IOptions<EmailOptions> emailOptions)
        {
            _emailSender = emailSender;
            _userRepository = userRepository;
            _emailOptions = emailOptions.Value;
        }
        public async Task<Result<bool, ResultError>> Handle(SendAppointmentConfirmationEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserById(request.UserId);
            var host = await _userRepository.GetUserById(request.HostId);
            var userDate = request.DateTimeInUTC.AddMinutes(user.TimezoneOffset);
            var hostDate = request.DateTimeInUTC.AddMinutes(host.TimezoneOffset);
            var ci = CultureInfo.GetCultureInfo("es-ES");

            var userBody = await File.ReadAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(), "Content/appointment_confirmation_user.html"), cancellationToken);
            userBody = userBody.Replace("#_name_#", user.Name)
                        .Replace("#_visibleDate_#", userDate.ToString("dddd, dd MMMM yyyy HH:mm", ci))
                        .Replace("#_dateFrom_#", $"{request.DateTimeInUTC.ToString("yyyyMMddTHHmm00Z")}")
                        .Replace("#_dateTo_#", $"{request.DateTimeInUTC.AddHours(1).ToString("yyyyMMddTHHmm00Z")}");
            if (this._emailOptions.SendEmailToUsers)
                this._emailSender.Send(user.Email, "Confirmación cita", userBody, true);

            var hostTitle = HttpUtility.UrlEncode($"{user.Name} { user.LastName} { user.Email}");
            var hostBody = await File.ReadAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(), "Content/appointment_confirmation.html"), cancellationToken);
            hostBody = hostBody.Replace("#_name_#", host.Name)
                        .Replace("#_visibleDate_#", hostDate.ToString("dddd, dd MMMM yyyy HH:mm", ci))
                        .Replace("#_userName_#", $"{user.Name} {user.LastName}")
                        .Replace("#_calendarHostTitle_#", $"{hostTitle}")
                        .Replace("#_userEmail_#", user.Email)
                        .Replace("#_dateFrom_#", $"{request.DateTimeInUTC.ToString("yyyyMMddTHHmm00Z")}")
                        .Replace("#_dateTo_#", $"{request.DateTimeInUTC.AddHours(1).ToString("yyyyMMddTHHmm00Z")}");


            return this._emailSender.Send(host.Email, "Confirmación cita", hostBody, true);
        }
    }
}
