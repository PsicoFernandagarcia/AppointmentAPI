using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.AppointmentUseCases.GetLastAppointment
{
    public class GetLastAppointmentsHandler : IRequestHandler<GetLastAppointmentsQuery, Result<IEnumerable<AppointmentDto>, ResultError>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAppointmentRepository _appointmentRepository;

        public GetLastAppointmentsHandler(IUserRepository userRepository, IAppointmentRepository appointmentRepository)
        {
            _userRepository = userRepository;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<Result<IEnumerable<AppointmentDto>, ResultError>> Handle(GetLastAppointmentsQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserById(request.PatientId);
            if (user is null)
                return Result.Failure<IEnumerable<AppointmentDto>, ResultError>("User not found or you don't have permissions to do this");
            return Result.Success<IEnumerable<AppointmentDto>, ResultError>(await _appointmentRepository.GetLastAppointments(request.HostId,request.PatientId, request.TotalCount));
        }
    }
}