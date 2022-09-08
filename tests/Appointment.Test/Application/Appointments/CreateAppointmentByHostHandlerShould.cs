using Appointment.Application.AppointmentUseCases.AddAppointment;
using Appointment.Application.AppointmentUseCases.AddAppointmentByHost;
using Appointment.Application.AuthUseCases.CreateUser;
using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using FluentAssertions;
using MediatR;
using Moq;
using Xunit;

namespace Appointment.Test.Application.Appointments
{
    public class CreateAppointmentByHostHandlerShould
    {
        private readonly Mock<IUserRepository> _userRepository = new();
        private readonly Mock<IMediator> _mediator = new();
        private readonly CreateAppointmentByHostHandler _handler;

        public CreateAppointmentByHostHandlerShould()
        {
            _handler = new(_mediator.Object, _userRepository.Object);
        }
        [Fact]
        public async Task Create_Appointment_And_Patient_Because_Does_Not_Exists()
        {
            var request = new CreateAppointmentByHostCommand("Test", DateTime.Now, DateTime.Now.AddHours(1), "Joaco.790@gmail.com", "Joaquin", 1, 1, 0, "", 1, DateTime.Now.ToShortDateString());
            _userRepository.Setup(ur => ur.GetUserByEmail(It.IsAny<string>()))
                .ReturnsAsync(null as User);
            _mediator.Setup(mr => mr.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(User.Create(1, "test", "email", null, null, null, true, "test", "test", 10).Value);

            var result = await _handler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            _userRepository.Verify(ur => ur.GetUserByEmail(It.IsAny<string>()), Times.Once);
            _mediator.Verify(mr => mr.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediator.Verify(mr => mr.Send(It.IsAny<CreateAppointmentCommand>(), It.IsAny<CancellationToken>()), Times.Once);

        }

        [Fact]
        public async Task Create_Only_Appointment_Because_Patient_Does_Exists()
        {
            var request = new CreateAppointmentByHostCommand("Test", DateTime.Now, DateTime.Now.AddHours(1), "Joaco.790@gmail.com", "Joaquin", 1, 1, 0, "", 1, DateTime.Now.ToShortDateString());
            _userRepository.Setup(ur => ur.GetUserByEmail(It.IsAny<string>()))
                .ReturnsAsync(User.Create(1, "test", "email", null, null, null, true, "test", "test", 10).Value);

            _mediator.Setup(mr => mr.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(User.Create(1, "test", "email", null, null, null, true, "test", "test", 10).Value);

            var result = await _handler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            _userRepository.Verify(ur => ur.GetUserByEmail(It.IsAny<string>()), Times.Once);
            _mediator.Verify(mr => mr.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediator.Verify(mr => mr.Send(It.IsAny<CreateAppointmentCommand>(), It.IsAny<CancellationToken>()), Times.Once);

        }

        [Fact]
        public async Task Create_Only_Appointment_With_Patient_Id_From_Request()
        {
            var request = new CreateAppointmentByHostCommand("Test", DateTime.Now, DateTime.Now.AddHours(1), "Joaco.790@gmail.com", "Joaquin", 1, 1, 10, "", 1, DateTime.Now.ToShortDateString());
            _userRepository.Setup(ur => ur.GetUserByEmail(It.IsAny<string>()))
                .ReturnsAsync(User.Create(1, "test", "email", null, null, null, true, "test", "test", 10).Value);

            _mediator.Setup(mr => mr.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(User.Create(1, "test", "email", null, null, null, true, "test", "test", 10).Value);

            var result = await _handler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            _userRepository.Verify(ur => ur.GetUserByEmail(It.IsAny<string>()), Times.Never);
            _mediator.Verify(mr => mr.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediator.Verify(mr => mr.Send(It.IsAny<CreateAppointmentCommand>(), It.IsAny<CancellationToken>()), Times.Once);

        }


    }
}
