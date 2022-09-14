using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.AppointmentUseCases.GetYearInformation
{
    public class GetYearInformationHandler : IRequestHandler<GetYearInformationQuery, Result<IEnumerable<AppointmentYearInformationDto>, ResultError>>
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public GetYearInformationHandler(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }
        public async Task<Result<IEnumerable<AppointmentYearInformationDto>, ResultError>> Handle(GetYearInformationQuery request, CancellationToken cancellationToken)
        {
            if (request.HostId <= 0) return Result.Failure<IEnumerable<AppointmentYearInformationDto>, ResultError>("Not valid host");
            return Result.Success<IEnumerable<AppointmentYearInformationDto>, ResultError>(
                   await _appointmentRepository.GetYearInformation(request.Year, request.HostId, request.PatientId)
                   );
        }
    }
}
