using Appointment.Application.AvailabilityUseCases.ChangeAvailabilityStatus;
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
    public class ChangeAvailabilityStatusHandlerShould
    {
        private readonly Mock<IUserRepository> _userRepository = new();
        private readonly Mock<IAvailabilityRepository> _availabilityRepository = new();
        private readonly ChangeAvailabilityStatusHandler _handler;
        private readonly Mock<IOutputCacheStore> _cacheStore = new();

        public ChangeAvailabilityStatusHandlerShould()
        {
            _handler = new(_userRepository.Object, _availabilityRepository.Object, _cacheStore.Object);
        }

        [Fact]
        public async Task Update_Status_To_Existing_Availability()
        {
            var request = new ChangeAvailabilityStatusCommand(1, true, 20);
            _userRepository.Setup(x => x.GetUserById(It.IsAny<int>()))
                 .ReturnsAsync(User.Create(1, "UserName", "email", null, null, null, true, "name", "lastName", 0).Value);

            _availabilityRepository.Setup(x => x.GetByAppointmentId(It.IsAny<int>()))
                    .ReturnsAsync(() => Availability.Create(30, 1, DateTime.Now, 60, false).Value);

            var result = await _handler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            _availabilityRepository.Verify(x => x.Update(It.IsAny<Availability>()), Times.Once);
            _cacheStore.Verify(cs => cs.EvictByTagAsync(CacheKeys.Availabilities, It.IsAny<CancellationToken>()), Times.Once);

        }

        [Fact]
        public async Task Remove_Appointment_Information_On_Update_Status()
        {
            var request = new ChangeAvailabilityStatusCommand(1, true, 30);
            _userRepository.Setup(x => x.GetUserById(It.IsAny<int>()))
                 .ReturnsAsync(User.Create(1, "UserName", "email", null, null, null, true, "name", "lastName", 0).Value);

            _availabilityRepository.Setup(x => x.GetByAppointmentId(It.IsAny<int>()))
             .ReturnsAsync(() => Availability.Create(30, 1, DateTime.Now, 60, false).Value);

            var result = await _handler.Handle(request, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _availabilityRepository.Verify(x => x.Update(It.Is<Availability>(
                av => av.AppointmentId == 0 && av.AppointmentWith == string.Empty
                )), Times.Once);
            _cacheStore.Verify(cs => cs.EvictByTagAsync(CacheKeys.Availabilities, It.IsAny<CancellationToken>()), Times.Once);

        }

        [Fact]
        public async Task Not_Update_Status_Due_To_Invalid_Host()
        {
            var request = new ChangeAvailabilityStatusCommand(1, true, 20);
            _userRepository.Setup(x => x.GetUserById(It.IsAny<int>()))
                 .ReturnsAsync(null as User);

            var result = await _handler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().BeOfType<ResultError>();
            _availabilityRepository.Verify(x => x.Update(It.IsAny<Availability>()), Times.Never);
            _cacheStore.Verify(cs => cs.EvictByTagAsync(CacheKeys.Availabilities, It.IsAny<CancellationToken>()), Times.Never);

        }
    }
}
