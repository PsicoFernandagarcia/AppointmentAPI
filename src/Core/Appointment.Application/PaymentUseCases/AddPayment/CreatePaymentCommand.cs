using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System;

namespace Appointment.Application.PaymentUseCases.AddPayment
{
    public class CreatePaymentCommand : IRequest<Result<Domain.Entities.Payment, ResultError>>
    {
        public int PatientId { get; set; }
        public int HostId { get; set; }
        public decimal Amount { get; set; }
        public int SessionsPaid { get; set; }
        public string Currency { get; set; }

    }
}
