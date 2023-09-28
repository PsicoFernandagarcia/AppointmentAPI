using Appointment.Application.AppointmentUseCases.AddAppointment;
using Appointment.Application.AvailabilityUseCases.AppointmentConfigured;
using Appointment.Application.PaymentUseCases.UpdateLatestPaymentSessions;
using Appointment.Application.SendEmailUseCase.AppointmentConfirmation;
using Appointment.Domain;
using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using FluentAssertions;
using MediatR;
using Moq;
using Xunit;
using Entities = Appointment.Domain.Entities;

namespace Appointment.Test.Application.Appointments
{
    public class CreateAppointmentHandlerShould
    {
        private readonly Mock<IAppointmentRepository> _appointmentRepository = new();
        private readonly Mock<IMediator> _mediator = new();
        private readonly CreateAppointmentHandler _handler;

        public CreateAppointmentHandlerShould()
        {
            var app = MockAppDbContext.GetMock();
            _handler = new(_appointmentRepository.Object, _mediator.Object, app.Object);
        }

        [Fact]
        public async Task Create_Appointment_To_Patient_Host()
        {
            var request = new CreateAppointmentCommand("Test", DateTime.Now, DateTime.Now.AddHours(1), "Joaquin", 1, 1, 2, "", 1, DateTime.Now.ToShortDateString());

            _mediator.Setup(m => m.Send(It.IsAny<AppointmentConfiguredCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success<Availability, ResultError>(Availability.Create(1, 1, DateTime.Now, 1, true).Value));

            var appointmentResult = Entities.Appointment.Create(1, "test", DateTime.Now.AddHours(1), DateTime.Now.AddHours(2), "Joaquin", 1, "",false,1,2,AppointmentStatus.CREATED,DateTime.Now).Value;
            _appointmentRepository.Setup(ar => ar.Create(It.IsAny<Entities.Appointment>())).ReturnsAsync(appointmentResult);

            var result = await _handler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            _mediator.Verify(m => m.Send(It.IsAny<AppointmentConfiguredCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediator.Verify(m => m.Send(It.IsAny<UpdateLastPaymentSessionsCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediator.Verify(m => m.Send(It.IsAny<SendAppointmentConfirmationEmailCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _appointmentRepository.Verify(ar => ar.Create(It.IsAny<Entities.Appointment>()), Times.Once);
        }

        [Fact]
        public async Task Fail_Create_Appointment_Due_To_Invalid_Parameters()
        {
            var request = new CreateAppointmentCommand("", DateTime.Now, DateTime.Now.AddHours(1), "Joaquin", 1, 1, 2, "", 1, DateTime.Now.ToShortDateString());

            _mediator.Setup(m => m.Send(It.IsAny<AppointmentConfiguredCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success<Availability, ResultError>(Availability.Create(1, 1, DateTime.Now, 1, true).Value));

            var result = await _handler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().BeOfType<CreationError>();

            _mediator.Verify(m => m.Send(It.IsAny<AppointmentConfiguredCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediator.Verify(m => m.Send(It.IsAny<UpdateLastPaymentSessionsCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediator.Verify(m => m.Send(It.IsAny<SendAppointmentConfirmationEmailCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _appointmentRepository.Verify(ar => ar.Create(It.IsAny<Entities.Appointment>()), Times.Never);
        }

    }
}
