using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace Appointment.Domain.Entities
{
    public class LastPaymentDto
    {
        public int Id { get;  set; }
        public DateTime PaidAt { get;  set; }
        public int PatientId { get;  set; }
        [ForeignKey("PatientId")]
        public User Patient { get;  set; }
        public int HostId { get;  set; }
        public decimal Amount { get;  set; }
        public string Currency { get;  set; }
        public int SessionsPaid { get;  set; }
        //If negative, the user needs to pay
        public int SessionsLeft { get; set; }
        public string? Observations { get;  set; }

        public static LastPaymentDto FromPaymnet(Payment entity)
        {
            return new LastPaymentDto
            {
                Id = entity.Id,
                PaidAt = entity.PaidAt,
                Patient = entity.Patient,
                Amount = entity.Amount,
                Currency = entity.Currency,
                HostId = entity.HostId,
                Observations = entity.Observations,
                PatientId = entity.PatientId,
                SessionsLeft = entity.SessionsLeft,
                SessionsPaid = entity.SessionsPaid,
            };
        }
    }
}