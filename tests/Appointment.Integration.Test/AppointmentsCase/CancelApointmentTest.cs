using Application.Integration.Test.Abstractions;
using Appointment.Application.AppointmentUseCases.AddAppointmentByHost;
using Appointment.Application.AppointmentUseCases.CancelAppointment;
using Appointment.Infrastructure.Configuration;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Appointment.Integration.Test.AppointmentsCase
{
    public class CancelAppointmentTest(TestWebApplicationFactory factory) : BaseIntegrationTest(factory)
    {
        [Fact]
        public async Task Should_AllowHostToCancelApointment()
        {
            var host = Utilities.UserHost;
            var patient = Utilities.UserCommon;
            var app = (await Utilities.InsertAppointments(factory))[0];
            var command = new CancelAppointmentsCommand
            {
                AppointmentId = app.Id,
                UserId = host.Id,
            };
            var res = await HttpClient.PostAsJsonAsync($"api/appointments/{app.Id}/Cancel", command);
            res.StatusCode.Should().Be(HttpStatusCode.OK);

            using var scope = factory.Services.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<AppDbContext>();
            var appdb = db.Appointments.First(a => a.Id == app.Id);
            appdb.Status.Should().Be(Domain.AppointmentStatus.CANCELED);

        }
    }
}
