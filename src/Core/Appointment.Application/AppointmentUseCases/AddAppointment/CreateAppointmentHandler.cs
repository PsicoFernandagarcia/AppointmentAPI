using Appointment.Application.AvailabilityUseCases.AppointmentConfigured;
using Appointment.Application.PaymentUseCases.UpdateLatestPaymentSessions;
using Appointment.Application.SendEmailUseCase.AppointmentConfirmation;
using Appointment.Domain;
using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Entities = Appointment.Domain.Entities;

namespace Appointment.Application.AppointmentUseCases.AddAppointment
{
    public class CreateAppointmentHandler : IRequestHandler<CreateAppointmentCommand, Result<Domain.Entities.Appointment, ResultError>>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IMediator _mediator;
        public CreateAppointmentHandler(IAppointmentRepository appointmentRepository, IMediator mediator)
        {
            _appointmentRepository = appointmentRepository;
            _mediator = mediator;
        }

        public async Task<Result<Entities.Appointment, ResultError>> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointmentResult = MapToEntity(request);
            if (appointmentResult.IsFailure) return Result.Failure<Domain.Entities.Appointment, ResultError>(new CreationError(appointmentResult.Error));

            var result = await DisableAvailability(request.AvailabilityId, cancellationToken);
            if (result.IsFailure) return Result.Failure<Domain.Entities.Appointment, ResultError>(new CreationError(result.Error.Message));

            await UpdateLastPayment(request, cancellationToken);
            await SendConfirmationEmail(request, cancellationToken);

            return await _appointmentRepository.Create(appointmentResult.Value);
        }

        private Result<Entities.Appointment> MapToEntity(CreateAppointmentCommand request)
            => Entities.Appointment.Create(0, request.Title, request.DateFrom, request.DateTo,
                request.With, request.CreatedById, request.Color, false, request.HostId
                , request.PatientId, AppointmentStatus.CREATED, DateTime.Now
                );

        private async Task<Result<Availability, ResultError>> DisableAvailability(int availabilityId, CancellationToken cancellationToken)
            => await _mediator.Send(new AppointmentConfiguredCommand()
            {
                AvailabilityId = availabilityId,
                IsEmpty = false
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