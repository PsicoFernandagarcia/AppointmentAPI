using Appointment.Domain;
using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
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
    public class CreatePaymentCommand : IRequest<Result<Domain.Entities.Payment, ResultError>>
    {
        public int PatientId { get; set; }
        public int HostId { get; set; }
        public decimal Amount { get; set; }
        public int SessionsPaid { get; set; }
        public string Currency { get; set; }
        public DateTime PaidAt { get; set; }
        public string? Observations { get; set; }
        public IEnumerable<int> Appointments { get; set; }
        public CreatePaymentCommand()
        {

        }

        public CreatePaymentCommand(int patientId,
                                    int hostId,
                                    decimal amount,
                                    int sessionsPaid,
                                    string currency,
                                    DateTime paidAt,
                                    string observations,
                                    IEnumerable<int> appointments)
        {
            PatientId = patientId;
            HostId = hostId;
            Amount = amount;
            SessionsPaid = sessionsPaid;
            Currency = currency;
            PaidAt = paidAt;
            Observations = observations;
            Appointments = appointments;
        }
        public AddPaymentDto ToDto()
        {
            var p = new AddPaymentDto
            {
                PatientId = PatientId,
                HostId = HostId,
                Amount = Amount,
                SessionsPaid = SessionsPaid,
                Currency = Currency,
                PaidAt = PaidAt,
                Observations = Observations,
                Appointments = Appointments
            };
            return p;
        }
    }
    public class CreatePaymentHandler : IRequestHandler<CreatePaymentCommand, Result<Payment, ResultError>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IOutputCacheStore _cachingStore;

        public CreatePaymentHandler(IPaymentRepository paymentRepository,
                                    IOutputCacheStore cachingStore,
                                    IAppointmentRepository appointmentRepository)
        {
            _paymentRepository = paymentRepository;
            _cachingStore = cachingStore;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<Result<Payment, ResultError>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            await _cachingStore.EvictByTagAsync(CacheKeys.Payments, cancellationToken);
            var insertResult = await _paymentRepository.Insert(request.ToDto());
            if (insertResult.IsFailure)
            {
                return insertResult;
            }

            var unpaidAppointments = await _appointmentRepository.GetByFilter(null, request.PatientId, true);
            var lastPaymentResult = await _paymentRepository.GetLast(request.PatientId, request.HostId);
            lastPaymentResult.SessionsLeft = unpaidAppointments.Where(a => a.Status != AppointmentStatus.CANCELED.ToString()).Count() * -1;

            await _paymentRepository.Update(lastPaymentResult);
            return insertResult;
        }


    }
}