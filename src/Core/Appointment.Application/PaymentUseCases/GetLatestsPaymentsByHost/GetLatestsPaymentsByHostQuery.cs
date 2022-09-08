using Appointment.Domain.Entities;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System.Collections.Generic;

namespace Appointment.Application.PaymentUseCases.GetLatestsPaymentsByHost
{
    public class GetLatestsPaymentsByHostQuery : IRequest<Result<IEnumerable<Payment>, ResultError>>
    {
        public int HostId { get; set; }

        public GetLatestsPaymentsByHostQuery(int hostId)
        {
            HostId = hostId;
        }

        public GetLatestsPaymentsByHostQuery()
        {

        }
    }
}
