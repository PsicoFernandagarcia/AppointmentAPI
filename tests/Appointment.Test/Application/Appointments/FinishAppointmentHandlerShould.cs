using Appointment.Application.AppointmentUseCases.FinishAppointments;
using Appointment.Domain;
using Appointment.Domain.Interfaces;
using Microsoft.AspNetCore.OutputCaching;
using Moq;
using Xunit;

namespace Appointment.Test.Application.Appointments
{
    public class FinishAppointmentHandlerShould
    {
        private readonly Mock<IAppointmentRepository> _appointmentRepository = new();
        private readonly FinishAppointmentsHandler _handler;
        private readonly Mock<IOutputCacheStore> _cacheStore = new();

        public FinishAppointmentHandlerShould()
        {
            _handler = new(_appointmentRepository.Object, _cacheStore.Object);
        }

        [Fact]
        public async Task Call_Repository_To_Finalize_Appointments()
        {
            var request = new FinishAppointmentsCommand();
            await _handler.Handle(request, CancellationToken.None);
            _appointmentRepository.Verify(ar => ar.FinalizeAppointments(), Times.Once);
            _cacheStore.Verify(cs => cs.EvictByTagAsync(CacheKeys.Appointments, It.IsAny<CancellationToken>()), Times.Once);

        }
    }
}
