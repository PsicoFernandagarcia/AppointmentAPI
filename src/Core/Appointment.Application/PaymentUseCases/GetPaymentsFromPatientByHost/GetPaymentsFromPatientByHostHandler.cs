using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.PaymentUseCases.GetPaymentsFromPatientByHost
{
    public class GetPaymentsFromPatientByHostHandler : IRequestHandler<GetPaymentsFromPatientByHostQuery, Result<IEnumerable<Payment>, ResultError>>
    {
        private readonly IPaymentRepository _paymentRepository;
        public GetPaymentsFromPatientByHostHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }
        public async Task<Result<IEnumerable<Payment>, ResultError>> Handle(GetPaymentsFromPatientByHostQuery request, CancellationToken cancellationToken)
        {
           var paymentsResult = await _paymentRepository.GetPayments(request.PatientId,request.HostId,request.Count );
           return Result.Success<IEnumerable<Payment>,ResultError>(paymentsResult);
        }
    }
}
