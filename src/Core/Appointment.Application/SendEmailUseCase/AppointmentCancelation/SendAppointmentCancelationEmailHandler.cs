using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.SendEmailUseCase.AppointmentCancelation
{
    public class SendAppointmentCancelationEmailHandler : IRequestHandler<SendAppointmentCancelationEmailCommand, Result<bool, ResultError>>
    {
        private readonly IEmailSender _emailSender;
        private readonly IUserRepository _userRepository;
        public SendAppointmentCancelationEmailHandler(IEmailSender emailSender, IUserRepository userRepository)
        {
            _emailSender = emailSender;
            _userRepository = userRepository;
        }
        public async Task<Result<bool, ResultError>> Handle(SendAppointmentCancelationEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserById(request.UserId);
            var host = await _userRepository.GetUserById(request.HostId);
            var hostDate = request.DateTimeInUTC.AddMinutes(host.TimezoneOffset);

            var ci = CultureInfo.GetCultureInfo("es-ES");
            var hostBody = await File.ReadAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(), "Content/appointment_cancelation.html"), cancellationToken);
            hostBody = hostBody.Replace("#_name_#", host.Name)
                        .Replace("#_visibleDate_#", hostDate.ToString("dddd, dd MMMM yyyy HH:mm", ci))
                        .Replace("#_patient_#", $"{user.Name} {user.LastName}")
                        .Replace("#_userEmail_#", user.Email);

            return this._emailSender.Send(host.Email, $"Cancelación de cita del {hostDate.ToString("dddd, dd MMMM HH:mm", ci)}", hostBody, true);
        }
    }
}
