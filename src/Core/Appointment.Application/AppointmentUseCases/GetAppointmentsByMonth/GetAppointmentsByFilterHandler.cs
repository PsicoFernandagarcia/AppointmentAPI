using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.AppointmentUseCases.GetAppointmentsByFilter
{
    public class GetAppointmentsByFilterHandler : IRequestHandler<GetAppointmentsByFilterQuery, Result<IEnumerable<AppointmentDto>, ResultError>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAppointmentRepository _appointmentRepository;

        public GetAppointmentsByFilterHandler(IUserRepository userRepository, IAppointmentRepository appointmentRepository)
        {
            _userRepository = userRepository;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<Result<IEnumerable<AppointmentDto>, ResultError>> Handle(GetAppointmentsByFilterQuery request, CancellationToken cancellationToken)
            => Result.Success<IEnumerable<AppointmentDto>, ResultError>(
                await _appointmentRepository.GetByFilter(request.Year, request.UserId)
                );

    }
}