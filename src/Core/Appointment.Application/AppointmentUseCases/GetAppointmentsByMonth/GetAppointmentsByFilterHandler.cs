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
    public class GetAppointmentsByFilterQuery : IRequest<Result<IEnumerable<AppointmentDto>, ResultError>>
    {
        public int? Year { get; set; }
        public int UserId { get; set; }
        public bool? IsUnpaid { get; set; }
    }
    public class GetAppointmentsByFilterHandler : IRequestHandler<GetAppointmentsByFilterQuery, Result<IEnumerable<AppointmentDto>, ResultError>>
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public GetAppointmentsByFilterHandler(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public async Task<Result<IEnumerable<AppointmentDto>, ResultError>> Handle(GetAppointmentsByFilterQuery request, CancellationToken cancellationToken)
            => Result.Success<IEnumerable<AppointmentDto>, ResultError>(
                await _appointmentRepository.GetByFilter(request.Year, request.UserId, request.IsUnpaid)
                );

    }
}