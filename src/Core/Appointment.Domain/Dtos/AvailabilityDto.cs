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

        public AvailabilityDto()
        {

        }

        public AvailabilityDto(int id, int hostId, DateTime dateOfAvailability, int amountOfTime, bool isEmpty)
        {
            Id = id;
            HostId = hostId;
            DateOfAvailability = dateOfAvailability;
            AmountOfTime = amountOfTime;
            IsEmpty = isEmpty;
        }
    }
}