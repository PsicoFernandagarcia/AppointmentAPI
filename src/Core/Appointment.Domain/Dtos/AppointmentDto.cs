using System;
using System.ComponentModel.DataAnnotations;

namespace Appointment.Domain.Entities
{
    public class AppointmentDto
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string With { get; set; }
        public int CreatedById { get; set; }
        public string CreatedBy { get; set; }
        public int HostId { get; set; }
        public string HostName { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string Color { get; set; }
        public bool IsDeleted { get; set; }
        public string Status { get; set; }
        public DateTime UpdatedAt { get; set; }

        public AppointmentDto()
        {

        }
    }
}