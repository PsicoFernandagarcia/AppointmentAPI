using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System;

namespace Appointment.Application.PaymentUseCases.AddPayment
{
    public class UpdatePaymentCommand : IRequest<Result<Domain.Entities.Payment, ResultError>>
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int HostId { get; set; }
        public decimal Amount { get; set; }
        public int SessionsPaid { get; set; }
        public string Currency { get; set; }
        public DateTime PaidAt { get; set; }
        public UpdatePaymentCommand()
        {

        }

        public UpdatePaymentCommand(int patientId, int hostId, decimal amount, int sessionsPaid, string currency, int paymentId, DateTime paidAt)
        {
            PatientId = patientId;
            HostId = hostId;
            Amount = amount;
            SessionsPaid = sessionsPaid;
            Currency = currency;
            Id = paymentId;
            PaidAt = paidAt;
        }
    }
}
