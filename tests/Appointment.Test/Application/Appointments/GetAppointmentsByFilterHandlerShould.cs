using Appointment.Application.AppointmentUseCases.GetAppointmentsByFilter;
using Appointment.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;
using Entities = Appointment.Domain.Entities;

namespace Appointment.Test.Application.Appointments
{
    public class GetAppointmentsByFilterHandlerShould
    {
        private readonly Mock<IAppointmentRepository> _appointmentRepository = new();
        private readonly GetAppointmentsByFilterHandler _handler;
        public GetAppointmentsByFilterHandlerShould()
        {
            _handler = new(_appointmentRepository.Object);
        }

        [Fact]
        public async Task Return_Empty_List_If_No_Data()
        {
            var request = new GetAppointmentsByFilterQuery();
            _appointmentRepository.Setup(x => x.GetByFilter(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new List<Entities.AppointmentDto>());

            var result = await _handler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(0);
        }

        [Fact]
        public async Task Return_List_With_Filtered_Items()
        {
            var request = new GetAppointmentsByFilterQuery();
            _appointmentRepository.Setup(x => x.GetByFilter(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new List<Entities.AppointmentDto>()
                {
                    new(),
                    new()
                });

            var result = await _handler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);
        }
    }
}
