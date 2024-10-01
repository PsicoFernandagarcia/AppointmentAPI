using Appointment.Domain.Entities;
using Appointment.Infrastructure.Configuration;
using Docker.DotNet.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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

        public static async Task InsertAppointments(TestWebApplicationFactory factory, List<Appointment.Domain.Entities.Appointment> appointments)
        {
            using var scope = factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await context.AddRangeAsync(appointments);
            await context.SaveChangesAsync();
        }
        public static async Task InsertPayments(TestWebApplicationFactory factory, List<Payment> payments)
        {
            using var scope = factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await context.AddRangeAsync(payments);
            await context.SaveChangesAsync();
        }


        public static async Task DeleteAllModels(TestWebApplicationFactory factory, List<int>? paymentIds = null, List<int>? appointmentIds = null)
        {
            using var scope = factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            if(appointmentIds != null)
            {
                await context.Appointments.Where(entity => appointmentIds.Contains(entity.Id)).ExecuteDeleteAsync();
                await context.SaveChangesAsync();
            }
            if (paymentIds != null)
            {
                await context.Payments.Where(entity => paymentIds.Contains(entity.Id)).ExecuteDeleteAsync();
                await context.SaveChangesAsync();
            }
        }

    }
}
