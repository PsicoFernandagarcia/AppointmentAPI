using Application.Integration.Test.Abstractions;
using Appointment.Application.AppointmentUseCases.AddAppointmentByHost;
using Appointment.Infrastructure.Configuration;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Appointment.Integration.Test.AppointmentsCase
{
    public class CreateAppointmentByHostTest(TestWebApplicationFactory factory) : BaseIntegrationTest(factory)
    {
        [Fact]
        public async Task Should_CreateNewAppointmentByHost()
        {
            var host = Utilities.UserHost;
            var patient = Utilities.UserCommon;
            var availability = (await Utilities.InsertAvailabilities(factory))[0];
            var command = new CreateAppointmentByHostCommand
            {
                AvailabilityId = availability.Id,
                HostId = host.Id,
                PatientId = patient.Id,
                PatientEmail = patient.Email,
                DateFrom = availability.DateOfAvailability,
                DateTo = availability.DateOfAvailability.AddMinutes(60),
                PatientName = patient.Name,
                Title = "New appointment",
                TimezoneOffset = 60,
            };
            var res = await HttpClient.PostAsJsonAsync("api/appointments/HostAssignments", command);
            res.StatusCode.Should().Be(HttpStatusCode.OK);

            using var scope = factory.Services.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<AppDbContext>();
            var app = db.Appointments.First(a => a.PatientId == patient.Id);
            app.Status.Should().Be(Domain.AppointmentStatus.CREATED);
            app.HostId.Should().Be(host.Id);

            var av = db.Availabilities.First(a => a.Id == availability.Id);
            av.AppointmentId.Should().Be(app.Id);
            av.IsEmpty.Should().BeFalse();

            var payments = db.Payments.Where(p => p.PatientId == patient.Id && p.HostId == host.Id).ToList();
            payments.Count().Should().Be(1);
        }
    }
}
