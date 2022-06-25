using Appointment.Domain.Entities;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System.Collections.Generic;

namespace Appointment.Application.AvailabilityUseCases.CreateAvailability
{
    public class CreateAvailabilitiesCommand : IRequest<Result<IEnumerable<Availability>, ResultError>>
    {
        public int HostId { get; set; }
        public IEnumerable<CreateAvailabilityCommand> Availabilities { get; set; }
        public IEnumerable<int> AvailabilitiesToRemove { get; set; }
    }
}