using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System.Collections.Generic;

namespace Appointment.Application.SendEmailUseCase.Reminder
{
    public class SendReminderEmailCommand : IRequest<Result<bool, ResultError>>
    {

        public string HostName { get; set; }
        public string HostEmail { get; set; }
        public IEnumerable<Domain.Entities.Appointment> Appointments { get; set; }

    }
}
