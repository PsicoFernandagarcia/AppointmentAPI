using Appointment.Application.AvailabilityUseCases.ChangeAvailabilityStatus;
using Appointment.Application.PaymentUseCases.UpdateLatestPaymentSessions;
using Appointment.Application.SendEmailUseCase.AppointmentCancelation;
using Appointment.Domain;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using Appointment.Infrastructure.Configuration;
using CSharpFunctionalExtensions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Appointment.Application.AppointmentUseCases.CancelAppointment
{
    public class CancelAppointmentHandler : IRequestHandler<CancelAppointmentsCommand, Result<bool, ResultError>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IMediator _mediator;
        private readonly AppDbContext _context;

        public CancelAppointmentHandler(IUserRepository userRepository, IAppointmentRepository appointmentRepository, IMediator mediator, AppDbContext context)
        {
            _userRepository = userRepository;
            _appointmentRepository = appointmentRepository;
            _mediator = mediator;
            _context = context;
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
            await using var scope = await _context.Database.BeginTransactionAsync();

            try
            {
                await _appointmentRepository.Update(appointment);
                await _mediator.Send(new ChangeAvailabilityStatusCommand(request.UserId, true, appointment.Id));
                await _mediator.Send(new UpdateLastPaymentSessionsCommand
                {
                    HostId = appointment.HostId,
                    PatientId = appointment.PatientId,
                    NewAppointmentAdded = false
                });

                await scope.CommitAsync();
                await _mediator.Send(new SendAppointmentCancelationEmailCommand
                {
                    UserId = appointment.PatientId,
                    HostId = appointment.HostId,
                    DateTimeInUTC = appointment.DateFrom.ToUniversalTime()
                }, cancellationToken);
            }
            catch (Exception)
            {
                await scope.RollbackAsync();
                throw;
            }
            return Result.Success<bool, ResultError>(true);
        }

        private bool isValidAppointmentToDelete(int userId, Domain.Entities.Appointment appointment)
            => !(appointment is null)
                && appointment.HostId == userId
                || (
                     appointment.PatientId == userId
                    && appointment.DateFrom.ToUniversalTime() > DateTime.UtcNow.AddDays(1)
                );
    }
}