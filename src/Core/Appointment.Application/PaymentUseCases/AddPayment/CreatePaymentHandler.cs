using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.PaymentUseCases.AddPayment
{
    public class CreatePaymentHandler : IRequestHandler<CreatePaymentCommand, Result<Payment, ResultError>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMediator _mediator;
        public CreatePaymentHandler(IPaymentRepository paymentRepository, IMediator mediator)
        {
            _paymentRepository = paymentRepository;
            _mediator = mediator;
        }

        public async Task<Result<Payment, ResultError>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            var lastPaymentResult = await _paymentRepository.GetLastPayment(request.PatientId, request.HostId);
            var sessionsLeft = request.SessionsPaid;
            if (lastPaymentResult == null)
                sessionsLeft += lastPaymentResult.SessionsLeft;
            var paymentResult = Payment.Create(0, DateTime.Now, request.PatientId, request.HostId, request.Amount,request.Currency, request.SessionsPaid, sessionsLeft);
            
            if (!paymentResult.IsSuccess) 
                    return Result.Failure<Payment, ResultError>(paymentResult.Error);

            return await _paymentRepository.Insert(paymentResult.Value);
        }


    }
}