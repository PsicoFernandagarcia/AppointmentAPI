using CSharpFunctionalExtensions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Appointment.Domain.Entities
{
    public class Payment
    {
        [Key]
        public int Id { get; private set; }
        public DateTime PaidAt { get; private set; }
        public int PatientId { get; private set; }
        [ForeignKey("PatientId")]
        public virtual User Patient { get; private set; }
        public int HostId { get; private set; }
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }
        public int SessionsPaid { get; private set; }
        //If negative, the user needs to pay
        public int SessionsLeft { get; set; }

        protected Payment()
        {

        }

        private Payment(int id, DateTime paidAt, int patientId, int hostId, decimal amount, string currency, int sessionsPaid, int sessionsLeft)
        {
            Id = id;
            PaidAt = paidAt.ToUniversalTime();
            PatientId = patientId;
            HostId = hostId;
            Amount = amount;
            SessionsPaid = sessionsPaid;
            SessionsLeft = sessionsLeft;
            Currency = currency;
        }

        public static Result<Payment> Create(int id, DateTime paidAt, int patientId, int hostId, decimal amount, string currency, int sessionsPaid, int sessionsLeft)
        {
            var validation = Validate(id, paidAt, patientId, hostId, amount, sessionsPaid);
            if (validation.IsFailure) return Result.Failure<Payment>(validation.Error);
            return new Payment(id, paidAt, patientId, hostId, amount, currency, sessionsPaid, sessionsLeft);
        }

        private static Result<string> Validate(int id, DateTime paidAt, int patientId, int hostId, decimal amount, int sessionsPaid)
        {
            string errors = string.Empty;
            if (id < 0) errors += " id not valid ";
            if (paidAt > DateTime.Now) errors += " paidAt not valid ";
            if (patientId <= 0) errors += " patientId not valid ";
            if (hostId <= 0) errors += " hostId not valid ";
            if (amount < 0) errors += " amount not valid ";
            if (sessionsPaid < 0) errors += " sessionsPaid not valid ";
            if (!string.IsNullOrWhiteSpace(errors)) return Result.Failure<string>(errors);
            return Result.Success(string.Empty);
        }
    }
}
