using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.PaymentUseCases.GetLatestsPaymentsByHost
{
    public class GetLatestsPaymentsByHostHandler : IRequestHandler<GetLatestsPaymentsByHostQuery, Result<IEnumerable<Payment>, ResultError>>
    {
        private readonly IPaymentRepository _paymentRepository;
        public GetLatestsPaymentsByHostHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }
        public async Task<Result<IEnumerable<Payment>, ResultError>> Handle(GetLatestsPaymentsByHostQuery request, CancellationToken cancellationToken)
        {
            var paymentsResult = await _paymentRepository.GetLatestPayments(request.HostId);
            return Result.Success<IEnumerable<Payment>, ResultError>(paymentsResult);
        }
    }
}
