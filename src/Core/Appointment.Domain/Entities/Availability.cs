using CSharpFunctionalExtensions;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Appointment.Domain.Entities
{
    public class Availability
    {
        [Key]
        public int Id { get; private set; }

        public int HostId { get; private set; }
        [IgnoreDataMember]
        public virtual User Host { get; private set; }
        //in utc
        public DateTime DateOfAvailability { get; private set; }

        //in minutes
        public int AmountOfTime { get; private set; }
        public bool IsEmpty { get; set; }

        protected Availability()
        {

        }

        private Availability(int id, int hostId, DateTime dateOfAvailability, int amountOfTime, bool isEmpty)
        {
            Id = id;
            HostId = hostId;
            DateOfAvailability = dateOfAvailability.ToLocalTime();
            AmountOfTime = amountOfTime;
            IsEmpty = isEmpty;
        }
        public static Result<Availability> Create(int id, int hostId, DateTime dateOfAvailability, int amountOfTime, bool isEmpty)
        {
            var validation = Validate(id, hostId, amountOfTime);
            if (validation.IsFailure) return Result.Failure<Availability>(validation.Error);
            return new Availability(id, hostId, dateOfAvailability, amountOfTime, isEmpty);
        }

        private static Result<string> Validate(int id, int hostId, int amountOfTime)
        {
            string errors = string.Empty;
            if (id < 0) errors += " id not valid ";
            if (hostId <= 0) errors += " host id not valid ";
            if (amountOfTime <= 0) errors += " time should be greater than 0 minutes ";
            if (!string.IsNullOrWhiteSpace(errors)) return Result.Failure<string>(errors);
            return Result.Success(string.Empty);
        }

    }

}