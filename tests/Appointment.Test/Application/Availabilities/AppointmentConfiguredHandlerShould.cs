using Appointment.Application.AvailabilityUseCases.AppointmentConfigured;
using Appointment.Domain;
using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using FluentAssertions;
using Microsoft.AspNetCore.OutputCaching;
using Moq;
using Xunit;

namespace Appointment.Test.Application.Availabilities
{
    public class AppointmentConfiguredHandlerShould
    {
        private readonly Mock<IAvailabilityRepository> _availabilityRepository = new();
        private readonly AppointmentConfiguredHandler _appointmentConfiguredHandler;
        private readonly Mock<IOutputCacheStore> _cacheStore = new();

        public AppointmentConfiguredHandlerShould()
        {
            _appointmentConfiguredHandler = new(_availabilityRepository.Object, _cacheStore.Object);
        }

        [Fact]
        public async Task Set_Empty_Property_To_Availability()
        {
            var request = new AppointmentConfiguredCommand(1, false, 0, string.Empty);
            _availabilityRepository.Setup(x => x.GetById(It.IsAny<int>()))
                .ReturnsAsync(() => Availability.Create(1, 1, DateTime.Now, 10, true).Value);

            var result = await _appointmentConfiguredHandler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType<Availability>();
            result.Value.IsEmpty.Should().BeFalse();
            _availabilityRepository.Verify(x => x.Update(It.IsAny<Availability>()), Times.Once);
            _cacheStore.Verify(cs => cs.EvictByTagAsync(CacheKeys.Availabilities, It.IsAny<CancellationToken>()), Times.Once);

        }

        [Fact]
        public async Task Return_Error_Due_To_Invalid_Availability()
        {
            var request = new AppointmentConfiguredCommand(1, false, 0, string.Empty);
            _availabilityRepository.Setup(x => x.GetById(It.IsAny<int>()))
                .ReturnsAsync(null as Availability);

            var result = await _appointmentConfiguredHandler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().BeOfType<ResultError>();
            _availabilityRepository.Verify(x => x.Update(It.IsAny<Availability>()), Times.Never);
            _cacheStore.Verify(cs => cs.EvictByTagAsync(CacheKeys.Availabilities, It.IsAny<CancellationToken>()), Times.Never);

        }
    }
}
