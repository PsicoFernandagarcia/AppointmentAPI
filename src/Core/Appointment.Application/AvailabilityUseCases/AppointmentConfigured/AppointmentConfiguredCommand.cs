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
        public int AppointmentId { get; set; }
        public string AppointmentWith { get; set; }
        public AppointmentConfiguredCommand()
        {

        }

        public AppointmentConfiguredCommand(int availabilityId, bool isEmpty, int appointmentId, string appointmentWith)
        {
            AvailabilityId = availabilityId;
            IsEmpty = isEmpty;
            AppointmentId = appointmentId;
            AppointmentWith = appointmentWith;
        }
    }
}