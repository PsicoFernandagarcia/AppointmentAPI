using Appointment.Domain.Entities;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;

namespace Appointment.Application.AvailabilityUseCases.AppointmentConfigured
{
    public class AppointmentConfiguredCommand : IRequest<Result<Availability, ResultError>>
    {
        public int AvailabilityId { get; set; }
        public bool IsEmpty { get; set; }
        public AppointmentConfiguredCommand()
        {

        }

        public AppointmentConfiguredCommand(int availabilityId, bool isEmpty)
        {
            AvailabilityId = availabilityId;
            IsEmpty = isEmpty;
        }
    }
}