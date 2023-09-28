using System;

namespace Appointment.Domain.Entities
{
    public class AvailabilityDto
    {
        public int Id { get; set; }

        public int HostId { get; set; }
        public DateTime DateOfAvailability { get; set; }
        public int AmountOfTime { get; set; }
        public bool IsEmpty { get; set; }
        public int AppointmentId { get; set; }
        public string AppointmentWith { get; set; }
        public AvailabilityDto()
        {

        }

        public AvailabilityDto(int id, int hostId, DateTime dateOfAvailability, int amountOfTime, bool isEmpty, int appointmentId = 0, string appointmentWith = "")
        {
            Id = id;
            HostId = hostId;
            DateOfAvailability = dateOfAvailability;
            AmountOfTime = amountOfTime;
            IsEmpty = isEmpty;
            AppointmentId = appointmentId;
            AppointmentWith = appointmentWith;
        }
    }
}