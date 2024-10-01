using Application.Integration.Test.Abstractions;
using Appointment.Application.AppointmentUseCases.GetAppointmentsByFilter;
using Appointment.Application.PaymentUseCases.AddPayment;
using Appointment.Domain;
using Appointment.Domain.Entities;
using Appointment.Infrastructure.Configuration;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Appointment.Integration.Test.AppointmentsCase
{
    public class GetAppointmentsTest(TestWebApplicationFactory factory) : BaseIntegrationTest(factory)
    {
        [Fact]
        public async Task Should_GetAllAppointmentsFromYear()
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
            var app3 = Domain.Entities.Appointment.Create(0,
                                                          "App2",
                                                          DateTime.Now.AddYears(-1),
                                                          DateTime.Now.AddYears(-1).AddMinutes(60),
                                                          "me",
                                                          host.Id,
                                                          "color",
                                                          false,
                                                          host.Id,
                                                          patient.Id,
                                                          AppointmentStatus.CREATED,
                                                          DateTime.Now,
                                                          null).Value;
            List<Domain.Entities.Appointment> appList = [app1, app2, app3];
            await Utilities.InsertAppointments(factory, appList);
            var query = new GetAppointmentsByFilterQuery
            {
                Year = 2024,
                UserId = patient.Id
            };
            var res = await HttpClient.GetAsync($"api/appointments?year={query.Year}&userId={query.UserId}");
            await Utilities.DeleteAllModels(factory, appointmentIds: appList.Select(a => a.Id).ToList());
            res.StatusCode.Should().Be(HttpStatusCode.OK);
            var resultObject = await res.ToObject<List<AppointmentDto>>();
            resultObject.Should().HaveCount(2);
        }


        [Fact]
        public async Task Should_GetAllUnpaidAppointmentsFromYear()
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
            var app3 = Domain.Entities.Appointment.Create(0,
                                                          "App2",
                                                          DateTime.Now.AddYears(-1),
                                                          DateTime.Now.AddYears(-1).AddMinutes(60),
                                                          "me",
                                                          host.Id,
                                                          "color",
                                                          false,
                                                          host.Id,
                                                          patient.Id,
                                                          AppointmentStatus.CREATED,
                                                          DateTime.Now,
                                                          null).Value;
            List<Domain.Entities.Appointment> appList = [app1, app2, app3];
            var payment = Payment.Create(0, DateTime.Now, patient.Id, host.Id, 100, "user", 1, 1, null, [app1, app3]).Value;
            await Utilities.InsertPayments(factory, [payment]);
            var query = new GetAppointmentsByFilterQuery
            {
                Year = 2024,
                UserId = patient.Id,
                IsUnpaid = true
            };
            var res = await HttpClient.GetAsync($"api/appointments?year={query.Year}&userId={query.UserId}&unpaid={query.IsUnpaid}");
            res.StatusCode.Should().Be(HttpStatusCode.OK);
            var resultObject = await res.ToObject<List<AppointmentDto>>();
            resultObject.Should().HaveCount(1);
            resultObject.First().Id.Should().Be(app1.Id);
            await Utilities.DeleteAllModels(factory, paymentIds: [payment.Id], appointmentIds: appList.Select(a => a.Id).ToList());

        }

    }
}
