using Appointment.Domain.Infrastructure;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.Options;
using System;
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
        private readonly EmailOptions _emailOptions;
        public SendAppointmentCancelationEmailHandler(IEmailSender emailSender, IUserRepository userRepository, IOptions<EmailOptions> emailOptions)
        {
            _emailSender = emailSender;
            _userRepository = userRepository;
            _emailOptions = emailOptions.Value;
        }
        public async Task<Result<bool, ResultError>> Handle(SendAppointmentCancelationEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserById(request.UserId);
            var host = await _userRepository.GetUserById(request.HostId);
            var hostDate = request.DateTimeInUTC.AddMinutes(host.TimezoneOffset);

            var ci = CultureInfo.GetCultureInfo("es-ES");
            var hostBody = await File.ReadAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(), "Content/appointment_cancelation.html"), cancellationToken);
            hostBody = hostBody.Replace("#_name_#", host.Name)
                        .Replace("#_visibleDate_#", hostDate.ToString("dddd, dd MMMM yyyy HH:mm",ci))
                        .Replace("#_patient_#", $"{user.Name} {user.LastName}")
                        .Replace("#_userEmail_#", user.Email);

            return this._emailSender.Send(host.Email, $"Cancelación de cita {hostDate.ToShortDateString()}", hostBody, true);
        }
    }
    public class SendAppointmentCancelationEmailCommand : IRequest<Result<bool, ResultError>>
    {

        public int UserId { get; set; }
        public int HostId { get; set; }
        public DateTime DateTimeInUTC { get; set; }

    }
}
