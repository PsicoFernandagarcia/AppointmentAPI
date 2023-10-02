using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.PaymentUseCases.AddPayment
{
    public class CreatePaymentHandler : IRequestHandler<CreatePaymentCommand, Result<Payment, ResultError>>
    {
        private readonly IPaymentRepository _paymentRepository;
        public CreatePaymentHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<Result<Payment, ResultError>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            var lastPaymentResult = await _paymentRepository.GetLast(request.PatientId, request.HostId);
            var sessionsLeft = request.SessionsPaid;
            if (lastPaymentResult != null && lastPaymentResult.PaidAt <= request.PaidAt)
                sessionsLeft += lastPaymentResult.SessionsLeft;
            
            if (lastPaymentResult != null && lastPaymentResult.PaidAt >= request.PaidAt)
            {
                lastPaymentResult.SessionsLeft += request.SessionsPaid;
                await _paymentRepository.Update(lastPaymentResult);
            }

            var paymentResult = Payment.Create(0, request.PaidAt, request.PatientId, request.HostId, request.Amount, request.Currency, request.SessionsPaid, sessionsLeft, request.Observations);

            if (!paymentResult.IsSuccess)
                return Result.Failure<Payment, ResultError>(new CreationError(paymentResult.Error));

            return await _paymentRepository.Insert(paymentResult.Value);
        }


    }
}