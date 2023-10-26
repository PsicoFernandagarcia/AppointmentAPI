using Appointment.Application.AvailabilityUseCases.CreateAvailability;
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
    public class CreateAvailabilityHandlerShould
    {
        private readonly Mock<IUserRepository> _userRepository = new();
        private readonly Mock<IAvailabilityRepository> _availabilityRepository = new();
        private readonly CreateAvailabilityHandler _createAvailabilityHandler;
        private readonly Mock<IOutputCacheStore> _cacheStore = new();


        public CreateAvailabilityHandlerShould()
        {
            _createAvailabilityHandler = new(_userRepository.Object, _availabilityRepository.Object, _cacheStore.Object);
        }

        private List<Availability> GetAvailabilities()
            => new()
        {
            Availability.Create(1,1,new DateTime(2022,10,5,10,0,0),60,true).Value,
            Availability.Create(2,1,new DateTime(2022,10,5,11,0,0),60,true).Value,
            Availability.Create(3,1,new DateTime(2022,10,5,13,0,0),60,true).Value,
            Availability.Create(4,1,new DateTime(2022,10,6,13,0,0),60,true).Value,
            Availability.Create(5,2,new DateTime(2022,10,5,10,0,0),60,true).Value,
            Availability.Create(6,2,new DateTime(2022,10,5,9,0,0),60,true).Value
        };

        [InlineData(1, 10, "2022/10/5 9:50")]
        [InlineData(1, 60, "2022/10/5 9:00")]
        [InlineData(1, 60, "2022/10/5 8:00")]
        [InlineData(1, 60, "2022/10/5 12:00")]
        [Theory]
        public async Task Create_Availability_Because_Has_Valid_Properties_And_NO_Overlap(int hostId, int amountOfTime, DateTime dateFrom)
        {
            var request = new CreateAvailabilityCommand(hostId, amountOfTime, dateFrom.ToLocalTime());
            _userRepository.Setup(u => u.GetUserById(It.IsAny<int>()))
                .ReturnsAsync(User.Create(1, "UserName", "email", null, null, null, true, "name", "lastName", 0).Value);

            _availabilityRepository.Setup(ar => ar.Insert(It.IsAny<Availability>())).ReturnsAsync(() => GetAvailabilities().First());
            _availabilityRepository.Setup(ar => ar.GetOverlapped(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(() =>
                {
                    var to = request.DateOfAvailability.AddMinutes(request.AmountOfTime);
                    return GetAvailabilities().Where(a =>
                         a.HostId == request.HostId
                         && (a.DateOfAvailability >= request.DateOfAvailability
                             || a.DateOfAvailability.AddMinutes(a.AmountOfTime - 1) >= request.DateOfAvailability)
                         && (a.DateOfAvailability < to
                            || a.DateOfAvailability.AddMinutes(a.AmountOfTime - 1) < to)
                    ).ToList();
                });
            var result = await _createAvailabilityHandler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType<Availability>();
            _availabilityRepository.Verify(x => x.Insert(It.IsAny<Availability>()), Times.Once);
            _cacheStore.Verify(cs => cs.EvictByTagAsync(CacheKeys.Availabilities, It.IsAny<CancellationToken>()), Times.Once);

        }

        [InlineData(1, 60, "2022/10/5 11:00")]
        [InlineData(1, 60, "2022/10/5 9:30")]
        [InlineData(1, 11, "2022/10/5 9:50")]
        [InlineData(1, 120, "2022/10/5 12:00")]
        [Theory]
        public async Task Return_Failure_Due_To_Overlap(int hostId, int amountOfTime, DateTime dateFrom)
        {
            var request = new CreateAvailabilityCommand(hostId, amountOfTime, dateFrom.ToLocalTime());
            _userRepository.Setup(u => u.GetUserById(It.IsAny<int>()))
                .ReturnsAsync(User.Create(1, "UserName", "email", null, null, null, true, "name", "lastName", 0).Value);

            _availabilityRepository.Setup(ar => ar.Insert(It.IsAny<Availability>())).ReturnsAsync(() => GetAvailabilities().First());
            _availabilityRepository.Setup(ar => ar.GetOverlapped(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(() =>
                {
                    var to = request.DateOfAvailability.AddMinutes(request.AmountOfTime);
                    return GetAvailabilities().Where(a =>
                         a.HostId == request.HostId
                         && (a.DateOfAvailability >= request.DateOfAvailability
                             || a.DateOfAvailability.AddMinutes(a.AmountOfTime - 1) >= request.DateOfAvailability)
                         && (a.DateOfAvailability < to
                            || a.DateOfAvailability.AddMinutes(a.AmountOfTime - 1) < to)
                    ).ToList();
                });
            var result = await _createAvailabilityHandler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().BeOfType<CreationError>();
            result.Error.Message.Should().Contain("there is already an appointment at this time");
            _availabilityRepository.Verify(x => x.Insert(It.IsAny<Availability>()), Times.Never);
            _cacheStore.Verify(cs => cs.EvictByTagAsync(CacheKeys.Availabilities, It.IsAny<CancellationToken>()), Times.Never);

        }

        [Fact]
        public async Task Return_Failure_Due_To_Invalid_Host()
        {
            var request = new CreateAvailabilityCommand(1, 60, DateTime.Now.ToLocalTime());
            _userRepository.Setup(u => u.GetUserById(It.IsAny<int>()))
                .ReturnsAsync(null as User);

            _availabilityRepository.Setup(ar => ar.Insert(It.IsAny<Availability>())).ReturnsAsync(() => GetAvailabilities().First());
            var result = await _createAvailabilityHandler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().BeOfType<ResultError>();
            result.Error.Message.Should().Contain("host does not exists");
            _availabilityRepository.Verify(x => x.Insert(It.IsAny<Availability>()), Times.Never);
            _cacheStore.Verify(cs => cs.EvictByTagAsync(CacheKeys.Availabilities, It.IsAny<CancellationToken>()), Times.Never);

        }

        [Fact]
        public async Task Create_Availabilities_Because_Valid_Properties_And_No_Overlap()
        {
            var request = new CreateAvailabilitiesCommand
            {
                HostId = 1,
                Availabilities = GetAvailabilities()
                                    .Where(a => a.HostId == 1)
                                    .Select(
                                        a => new CreateAvailabilityCommand(a.HostId, a.AmountOfTime, a.DateOfAvailability)
                                    ),
                AvailabilitiesToRemove = new int[1] { 10 }
            };

            _userRepository.Setup(u => u.GetUserById(It.IsAny<int>()))
               .ReturnsAsync(User.Create(1, "UserName", "email", null, null, null, true, "name", "lastName", 0).Value);

            _availabilityRepository.Setup(ar => ar.Insert(It.IsAny<IEnumerable<Availability>>())).ReturnsAsync(() => GetAvailabilities());
            _availabilityRepository.Setup(ar => ar.GetOverlapped(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<Availability>());

            var result = await _createAvailabilityHandler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(GetAvailabilities().Count);
            _availabilityRepository.Verify(x => x.Insert(It.IsAny<IEnumerable<Availability>>()), Times.Once);
            _availabilityRepository.Verify(x => x.Delete(It.IsAny<IEnumerable<int>>()), Times.Once);
            _cacheStore.Verify(cs => cs.EvictByTagAsync(CacheKeys.Availabilities, It.IsAny<CancellationToken>()), Times.Once);

        }

        [Fact]
        public async Task Not_Create_Availabilities_Because_Overlap()
        {
            var request = new CreateAvailabilitiesCommand
            {
                HostId = 1,
                Availabilities = GetAvailabilities()
                                    .Where(a => a.HostId == 1)
                                    .Select(
                                        a => new CreateAvailabilityCommand(a.HostId, a.AmountOfTime, a.DateOfAvailability)
                                    ),
                AvailabilitiesToRemove = new int[1] { 10 }
            };

            _userRepository.Setup(u => u.GetUserById(It.IsAny<int>()))
               .ReturnsAsync(User.Create(1, "UserName", "email", null, null, null, true, "name", "lastName", 0).Value);

            _availabilityRepository.Setup(ar => ar.Insert(It.IsAny<IEnumerable<Availability>>())).ReturnsAsync(() => GetAvailabilities());
            _availabilityRepository.Setup(ar => ar.GetOverlapped(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(GetAvailabilities());

            var result = await _createAvailabilityHandler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().BeOfType<CreationError>();
            result.Error.Message.Should().Contain("there is already an appointment at this time");
            _availabilityRepository.Verify(x => x.Insert(It.IsAny<Availability>()), Times.Never);
            _availabilityRepository.Verify(x => x.Delete(It.IsAny<IEnumerable<int>>()), Times.Never);
            _cacheStore.Verify(cs => cs.EvictByTagAsync(CacheKeys.Availabilities, It.IsAny<CancellationToken>()), Times.Never);

        }

        [Fact]
        public async Task Not_Create_Availabilities_Because_Invalid_Host()
        {
            var request = new CreateAvailabilitiesCommand
            {
                HostId = 1,
                Availabilities = GetAvailabilities()
                                    .Where(a => a.HostId == 1)
                                    .Select(
                                        a => new CreateAvailabilityCommand(a.HostId, a.AmountOfTime, a.DateOfAvailability)
                                    ),
                AvailabilitiesToRemove = new int[1] { 10 }
            };

            _userRepository.Setup(u => u.GetUserById(It.IsAny<int>()))
               .ReturnsAsync(null as User);

            _availabilityRepository.Setup(ar => ar.Insert(It.IsAny<IEnumerable<Availability>>())).ReturnsAsync(() => GetAvailabilities());
            _availabilityRepository.Setup(ar => ar.GetOverlapped(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<Availability>());

            var result = await _createAvailabilityHandler.Handle(request, CancellationToken.None);
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().BeOfType<ResultError>();
            result.Error.Message.Should().Contain("host does not exists");
            _availabilityRepository.Verify(x => x.Insert(It.IsAny<Availability>()), Times.Never);
            _availabilityRepository.Verify(x => x.Delete(It.IsAny<IEnumerable<int>>()), Times.Never);
            _cacheStore.Verify(cs => cs.EvictByTagAsync(CacheKeys.Availabilities, It.IsAny<CancellationToken>()), Times.Never);

        }
    }
}
