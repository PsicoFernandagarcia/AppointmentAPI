using Application.Integration.Test.Abstractions;
using Appointment.Application.PaymentUseCases.AddPayment;
using Appointment.Domain;
using Appointment.Domain.Entities;
using Appointment.Infrastructure.Configuration;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Appointment.Integration.Test.PaymentsCase
{
    public class CreatePaymentTest(TestWebApplicationFactory factory) : BaseIntegrationTest(factory)
    {

        [Fact]
        public async Task Should_InsertNewPayment()
        {
            var host = Utilities.UserHost;
            var patient = Utilities.UserCommon;
            var app1 = Domain.Entities.Appointment.Create(0,
                                                          "App1",
                                                          DateTime.Now,
                                                          DateTime.Now.AddMinutes(60),
                                                          "me",
                                                          host.Id,
                                                          "color",
                                                          false,
                                                          host.Id,
                                                          patient.Id,
                                                          AppointmentStatus.CREATED,
                                                          DateTime.Now,
                                                          null).Value;
            var app2 = Domain.Entities.Appointment.Create(0,
                                                          "App2",
                                                          DateTime.Now,
                                                          DateTime.Now.AddMinutes(60),
                                                          "me",
                                                          host.Id,
                                                          "color",
                                                          false,
                                                          host.Id,
                                                          patient.Id,
                                                          AppointmentStatus.CREATED,
                                                          DateTime.Now,
                                                          null).Value;
            List<Domain.Entities.Appointment> appList = [app1, app2];
            await Utilities.InsertAppointments(factory, appList);
            var command = new CreatePaymentCommand
            {
                Amount = 100,
                Appointments = [app1.Id, app2.Id],
                Currency = "EUR",
                HostId = host.Id,
                PatientId = patient.Id,
                PaidAt = DateTime.Now,
                SessionsPaid = 1
            };
            var res = await HttpClient.PostAsJsonAsync("api/payments", command);
            res.StatusCode.Should().Be(HttpStatusCode.OK);
            var resultObject = await res.ToObject<AddPaymentResponseDto>();
            resultObject.Amount.Should().Be(100);
            using var scope = factory.Services.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<AppDbContext>();
            var dbAppointments = await db.Appointments.Where(a => appList.Select(ap => ap.Id).Contains(a.Id) && a.PaymentId != null).ToListAsync();
            dbAppointments.Should().HaveCount(2);




        }

    }
}
