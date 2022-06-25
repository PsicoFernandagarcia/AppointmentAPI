using Appointment.Domain.Entities;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System;

namespace Appointment.Application.AvailabilityUseCases.CreateAvailability
{
    public class CreateAvailabilityCommand : IRequest<Result<Availability, ResultError>>
    {
        public int HostId { get; set; }
        public int AmountOfTime { get; set; }
        public DateTime DateOfAvailability { get; set; }
    }
}
