using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System;

namespace Appointment.Application.SendEmailUseCase.AppointmentConfirmation
{
    public class SendAppointmentConfirmationEmailCommand : IRequest<Result<bool, ResultError>>
    {

        public int UserId { get; set; }
        public int HostId { get; set; }
        public DateTime DateTimeInUTC { get; set; }

    }
}
