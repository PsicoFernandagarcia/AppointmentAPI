using Appointment.Domain.Entities;
using Appointment.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Application.Integration.Test.Abstractions
{
    public static class Utilities
    {
        public static readonly User UserHost = User.Create(1, "test", "test@gmail.com", [], [], [new Role { Id = 1, Name = "HOST" }], true, "test", "LN", 60).Value;
        public static readonly User UserCommon = User.Create(2, "common", "common@gmail.com", [], [], [new Role { Id = 2, Name = "COMMON" }], true, "common", "LN", 60).Value;
        public static void InitializeDbForTests(AppDbContext db)
        {
            if (db.Users.Any(u => u.Id == UserHost.Id))
            {
                return;
            };
            db.Users.AddRange([UserHost,UserCommon]);
            db.SaveChanges();
        }

    }
}
