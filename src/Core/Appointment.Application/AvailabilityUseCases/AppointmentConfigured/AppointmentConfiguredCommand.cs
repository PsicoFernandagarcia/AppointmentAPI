using System.Collections.Generic;
using Appointment.Domain.Entities;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;

namespace Appointment.Application.AvailabilityUseCases.AppointmentConfigured
{
    public class AppointmentConfiguredCommand : IRequest<Result<string, ResultError>>
    {
        public int AvailabilityId { get; set; }
        public bool IsEmpty { get; set; }
    }
}