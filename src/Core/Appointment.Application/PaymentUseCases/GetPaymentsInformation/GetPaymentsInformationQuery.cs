using Appointment.Domain.Entities;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System.Collections.Generic;

namespace Appointment.Application.PaymentUseCases.GetLatestsPaymentsByHost
{
    public class GetPaymentsInformationQuery : IRequest<Result<IEnumerable<PaymentInformation>, ResultError>>
    {
        public int HostId { get; set; }
        public int Year { get; set; }
    }
}
