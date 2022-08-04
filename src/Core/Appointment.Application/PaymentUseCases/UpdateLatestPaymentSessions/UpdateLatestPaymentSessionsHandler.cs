using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.PaymentUseCases.UpdateLatestPaymentSessions
{
    public class UpdateLatestPaymentSessionsHandler : IRequestHandler<UpdateLatestPaymentSessionsCommand, Result<Payment, ResultError>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMediator _mediator;
        public UpdateLatestPaymentSessionsHandler(IPaymentRepository paymentRepository, IMediator mediator)
        {
            _paymentRepository = paymentRepository;
            _mediator = mediator;
        }

        public async Task<Result<Payment, ResultError>> Handle(UpdateLatestPaymentSessionsCommand request, CancellationToken cancellationToken)
        {
            var sessionsToAdd = request.NewAppointmentAdded ? -1 : 1; 
            var lastPayment = await _paymentRepository.GetLastPayment(request.PatientId, request.HostId);
            if (lastPayment != null)
            {
                lastPayment.SessionsLeft += sessionsToAdd;
                return await _paymentRepository.Update(lastPayment);
            }
            return await _paymentRepository.Insert(Payment.Create(0,DateTime.Now,request.PatientId,request.HostId,0,request.Currency,0, sessionsToAdd).Value);
        }


    }
}