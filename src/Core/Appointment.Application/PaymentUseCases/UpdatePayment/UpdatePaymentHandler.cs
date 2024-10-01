using Appointment.Domain;
using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using Appointment.Infrastructure.Repositories;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.PaymentUseCases.AddPayment
{
    public class UpdatePaymentCommand : IRequest<Result<Payment, ResultError>>
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int HostId { get; set; }
        public decimal Amount { get; set; }
        public int SessionsPaid { get; set; }
        public string Currency { get; set; }
        public DateTime PaidAt { get; set; }
        public string? Observations { get; set; }
        public IEnumerable<int> Appointments { get; set; }
        public UpdatePaymentCommand()
        {

        }

        public UpdatePaymentCommand(int patientId,
                                    int hostId,
                                    decimal amount,
                                    int sessionsPaid,
                                    string currency,
                                    int paymentId,
                                    DateTime paidAt,
                                    string observations,
                                    IEnumerable<int> appointments)
        {
            PatientId = patientId;
            HostId = hostId;
            Amount = amount;
            SessionsPaid = sessionsPaid;
            Currency = currency;
            Id = paymentId;
            PaidAt = paidAt;
            Observations = observations;
            Appointments = appointments;
        }
    }
    public class UpdatePaymentHandler : IRequestHandler<UpdatePaymentCommand, Result<Payment, ResultError>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IOutputCacheStore _cachingStore;

        public UpdatePaymentHandler(IPaymentRepository paymentRepository,
                                    IOutputCacheStore cachingStore,
                                    IAppointmentRepository appointmentRepository)
        {
            _paymentRepository = paymentRepository;
            _cachingStore = cachingStore;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<Result<Payment, ResultError>> Handle(UpdatePaymentCommand request, CancellationToken cancellationToken)
        {
            var appointments = await _appointmentRepository.GetByIds(request.Appointments);
            var payment = await _paymentRepository.Get(request.Id);
            var sessionLeft = request.SessionsPaid - payment.SessionsPaid ;
            var paymentResult = payment.Update(request.PaidAt,
                                               request.PatientId,
                                               request.HostId,
                                               request.Amount,
                                               request.Currency,
                                               request.SessionsPaid,
                                               sessionLeft + payment.SessionsLeft,
                                               request.Observations,
                                               appointments);

            if (!paymentResult.IsSuccess)
                return Result.Failure<Payment, ResultError>(new CreationError(paymentResult.Error));

            await _paymentRepository.Update(paymentResult.Value);

            var unpaidAppointments = await _appointmentRepository.GetByFilter(null, request.PatientId, true);
            var lastPaymentResult = await _paymentRepository.GetLast(request.PatientId, request.HostId);
            lastPaymentResult.SessionsLeft = unpaidAppointments.Count() * -1;

            await _cachingStore.EvictByTagAsync(CacheKeys.Payments, cancellationToken);
            return await _paymentRepository.Update(lastPaymentResult);
        }


    }
}