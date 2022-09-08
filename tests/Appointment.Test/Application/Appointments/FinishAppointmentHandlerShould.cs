using Appointment.Application.AppointmentUseCases.FinishAppointments;
using Appointment.Domain.Interfaces;
using Moq;
using Xunit;

namespace Appointment.Test.Application.Appointments
{
    public class FinishAppointmentHandlerShould
    {
        private readonly Mock<IAppointmentRepository> _appointmentRepository = new();
        private readonly FinishAppointmentsHandler _handler;
        public FinishAppointmentHandlerShould()
        {
            _handler = new(_appointmentRepository.Object);
        }

        [Fact]
        public async Task Call_Repository_To_Finalize_Appointments()
        {
            var request = new FinishAppointmentsCommand();
            await _handler.Handle(request, CancellationToken.None);
            _appointmentRepository.Verify(ar => ar.FinalizeAppointments(), Times.Once);
        }
    }
}
