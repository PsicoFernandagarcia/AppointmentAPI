using Appointment.Application.AppointmentUseCases.CancelAppointment;
using Appointment.Application.AvailabilityUseCases.ChangeAvailabilityStatus;
using Appointment.Application.PaymentUseCases.UpdateLatestPaymentSessions;
using Appointment.Application.SendEmailUseCase.AppointmentCancelation;
using Appointment.Domain;
using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using Appointment.Infrastructure.Configuration;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using System.Data.Common;
using Xunit;
using Entities = Appointment.Domain.Entities;

namespace Appointment.Test.Application.Appointments
{
    public class CancelAppointmentHandlerShould
    {
        private readonly Mock<IUserRepository> _userRepository = new();
        private readonly Mock<IAppointmentRepository> _appointmentRepository = new();
        private readonly Mock<IMediator> _mediator = new();
        private readonly CancelAppointmentHandler _handler;
        private readonly Mock<IOutputCacheStore> _cacheStore = new();

        public CancelAppointmentHandlerShould()
        {
            var app = MockAppDbContext.GetMock();
            _handler = new(_userRepository.Object, _appointmentRepository.Object, _mediator.Object, app.Object, _cacheStore.Object);
        }

        [Fact]
        public async Task Not_Cancel_Appointment_Because_User_Does_Not_Exists()
        {
            var request = new CancelAppointmentsCommand(1, 2);
            _userRepository.Setup(ur => ur.GetUserById(It.IsAny<int>())).ReturnsAsync(null as User);

            var result = await _handler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().BeOfType<ResultError>();
            _appointmentRepository.Verify(ar => ar.Update(It.IsAny<Entities.Appointment>()), Times.Never);
            _mediator.Verify(mr => mr.Send(It.IsAny<ChangeAvailabilityStatusCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediator.Verify(mr => mr.Send(It.IsAny<UpdateLastPaymentSessionsCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediator.Verify(mr => mr.Send(It.IsAny<SendAppointmentCancelationEmailCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _cacheStore.Verify(cs => cs.EvictByTagAsync(CacheKeys.Appointments, It.IsAny<CancellationToken>()), Times.Never);

        }

        [Fact]
        public async Task Not_Cancel_Appointment_Due_To_Wrong_Patient_Or_Host_Associated()
        {
            var request = new CancelAppointmentsCommand(1, 2);
            _userRepository.Setup(ur => ur.GetUserById(It.IsAny<int>()))
                .ReturnsAsync(User.Create(1, "test", "email", null, null, null, true, "test", "test", 10).Value);

            _appointmentRepository.Setup(ar => ar.GetById(It.IsAny<int>()))
                .ReturnsAsync(Entities.Appointment.Create(1, "Test", DateTime.Now.AddDays(1), DateTime.Now.AddDays(1).AddHours(1), "Joaquin", 3, "", false, 2, 3, Appointment.Domain.AppointmentStatus.CREATED, DateTime.Now).Value);

            var result = await _handler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().BeOfType<ResultError>();
            result.Error.Message.Should().Be("Appointment not valid or you don't have permissions to do this");
            _appointmentRepository.Verify(ar => ar.Update(It.IsAny<Entities.Appointment>()), Times.Never);
            _mediator.Verify(mr => mr.Send(It.IsAny<ChangeAvailabilityStatusCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediator.Verify(mr => mr.Send(It.IsAny<UpdateLastPaymentSessionsCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediator.Verify(mr => mr.Send(It.IsAny<SendAppointmentCancelationEmailCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _cacheStore.Verify(cs => cs.EvictByTagAsync(CacheKeys.Appointments, It.IsAny<CancellationToken>()), Times.Never);

        }

        [Fact]
        public async Task Not_Cancel_Appointment_Because_Has_Lower_Date_Than_Today_And_Is_Not_Host()
        {
            var request = new CancelAppointmentsCommand(3, 2);
            _userRepository.Setup(ur => ur.GetUserById(It.IsAny<int>()))
                .ReturnsAsync(User.Create(1, "test", "email", null, null, null, true, "test", "test", 10).Value);

            _appointmentRepository.Setup(ar => ar.GetById(It.IsAny<int>()))
                .ReturnsAsync(Entities.Appointment.Create(1, "Test", DateTime.Now.AddDays(-1), DateTime.Now.AddDays(-1).AddHours(1), "Joaquin", 3, "", false, 2, 3, Appointment.Domain.AppointmentStatus.CREATED, DateTime.Now).Value);

            var result = await _handler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().BeOfType<ResultError>();
            result.Error.Message.Should().Be("Appointment not valid or you don't have permissions to do this");
            _appointmentRepository.Verify(ar => ar.Update(It.IsAny<Entities.Appointment>()), Times.Never);
            _mediator.Verify(mr => mr.Send(It.IsAny<ChangeAvailabilityStatusCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediator.Verify(mr => mr.Send(It.IsAny<UpdateLastPaymentSessionsCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediator.Verify(mr => mr.Send(It.IsAny<SendAppointmentCancelationEmailCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _cacheStore.Verify(cs => cs.EvictByTagAsync(CacheKeys.Appointments, It.IsAny<CancellationToken>()), Times.Never);

        }

        [Fact]
        public async Task Not_Cancel_Appointment_Because_Is_In_24_Hours_Or_Less_And_Is_Not_Host()
        {
            var request = new CancelAppointmentsCommand(1, 2);
            _userRepository.Setup(ur => ur.GetUserById(It.IsAny<int>()))
                .ReturnsAsync(User.Create(1, "test", "email", null, null, null, true, "test", "test", 10).Value);

            _appointmentRepository.Setup(ar => ar.GetById(It.IsAny<int>()))
                .ReturnsAsync(Entities.Appointment.Create(1, "Test", DateTime.Now.AddHours(24), DateTime.Now.AddHours(26), "Joaquin", 3, "", false, 2, 1, Appointment.Domain.AppointmentStatus.CREATED, DateTime.Now).Value);

            var result = await _handler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().BeOfType<ResultError>();
            result.Error.Message.Should().Be("Appointment not valid or you don't have permissions to do this");
            _appointmentRepository.Verify(ar => ar.Update(It.IsAny<Entities.Appointment>()), Times.Never);
            _mediator.Verify(mr => mr.Send(It.IsAny<ChangeAvailabilityStatusCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediator.Verify(mr => mr.Send(It.IsAny<UpdateLastPaymentSessionsCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediator.Verify(mr => mr.Send(It.IsAny<SendAppointmentCancelationEmailCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _cacheStore.Verify(cs => cs.EvictByTagAsync(CacheKeys.Appointments, It.IsAny<CancellationToken>()), Times.Never);

        }

        [Fact]
        public async Task Cancel_Appointment_Because_Has_Lower_Date_Than_Today_And_Is_Host()
        {
            int hostId = 10;
            var request = new CancelAppointmentsCommand(hostId, 2);
            _userRepository.Setup(ur => ur.GetUserById(It.IsAny<int>()))
                .ReturnsAsync(User.Create(2, "test", "email", null, null, null, true, "test", "test", 10).Value);

            _appointmentRepository.Setup(ar => ar.GetById(It.IsAny<int>()))
                .ReturnsAsync(Entities.Appointment.Create(1, "Test", DateTime.Now.AddDays(-1), DateTime.Now.AddDays(-1).AddHours(1), "Joaquin", 3, "", false, hostId, 3, Appointment.Domain.AppointmentStatus.CREATED, DateTime.Now).Value);

            var result = await _handler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            _appointmentRepository.Verify(ar => ar.Update(It.IsAny<Entities.Appointment>()), Times.Once);
            _mediator.Verify(mr => mr.Send(It.IsAny<ChangeAvailabilityStatusCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediator.Verify(mr => mr.Send(It.IsAny<UpdateLastPaymentSessionsCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediator.Verify(mr => mr.Send(It.IsAny<SendAppointmentCancelationEmailCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _cacheStore.Verify(cs => cs.EvictByTagAsync(CacheKeys.Appointments, It.IsAny<CancellationToken>()), Times.Once);

        }

        [Fact]
        public async Task Cancel_Appointment_By_Host_Because_Is_Valid_To_Be_Canceled()
        {
            var request = new CancelAppointmentsCommand(1, 2);
            _userRepository.Setup(ur => ur.GetUserById(It.IsAny<int>()))
                .ReturnsAsync(User.Create(1, "test", "email", null, null, null, true, "test", "test", 10).Value);

            _appointmentRepository.Setup(ar => ar.GetById(It.IsAny<int>()))
                .ReturnsAsync(Entities.Appointment.Create(1, "Test", DateTime.Now.AddDays(1), DateTime.Now.AddDays(1).AddHours(1), "Joaquin", 3, "", false, 1, 3, Appointment.Domain.AppointmentStatus.CREATED, DateTime.Now).Value);

            var result = await _handler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            _appointmentRepository.Verify(ar => ar.Update(It.IsAny<Entities.Appointment>()), Times.Once);
            _mediator.Verify(mr => mr.Send(It.IsAny<ChangeAvailabilityStatusCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediator.Verify(mr => mr.Send(It.IsAny<UpdateLastPaymentSessionsCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediator.Verify(mr => mr.Send(It.IsAny<SendAppointmentCancelationEmailCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _cacheStore.Verify(cs => cs.EvictByTagAsync(CacheKeys.Appointments, It.IsAny<CancellationToken>()), Times.Once);

        }

        [Fact]
        public async Task Cancel_Appointment_By_Patient_Because_Is_Valid_To_Be_Canceled()
        {
            var request = new CancelAppointmentsCommand(2, 2);
            _userRepository.Setup(ur => ur.GetUserById(It.IsAny<int>()))
                .ReturnsAsync(User.Create(1, "test", "email", null, null, null, true, "test", "test", 10).Value);

            _appointmentRepository.Setup(ar => ar.GetById(It.IsAny<int>()))
                .ReturnsAsync(Entities.Appointment.Create(1, "Test", DateTime.Now.AddDays(1), DateTime.Now.AddDays(1).AddHours(1), "Joaquin", 3, "", false, 2, 1, Appointment.Domain.AppointmentStatus.CREATED, DateTime.Now).Value);

            var result = await _handler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            _appointmentRepository.Verify(ar => ar.Update(It.IsAny<Entities.Appointment>()), Times.Once);
            _mediator.Verify(mr => mr.Send(It.IsAny<ChangeAvailabilityStatusCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediator.Verify(mr => mr.Send(It.IsAny<UpdateLastPaymentSessionsCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediator.Verify(mr => mr.Send(It.IsAny<SendAppointmentCancelationEmailCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _cacheStore.Verify(cs => cs.EvictByTagAsync(CacheKeys.Appointments, It.IsAny<CancellationToken>()), Times.Once);

        }

    }
}
