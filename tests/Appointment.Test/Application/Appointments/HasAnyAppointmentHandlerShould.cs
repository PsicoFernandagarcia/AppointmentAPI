using Appointment.Application.AppointmentUseCases.HasAnyAppointment;
using Appointment.Domain.Interfaces;
using Moq;
using Xunit;

namespace Appointment.Test.Application.Appointments
{
    public class HasAnyAppointmentHandlerShould
    {
        private readonly Mock<IAppointmentRepository> _appointmentRepository = new();
        private readonly HasAnyAppointmentHandler _handler;
        public HasAnyAppointmentHandlerShould()
        {
            _handler = new(_appointmentRepository.Object);
        }

        [Fact]
        public async Task Call_Repository_To_Check_If_Has_Appointments()
        {
            var request = new HasAnyAppointmentQuery();
            await _handler.Handle(request, CancellationToken.None);
            _appointmentRepository.Verify(ar => ar.HasAnyAppointment(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }
    }
}
