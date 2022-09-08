using Appointment.Application.AppointmentUseCases.GetMyAppointment;
using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using FluentAssertions;
using Moq;
using Xunit;
using Entities = Appointment.Domain.Entities;

namespace Appointment.Test.Application.Appointments
{
    public class GetMyAppointmentsHandlerShould
    {
        private readonly Mock<IUserRepository> _userRepository = new();
        private readonly Mock<IAppointmentRepository> _appointmentRepository = new();
        private readonly GetMyAppointmentsHandler _handler;
        public GetMyAppointmentsHandlerShould()
        {
            _handler = new(_userRepository.Object, _appointmentRepository.Object);
        }

        [Fact]
        public async Task Return_Error_When_User_Does_Not_Exists()
        {
            var request = new GetMyAppointmentsQuery(1, DateTime.Now);
            _userRepository.Setup(x => x.GetUserById(It.IsAny<int>()))
                .ReturnsAsync(null as User);

            var result = await _handler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().BeOfType<ResultError>();
            result.Error.Message.Should().Be("User not found or you don't have permissions to do this");
        }

        [Fact]
        public async Task Return_List_With_Zero_Filtered_Items()
        {
            var request = new GetMyAppointmentsQuery(1, DateTime.Now);
            _userRepository.Setup(x => x.GetUserById(It.IsAny<int>()))
                .ReturnsAsync(User.Create(1, "test", "email", null, null, null, true, "test", "test", 10).Value);

            _appointmentRepository.Setup(x => x.GetByUserId(It.IsAny<int>(), It.IsAny<DateTime>()))
               .ReturnsAsync(new List<Entities.AppointmentDto>());

            var result = await _handler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(0);
        }

        [Fact]
        public async Task Return_List_With_Filtered_Items()
        {
            var request = new GetMyAppointmentsQuery(1, DateTime.Now);
            _userRepository.Setup(x => x.GetUserById(It.IsAny<int>()))
                .ReturnsAsync(User.Create(1, "test", "email", null, null, null, true, "test", "test", 10).Value);

            _appointmentRepository.Setup(x => x.GetByUserId(It.IsAny<int>(), It.IsAny<DateTime>()))
               .ReturnsAsync(new List<Entities.AppointmentDto>() { new(), new() });

            var result = await _handler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);
        }
    }
}
