using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;

namespace Appointment.Domain.Entities
{
    public class Availability
    {
        [Key]
        public int Id { get; set; }

        public int HostId { get; set; }
        [IgnoreDataMember]
        public virtual User Host{ get; set; }
        //in utc
        public DateTime DateOfAvailability { get; set; }

        //in minutes
        public int AmountOfTime{ get; set; }
        public bool IsEmpty { get; set; }

        protected  Availability()
        {
            
        }

        public Availability(int id, int hostId, DateTime dateOfAvailability, int amountOfTime, bool isEmpty)
        {
            Id = id;
            HostId = hostId;
            DateOfAvailability = dateOfAvailability.ToLocalTime();
            AmountOfTime = amountOfTime;
            IsEmpty = isEmpty;
        }
        public static Result<Availability> Create(int id, int hostId, DateTime dateOfAvailability, int amountOfTime, bool isEmpty)
        {
            var validation = Validate(id, hostId, dateOfAvailability, amountOfTime);
            if (validation.IsFailure) return Result.Failure<Availability>(validation.Error);
            return new Availability(id, hostId, dateOfAvailability, amountOfTime, isEmpty);
        }

        private static Result<string> Validate(int id, int hostId, DateTime dateOfAvailability, int amountOfTime)
        {
            string errors = string.Empty;
            if (id < 0) errors += " id not valid ";
            if (hostId < 0) errors += " host id not valid ";
            if (amountOfTime < 0) errors += " time should be greater than 10 minutes ";
            if (!string.IsNullOrWhiteSpace(errors)) return Result.Failure<string>(errors);
            return Result.Success(string.Empty);
        }

    }

}