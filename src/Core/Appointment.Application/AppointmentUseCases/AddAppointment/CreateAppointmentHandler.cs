using Appointment.Application.AvailabilityUseCases.AppointmentConfigured;
using Appointment.Application.PaymentUseCases.UpdateLatestPaymentSessions;
using Appointment.Application.SendEmailUseCase.AppointmentConfirmation;
using Appointment.Domain;
using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using Appointment.Infrastructure.Configuration;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using System;
using System.Threading;
using System.Threading.Tasks;
using Entities = Appointment.Domain.Entities;

namespace Appointment.Application.AppointmentUseCases.AddAppointment
{
    public class CreateAppointmentHandler : IRequestHandler<CreateAppointmentCommand, Result<Domain.Entities.Appointment, ResultError>>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly AppDbContext _context;
        private readonly IMediator _mediator;
        private readonly IOutputCacheStore _cachingStore;

        public CreateAppointmentHandler(IAppointmentRepository appointmentRepository,
                                        IMediator mediator,
                                        AppDbContext context,
                                        IOutputCacheStore cachingStore)
        {
            _appointmentRepository = appointmentRepository;
            _mediator = mediator;
            _context = context;
            _cachingStore = cachingStore;
        }

        public async Task<Result<Entities.Appointment, ResultError>> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointmentResult = MapToEntity(request);
            if (appointmentResult.IsFailure) return Result.Failure<Domain.Entities.Appointment, ResultError>(new CreationError(appointmentResult.Error));

            await using var scope = await _context.Database.BeginTransactionAsync();
            try
            {
                await UpdateLastPayment(request, cancellationToken);
                await _appointmentRepository.Create(appointmentResult.Value);
                var result = await DisableAvailability(request.AvailabilityId, appointmentResult.Value.Id, appointmentResult.Value.Patient?.FullName ?? appointmentResult.Value.With, cancellationToken);
                if (result.IsFailure) return Result.Failure<Entities.Appointment, ResultError>(new CreationError(result.Error.Message));
                await scope.CommitAsync(cancellationToken);
            }
            catch (Exception)
            {
                await scope.RollbackAsync(cancellationToken);
                throw;
            }
            await _cachingStore.EvictByTagAsync(CacheKeys.Appointments, cancellationToken);

            await SendConfirmationEmail(request, cancellationToken);
            return appointmentResult.Value;
        }

        private static Result<Entities.Appointment> MapToEntity(CreateAppointmentCommand request)
            => Entities.Appointment.Create(0, request.Title, request.DateFrom, request.DateTo,
                request.With, request.CreatedById, request.Color, false, request.HostId
                , request.PatientId, AppointmentStatus.CREATED, DateTime.Now
                );

        private async Task<Result<Availability, ResultError>> DisableAvailability(int availabilityId, int appointmentId,
                                                                                  string appointmentWith,
                                                                                  CancellationToken cancellationToken)
            => await _mediator.Send(new AppointmentConfiguredCommand()
            {
                AvailabilityId = availabilityId,
                IsEmpty = false,
                AppointmentId = appointmentId,
                AppointmentWith = appointmentWith,
            }, cancellationToken);

        private async Task UpdateLastPayment(CreateAppointmentCommand request, CancellationToken cancellationToken)
            => await _mediator.Send(new UpdateLastPaymentSessionsCommand
            {
                HostId = request.HostId,
                PatientId = request.PatientId,
                NewAppointmentAdded = true
            }, cancellationToken);
        private async Task SendConfirmationEmail(CreateAppointmentCommand request, CancellationToken cancellationToken)
            => await _mediator.Send(new SendAppointmentConfirmationEmailCommand
            {
                UserId = request.PatientId,
                HostId = request.HostId,
                DateTimeInUTC = request.DateFrom.ToUniversalTime()
            }, cancellationToken);


    }
}