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
    }
}