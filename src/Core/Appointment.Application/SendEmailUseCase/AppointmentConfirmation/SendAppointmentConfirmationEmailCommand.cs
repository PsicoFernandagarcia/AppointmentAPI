using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;

namespace Appointment.Application.SendEmailUseCase.AppointmentConfirmation
{
    public class SendAppointmentConfirmationEmailHandler: IRequestHandler<SendAppointmentConfirmationEmailCommand, Result<bool, ResultError>>
    {
        private readonly IEmailSender _emailSender;
        private readonly IUserRepository _userRepository;
        public SendAppointmentConfirmationEmailHandler(IEmailSender emailSender, IUserRepository userRepository)
        {
            _emailSender = emailSender;
            _userRepository = userRepository;
        }
        public async Task<Result<bool, ResultError>> Handle(SendAppointmentConfirmationEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserById(request.UserId);
            var host = await _userRepository.GetUserById(request.HostId);
            var userDate = request.DateTimeInUTC.AddMinutes(user.TimezoneOffset);
            var hostDate = request.DateTimeInUTC.AddMinutes(host.TimezoneOffset);

            var ci = new CultureInfo("es-ES");
            var body = await File.ReadAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(), "Content/appointment_confirmation.html"),cancellationToken);
            body = body.Replace("#_name_#", user.Name)
                        .Replace("#_visibleDate_#", userDate.ToString("dddd, dd MMMM yyyy HH:mm", ci))
                        .Replace("#_dateFrom_#", $"{request.DateTimeInUTC.ToString("MMddyyyyTHHmm00Z")}")
                        .Replace("#_dateTo_#", $"{request.DateTimeInUTC.AddHours(1).ToString("MMddyyyyTHHmm00Z")}");
            return this._emailSender.Send(user.Email, "Confirmación cita", body, true);
        }
    }
    public class SendAppointmentConfirmationEmailCommand: IRequest<Result<bool, ResultError>>
    {

        public int UserId { get; set; }
        public int HostId { get; set; }
        public DateTime DateTimeInUTC { get; set; }

    }
}
