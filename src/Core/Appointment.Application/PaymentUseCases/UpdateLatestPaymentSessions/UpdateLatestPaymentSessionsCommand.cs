using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System;

namespace Appointment.Application.PaymentUseCases.UpdateLatestPaymentSessions
{
    public class UpdateLatestPaymentSessionsCommand : IRequest<Result<Domain.Entities.Payment, ResultError>>
    {
        public int PatientId { get; set; }
        public int HostId { get; set; }
        public bool NewAppointmentAdded { get; set; }
        public string Currency { get; set; }

    }
}
