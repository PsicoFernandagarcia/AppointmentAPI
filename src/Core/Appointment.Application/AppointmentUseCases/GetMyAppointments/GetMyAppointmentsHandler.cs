using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.AppointmentUseCases.GetMyAppointment
{
    public class GetMyAppointmentsHandler : IRequestHandler<GetMyAppointmentsQuery, Result<IEnumerable<AppointmentDto>, ResultError>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAppointmentRepository _appointmentRepository;

        public GetMyAppointmentsHandler(IUserRepository userRepository, IAppointmentRepository appointmentRepository)
        {
            _userRepository = userRepository;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<Result<IEnumerable<AppointmentDto>, ResultError>> Handle(GetMyAppointmentsQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserById(request.UserId);
            if (user is null)
                return Result.Failure<IEnumerable<AppointmentDto>, ResultError>("User not found or you don't have permissions to do this");
            return Result.Success<IEnumerable<AppointmentDto>, ResultError>(await _appointmentRepository.GetByUserId(request.UserId, request.DateFrom));
        }
    }
}