using Appointment.Domain.Entities;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System.Collections.Generic;

namespace Appointment.Application.AppointmentUseCases.GetYearInformation
{
    public class GetYearInformationQuery: IRequest<Result<IEnumerable<AppointmentYearInformationDto>, ResultError>>
    {
        public int Year { get; set; }
        public int? PatientId { get; set; }
        public int HostId { get; set; }
    }
}
