using System;
using Appointment.Domain.Entities;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;

namespace Appointment.Application.AppointmentUseCases.AddAppointmentByHost
{
    public class CreateAppointmentByHostCommand: IRequest<Result<Domain.Entities.Appointment,ResultError>>
    {
        public string Title { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string PatientEmail { get; set; }
        public string PatientName { get; set; }
        public int TimezoneOffset { get; set; }
        public int HostId { get; set; }
        public int PatientId { get; set; }
        public string Color { get; set; }
        public int AvailabilityId { get; set; }
        public string LocalDateTime { get; set; }


    }
}
