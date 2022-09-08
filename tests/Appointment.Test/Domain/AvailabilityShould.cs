using Appointment.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Appointment.Test.Domain
{
    public class AvailabilityShould
    {

        [InlineData(1, 1, "3022/10/25", 60, true)]
        [InlineData(0, 1, "3021/10/25", 10, false)]
        [Theory]
        public void Be_Created_Because_Of_Valid_Properties(int id, int hostId, DateTime dateOfAvailability, int amountOfTime, bool isEmpty)
        {
            var availabilityResult = Availability.Create(id, hostId, dateOfAvailability, amountOfTime, isEmpty);
            availabilityResult.IsSuccess.Should().BeTrue();
            availabilityResult.Value.Should().BeOfType<Availability>();
        }

        [InlineData(-1, 1, "3022/10/25", 60, true)]
        [InlineData(1, 1, "3022/01/25", 0, true)]
        [InlineData(1, 1, "3022/01/25", -1, true)]
        [InlineData(1, -1, "3022/01/25", 1, true)]
        [InlineData(1, 0, "3022/01/25", 1, true)]

        [Theory]
        public void Not_Be_Created_Because_Of_Inalid_Properties(int id, int hostId, DateTime dateOfAvailability, int amountOfTime, bool isEmpty)
        {
            var availabilityResult = Availability.Create(id, hostId, dateOfAvailability, amountOfTime, isEmpty);
            availabilityResult.IsSuccess.Should().BeFalse();
            availabilityResult.Error.Should().NotBeNullOrWhiteSpace();
        }

    }
}
