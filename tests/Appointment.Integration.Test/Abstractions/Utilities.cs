﻿using Appointment.Domain;
using Appointment.Domain.Entities;
using Appointment.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

        public static async Task<List<Appointment.Domain.Entities.Appointment>> InsertAppointments(TestWebApplicationFactory factory, List<Appointment.Domain.Entities.Appointment>? appointments = null)
        {
            using var scope = factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            if(appointments is null || !appointments.Any())
            {
                var app1 = Appointment.Domain.Entities.Appointment.Create(0,
                                                          "App1",
                                                          DateTime.Now,
                                                          DateTime.Now.AddMinutes(60),
                                                          "me",
                                                          UserHost.Id,
                                                          "color",
                                                          false,
                                                          UserHost.Id,
                                                          UserCommon.Id,
                                                          AppointmentStatus.CREATED,
                                                          DateTime.Now,
                                                          null).Value;
                appointments = [app1];
            }
            await context.Appointments.AddRangeAsync(appointments);
            await context.SaveChangesAsync();
            return appointments;
        }

        public static async Task<List<Availability>> InsertAvailabilities(TestWebApplicationFactory factory, List<Availability>? availabilities = null)
        {
            using var scope = factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            if (availabilities is null || !availabilities.Any())
            {
                var av = Availability.Create(0, UserHost.Id, DateTime.UtcNow, 60, true).Value;
                availabilities = [av];
            }
            await context.Availabilities.AddRangeAsync(availabilities);
            await context.SaveChangesAsync();
            return availabilities;
        }
        public static async Task InsertPayments(TestWebApplicationFactory factory, List<Payment> payments)
        {
            using var scope = factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await context.Payments.AddRangeAsync(payments);
            await context.SaveChangesAsync();
        }


        //public static async Task DeleteAllModels(TestWebApplicationFactory factory, List<int>? paymentIds = null, List<int>? appointmentIds = null)
        public static async Task DeleteAllModels(TestWebApplicationFactory factory)
        {
            using var scope = factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await context.Appointments.ExecuteDeleteAsync();
            await context.Payments.ExecuteDeleteAsync();
            //if (appointmentIds != null)
            //{
            //    await context.Appointments.Where(entity => appointmentIds.Contains(entity.Id)).ExecuteDeleteAsync();
            //    await context.SaveChangesAsync();
            //}
            //if (paymentIds != null)
            //{
            //    await context.Payments.Where(entity => paymentIds.Contains(entity.Id)).ExecuteDeleteAsync();
            //    await context.SaveChangesAsync();
            //}
        }

    }
}
