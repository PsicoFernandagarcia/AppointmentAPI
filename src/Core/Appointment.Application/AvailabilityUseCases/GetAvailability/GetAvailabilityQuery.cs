using Appointment.Domain.Entities;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System;
using System.Collections.Generic;

namespace Appointment.Application.AvailabilityUseCases.GetAvailability
{
    public class GetAvailabilityQuery : IRequest<Result<IEnumerable<AvailabilityDto>, ResultError>>
    {
        public int HostId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public bool ShowOnlyAvailable { get; set; }
    }
}
