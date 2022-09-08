using Appointment.Application.AvailabilityUseCases.GetAvailability;
using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using FluentAssertions;
using Moq;
using Xunit;

namespace Appointment.Test.Application.Availabilities
{
    public class GetAvailabilityHandlerShould
    {
        private readonly Mock<IUserRepository> _userRepository = new();
        private readonly Mock<IAvailabilityRepository> _availabilityRepository = new();
        private readonly GetAvailabilityHandler _getAvailabilityHandler;

        public GetAvailabilityHandlerShould()
        {
            _getAvailabilityHandler = new(_userRepository.Object, _availabilityRepository.Object);
        }

        private List<AvailabilityDto> GetAvailabilities()
        => new()
        {
            new AvailabilityDto(1, 1, new DateTime(2022,10,5,10,0,0), 10, true),
            new AvailabilityDto(2, 1, new DateTime(2022,10,5,11,0,0), 10, false),
            new AvailabilityDto(3, 1, new DateTime(2022,11,1,12,0,0), 10, true),
            new AvailabilityDto(4, 1, new DateTime(2022,11,5,10,0,0), 10, true),
            new AvailabilityDto(5, 2, new DateTime(2022,10,5,10,0,0), 10, true),
            new AvailabilityDto(6, 2, new DateTime(2022,11,5,10,0,0), 10, true),
            new AvailabilityDto(7, 2, new DateTime(2022,12,5,10,0,0), 10, true)
        };

        [InlineData(1, "2022/10/01 10:00", "2022/12/01 10:00", false, 4)]
        [InlineData(1, "2022/10/05 10:00", "2022/11/05 10:00", false, 4)]
        [InlineData(1, "2022/10/05 10:00", "2022/11/05 10:00", true, 3)]
        [InlineData(1, "2022/10/05 10:00", "2022/10/30 10:00", false, 2)]
        [InlineData(2, "2022/10/05 10:00", "2022/10/30 10:00", false, 1)]
        [InlineData(1, "2022/10/05 10:30", "2022/10/6 9:00", true, 0)]
        [InlineData(3, "2022/09/05 10:30", "2023/10/6 9:00", false, 0)]
        [InlineData(3, "2023/09/05 10:30", "2023/10/6 9:00", false, 0)]
        [Theory]
        public async Task Return_Availabilities_With_Filter_Applied(int hostId, DateTime dateFrom, DateTime dateTo, bool showOnlyEmpty, int totalRowsToShow)
        {
            _userRepository.Setup(u => u.GetUserById(It.IsAny<int>()))
                .ReturnsAsync(User.Create(1, "UserName", "email", null, null, null, true, "name", "lastName", 0).Value);
            _availabilityRepository.Setup(ar => ar.GetByFilter(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<bool>()))
                .ReturnsAsync(() =>
                {
                    return GetAvailabilities().Where(
                             a => a.HostId == hostId
                                          && a.DateOfAvailability >= dateFrom
                                          && a.DateOfAvailability <= dateTo
                                          && a.IsEmpty == (showOnlyEmpty ? showOnlyEmpty : a.IsEmpty)
                             );
                });
            var totalAvailabilities = await _getAvailabilityHandler.Handle(new GetAvailabilityQuery(hostId, dateFrom, dateTo, showOnlyEmpty), CancellationToken.None);
            totalAvailabilities.IsSuccess.Should().BeTrue();
            totalAvailabilities.Value.Should().HaveCount(totalRowsToShow);
        }

        [Fact]
        public async Task Return_Error_Because_Host_Does_Not_Exists()
        {
            _userRepository.Setup(u => u.GetUserById(It.IsAny<int>()))
                .ReturnsAsync(null as User);
            _availabilityRepository.Setup(ar => ar.GetByFilter(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<bool>()))
                .ReturnsAsync(() =>
                {
                    return GetAvailabilities();
                });
            var totalAvailabilities = await _getAvailabilityHandler.Handle(new GetAvailabilityQuery(10, DateTime.Now, DateTime.Now, false), CancellationToken.None);
            totalAvailabilities.IsSuccess.Should().BeFalse();
            totalAvailabilities.Error.Should().BeOfType<ResultError>();
            totalAvailabilities.Error.Message.Should().Contain("host does not exists");

        }



    }
}
