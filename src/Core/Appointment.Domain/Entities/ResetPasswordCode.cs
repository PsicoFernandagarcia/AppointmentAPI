using CSharpFunctionalExtensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace Appointment.Domain.Entities
{
    public class ResetPasswordCode
    {
        [Key]
        public int Id { get; private set; }
        [Required]
        public string  UserEmail { get; private set; }
        [Required]
        public int Code { get; private set; }
        public DateTime EndDate { get; private set; }

        private ResetPasswordCode(int id, string userEmail, int code, DateTime endDate)
        {
            Id = id;
            UserEmail = userEmail;
            Code = code;
            EndDate = endDate;
        }
        public static Result<ResetPasswordCode> Create(int id, string userEmail, int code)
        {
            var validation = Validate(id, userEmail, code);
            if (validation.IsFailure) return Result.Failure<ResetPasswordCode>(validation.Error);
            return new ResetPasswordCode(id, userEmail, code, DateTime.Now.AddMinutes(30));
        }

        private static Result<string> Validate(int id, string userEmail, int code)
        {
            string errors = string.Empty;
            if (id < 0) errors += " id not valid ";
            if (code < 1000) errors += " code not valid ";
            if (string.IsNullOrWhiteSpace(userEmail)) errors += " email not valid ";
            if (!string.IsNullOrWhiteSpace(errors)) return Result.Failure<string>(errors);
            return Result.Success(string.Empty);
        }


    }
}
