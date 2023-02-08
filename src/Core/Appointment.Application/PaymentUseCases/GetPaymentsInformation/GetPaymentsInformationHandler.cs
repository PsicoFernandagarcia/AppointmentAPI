using Appointment.Application.PaymentUseCases.GetLatestsPaymentsByHost;
using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.PaymentUseCases.GetPaymentsInformationHandler
{
    public class GetPaymentsInformationHandlerHandler : IRequestHandler<GetPaymentsInformationQuery, Result<IEnumerable<PaymentInformation>, ResultError>>
    {
        private readonly IPaymentRepository _paymentRepository;
        public GetPaymentsInformationHandlerHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }
        public async Task<Result<IEnumerable<PaymentInformation>, ResultError>> Handle(GetPaymentsInformationQuery request, CancellationToken cancellationToken)
        {
            var paymentsResult = await _paymentRepository.GetYearInformation(request.Year, request.HostId);
            return Result.Success<IEnumerable<PaymentInformation>, ResultError>(paymentsResult);
        }
    }
}
