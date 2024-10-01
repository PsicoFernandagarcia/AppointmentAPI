using CSharpFunctionalExtensions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Appointment.Domain.Entities
{
    public class Appointment
    {
        [Key]
        public int Id { get; private set; }
        public string Title { get; private set; }
        public DateTime DateFrom { get; private set; }
        public DateTime DateTo { get; private set; }
        public string With { get; private set; }
        public int CreatedById { get; private set; }
        public User CreatedBy { get; private set; }
        public int HostId { get; private set; }
        public User Host { get; private set; }
        public int PatientId { get; private set; }
        public User Patient { get; private set; }
        public string Color { get; private set; }
        public bool IsDeleted { get; private set; }
        public AppointmentStatus Status { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public int? PaymentId { get; private set; }
        [ForeignKey("PaymentId")]
        public Payment? Payment { get; private set; }

        protected Appointment()
        {

        }
        private Appointment(int id, string title, DateTime dateFrom, DateTime dateTo, string with, int createdById, string color
            , bool isDeleted, int hostId, int patientId, AppointmentStatus status, DateTime updatedAt, int? paymentId)
        {
            Id = id;
            Title = title;
            DateFrom = dateFrom.ToLocalTime();
            DateTo = dateTo.ToLocalTime();
            With = with;
            CreatedById = createdById;
            Color = color;
            IsDeleted = isDeleted;
            HostId = hostId;
            PatientId = patientId;
            Status = status;
            UpdatedAt = updatedAt;
            PaymentId = paymentId;
        }


        public static Result<Appointment> Create(int id,
                                                 string title,
                                                 DateTime dateFrom,
                                                 DateTime dateTo,
                                                 string with,
                                                 int createdBy,
                                                 string color,
                                                 bool isDeleted,
                                                 int hostId,
                                                 int patientId,
                                                 AppointmentStatus status,
                                                 DateTime updatedAt,
                                                 int? paymnetId)
        {
            var validation = Validate(id, title, dateFrom, dateTo, with, createdBy, hostId, patientId, updatedAt);
            if (validation.IsFailure) return Result.Failure<Appointment>(validation.Error);
            return new Appointment(id, title, dateFrom, dateTo, with, createdBy, color, isDeleted, hostId, patientId, status, updatedAt, paymnetId);
        }

        private static Result<string> Validate(int id,
                                               string title,
                                               DateTime dateFrom,
                                               DateTime dateTo,
                                               string with,
                                               int createdBy,
                                               int hostId,
                                               int patientId,
                                               DateTime updatedAt)
        {
            string errors = string.Empty;
            if (id < 0) errors += " id not valid ";
            if (hostId <= 0) errors += "host id not valid ";
            if (patientId < 0) errors += "host id not valid ";
            if (updatedAt > DateTime.Now) errors += "updatedAt not valid ";
            if (string.IsNullOrWhiteSpace(title)) errors += " title not valid ";
            if (string.IsNullOrWhiteSpace(with)) errors += " with not valid ";
            if (dateFrom >= dateTo) errors += " date from must be lower than date to ";
            if (createdBy <= 0) errors += " user id not valid ";
            if (!string.IsNullOrWhiteSpace(errors)) return Result.Failure<string>(errors);
            return Result.Success(string.Empty);
        }

        public void ChangeStatus(AppointmentStatus newStatus)
         => this.Status = newStatus;

    }
}