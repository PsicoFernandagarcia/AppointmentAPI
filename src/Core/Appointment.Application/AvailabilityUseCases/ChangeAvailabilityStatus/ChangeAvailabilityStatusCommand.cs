using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System;

namespace Appointment.Application.AvailabilityUseCases.ChangeAvailabilityStatus
{
    public class ChangeAvailabilityStatusCommand : IRequest<Result<bool, ResultError>>
    {

        public int HostId { get; set; }
        public bool IsEmpty { get; set; }
        public int AppointmentId { get; set; }

        public ChangeAvailabilityStatusCommand()
        {

        }

        public ChangeAvailabilityStatusCommand(int hostId, bool isEmpty, int appointmentId)
        {
            HostId = hostId;
            IsEmpty = isEmpty;
            AppointmentId = appointmentId;
        }
    }
}