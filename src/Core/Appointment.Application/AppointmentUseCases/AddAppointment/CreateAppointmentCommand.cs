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

        public CreateAppointmentCommand()
        {

        }

        public CreateAppointmentCommand(string title,
                                        DateTime dateFrom,
                                        DateTime dateTo,
                                        string with,
                                        int createdById,
                                        int hostId,
                                        int patientId,
                                        string color,
                                        int availabilityId,
                                        string localDateTime)
        {
            Title = title;
            DateFrom = dateFrom;
            DateTo = dateTo;
            With = with;
            CreatedById = createdById;
            HostId = hostId;
            PatientId = patientId;
            Color = color;
            AvailabilityId = availabilityId;
            LocalDateTime = localDateTime;
        }
    }
}
