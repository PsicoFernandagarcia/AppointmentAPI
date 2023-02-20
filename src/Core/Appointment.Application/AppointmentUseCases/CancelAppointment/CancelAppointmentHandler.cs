using Appointment.Application.AvailabilityUseCases.ChangeAvailabilityStatus;
using Appointment.Application.PaymentUseCases.UpdateLatestPaymentSessions;
using Appointment.Application.SendEmailUseCase.AppointmentCancelation;
using Appointment.Domain;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.AppointmentUseCases.CancelAppointment
{
    public class CancelAppointmentHandler : IRequestHandler<CancelAppointmentsCommand, Result<bool, ResultError>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IMediator _mediator;

        public CancelAppointmentHandler(IUserRepository userRepository, IAppointmentRepository appointmentRepository, IMediator mediator)
        {
            _userRepository = userRepository;
            _appointmentRepository = appointmentRepository;
            _mediator = mediator;
        }

        public async Task<Result<bool, ResultError>> Handle(CancelAppointmentsCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserById(request.UserId);
            if (user is null)
                return Result.Failure<bool, ResultError>("User not found or you don't have permissions to do this");

            var appointment = await _appointmentRepository.GetById(request.AppointmentId);
            if (!isValidAppointmentToDelete(request.UserId, appointment))
                return Result.Failure<bool, ResultError>("Appointment not valid or you don't have permissions to do this");

            appointment.ChangeStatus(AppointmentStatus.CANCELED);
            await _appointmentRepository.Update(appointment);
            await _mediator.Send(new ChangeAvailabilityStatusCommand
            {
                HostId = appointment.HostId,
                IsEmpty = true,
                DateFromUtc = appointment.DateFrom.AddMinutes(-5),
                DateToUtc = appointment.DateFrom.AddMinutes(5),
            });
            await _mediator.Send(new UpdateLastPaymentSessionsCommand
            {
                HostId = appointment.HostId,
                PatientId = appointment.PatientId,
                NewAppointmentAdded = false
            });
            await _mediator.Send(new SendAppointmentCancelationEmailCommand
            {
                UserId = appointment.PatientId,
                HostId = appointment.HostId,
                DateTimeInUTC = appointment.DateFrom.ToUniversalTime()
            }, cancellationToken);
            return Result.Success<bool, ResultError>(true);
        }

        private bool isValidAppointmentToDelete(int userId, Domain.Entities.Appointment appointment)
            => !(appointment is null)
                && appointment.HostId == userId
                || (
                     appointment.PatientId == userId
                    && appointment.DateFrom.ToUniversalTime() > DateTime.UtcNow
                );
    }
}