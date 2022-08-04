using CSharpFunctionalExtensions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Appointment.Domain.Entities
{
    public class User
    {
        [Key]
        public int Id { get; private set; }

        public string UserName { get; private set; }
        public string Name { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public virtual ICollection<Role> Roles { get; private set; }
        public bool IsExternal { get; private set; }
        public int TimezoneOffset { get; set; }
        [IgnoreDataMember]
        public byte[] PasswordHash { get; private set; }
        [IgnoreDataMember]
        public byte[] PasswordSalt { get; private set; }
        public virtual IEnumerable<Payment> Payments { get; set; }

        protected User(string lastName)
        {
            LastName = lastName;
        }

        private User(int id, string userName, string email, byte[] passwordHash, byte[] passwordSalt, bool isExternal, ICollection<Role> roles, string lastName, string name, int timezoneOffset)
        {
            Id = id;
            UserName = userName;
            Email = email;
            PasswordHash = passwordHash;
            PasswordSalt = passwordSalt;
            Roles = roles;
            LastName = lastName;
            IsExternal = isExternal;
            Name = name;
            TimezoneOffset = timezoneOffset;
        }


        public static Result<User> Create(int id, string userName, string email, byte[] passwordHash, byte[] passwordSalt,
            ICollection<Role> roles, bool isExternal, string name, string lastName, int timezoneOffset)
        {
            var validation = Validate(id, userName, email, name, lastName);
            if (validation.IsFailure) return Result.Failure<User>(validation.Error);
            return new User(id, userName, email, passwordHash, passwordSalt, isExternal, roles, lastName, name, timezoneOffset);
        }

        private static Result<string> Validate(int id, string userName, string email, string name, string lastName)
        {
            string errors = string.Empty;
            if (id < 0) errors += " id not valid ";
            if (string.IsNullOrWhiteSpace(userName)) errors += " username not valid ";
            if (string.IsNullOrWhiteSpace(email)) errors += " email not valid ";
            if (string.IsNullOrWhiteSpace(name)) errors += " name not valid ";
            if (string.IsNullOrWhiteSpace(lastName)) errors += " lastName not valid ";
            if (!string.IsNullOrWhiteSpace(errors)) return Result.Failure<string>(errors);
            return Result.Success(string.Empty);
        }
    }
}
