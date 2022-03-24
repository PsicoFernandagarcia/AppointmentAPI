using System.Collections.Generic;
using Appointment.Domain.Entities;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;

namespace Appointment.Application.AvailabilityUseCases.CreateAvailability
{
    public class CreateAvailabilitiesCommand : IRequest<Result<IEnumerable<Availability>, ResultError>>
    {
        public int HostId { get; set; }
        public IEnumerable<CreateAvailabilityCommand> Availabilities { get; set; }
        public IEnumerable<int> AvailabilitiesToRemove { get; set; }
    }
}