using Appointment.Application.AvailabilityUseCases.AppointmentConfigured;
using Appointment.Application.PaymentUseCases.UpdateLatestPaymentSessions;
using Appointment.Application.SendEmailUseCase.AppointmentConfirmation;
using Appointment.Domain;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

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

        public async Task<Result<Domain.Entities.Appointment, ResultError>> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new AppointmentConfiguredCommand()
            {
                AvailabilityId = request.AvailabilityId,
                IsEmpty = false
            }, cancellationToken);
            if (result.IsFailure) return Result.Failure<Domain.Entities.Appointment, ResultError>(result.Error);

            var appointmentResult = Domain.Entities.Appointment.Create(0, request.Title, request.DateFrom, request.DateTo,
                request.With, request.CreatedById, request.Color, false, request.HostId
                , request.PatientId, AppointmentStatus.CREATED, DateTime.Now
                );
            if (appointmentResult.IsFailure) return Result.Failure<Domain.Entities.Appointment, ResultError>(appointmentResult.Error);

            await _mediator.Send(new UpdateLatestPaymentSessionsCommand
            {
                HostId = request.HostId,
                PatientId = request.PatientId,
                NewAppointmentAdded = true
            });
            await _mediator.Send(new SendAppointmentConfirmationEmailCommand
            {
                UserId = request.PatientId,
                HostId = request.HostId,
                DateTimeInUTC = request.DateFrom.ToUniversalTime()
            }, cancellationToken);
            return await _appointmentRepository.Create(appointmentResult.Value);
        }


    }
}