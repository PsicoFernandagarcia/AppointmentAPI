﻿using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.PaymentUseCases.AddPayment
{
    public class UpdatePaymentHandler : IRequestHandler<UpdatePaymentCommand, Result<Payment, ResultError>>
    {
        private readonly IPaymentRepository _paymentRepository;
        public UpdatePaymentHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<Result<Payment, ResultError>> Handle(UpdatePaymentCommand request, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.Get(request.Id);
            var sessionLeft = request.SessionsPaid - payment.SessionsPaid ;
            var paymentResult = payment.Update(request.PaidAt, request.PatientId, request.HostId, request.Amount, request.Currency, request.SessionsPaid, sessionLeft + payment.SessionsLeft);

            if (!paymentResult.IsSuccess)
                return Result.Failure<Payment, ResultError>(new CreationError(paymentResult.Error));

            var lastPayment = await _paymentRepository.GetLast(request.PatientId, request.HostId);
            if (lastPayment.Id != payment.Id)
            {
                lastPayment.SessionsLeft += sessionLeft;
                await _paymentRepository.Update(lastPayment);
            }
            return await _paymentRepository.Update(paymentResult.Value);
        }


    }
}