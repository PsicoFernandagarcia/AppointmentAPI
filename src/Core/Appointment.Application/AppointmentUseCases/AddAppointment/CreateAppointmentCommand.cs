using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System;

namespace Appointment.Application.AppointmentUseCases.AddAppointment
{
    public class CreateAppointmentCommand : IRequest<Result<Domain.Entities.Appointment, ResultError>>
    {
        public string Title { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string With { get; set; }
        public int CreatedById { get; set; }
        public int HostId { get; set; }
        public int PatientId { get; set; }
        public string Color { get; set; }
        public int AvailabilityId { get; set; }
        public string LocalDateTime { get; set; }


    }
}
