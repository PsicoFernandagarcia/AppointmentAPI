using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Appointment.Application.AvailabilityUseCases.ChangeAvailabilityStatus;
using Appointment.Domain;
using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;

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
                HostId= appointment.HostId,
                IsEmpty = true,
                DateFromUtc = appointment.DateFrom.AddMinutes(-5),
                DateToUtc = appointment.DateFrom.AddMinutes(5),
            });
            return Result.Success<bool, ResultError>(true);
        }

        private bool isValidAppointmentToDelete(int userId, Domain.Entities.Appointment appointment)
            => !(appointment is null)
                && (
                    appointment.HostId == userId
                || appointment.PatientId == userId
            )
            && appointment.DateFrom.ToUniversalTime() > DateTime.UtcNow;
    }
}