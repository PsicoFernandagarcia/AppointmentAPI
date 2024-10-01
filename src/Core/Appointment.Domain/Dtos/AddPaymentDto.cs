using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Appointment.Domain.Entities
{
    public class AddPaymentDto
    {
        public int PatientId { get; set; }
        public int HostId { get; set; }
        public decimal Amount { get; set; }
        public int SessionsPaid { get; set; }
        public string Currency { get; set; }
        public DateTime PaidAt { get; set; }
        public string? Observations { get; set; }
        public IEnumerable<int> Appointments { get; set; }
    }

    public class AddPaymentResponseDto
    {
        public int Id { get; set; }
        public DateTime PaidAt { get; set; }
        public int PatientId { get; set; }
        public User Patient { get; set; }
        public int HostId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public int SessionsPaid { get; set; }
        public int SessionsLeft { get; set; }
        public string? Observations { get; set; }
    }
}