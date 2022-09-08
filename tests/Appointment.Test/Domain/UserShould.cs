using Appointment.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Appointment.Test.Domain
{
    public class UserShould
    {

        [InlineData(1, "UserName", "Email", true, "lastname", "name", 10)]
        [InlineData(1, "UserName", "Email", false, "lastname", "name", -10)]
        [InlineData(0, "UserName", "Email", false, "lastname", "name", -10)]
        [Theory]
        public void Be_Created_Because_Of_Valid_Properties(int id, string userName, string email, bool isExternal, string lastName, string name, int timezoneOffset)
        {
            var userResult = User.Create(id, userName, email, null, null, null, isExternal, lastName, name, timezoneOffset);
            userResult.IsSuccess.Should().BeTrue();
            userResult.Value.Should().BeOfType<User>();
        }

        [InlineData(-1, "UserName", "Email", true, "lastname", "name", 10)]
        [InlineData(1, "", "Email", false, "lastname", "name", -10)]
        [InlineData(1, null, "Email", false, "lastname", "name", -10)]
        [InlineData(1, "UserName", "", false, "lastname", "name", -10)]
        [InlineData(1, "UserName", null, false, "lastname", "name", -10)]
        [InlineData(1, "test", "Email", false, "", "name", -10)]
        [InlineData(1, "test", "Email", false, null, "name", -10)]
        [InlineData(1, "test", "Email", false, "lastname", "", -10)]
        [InlineData(1, "test", "Email", false, "lastname", null, -10)]
        [Theory]
        public void Not_Be_Created_Because_Of_Invalid_Properties(int id, string userName, string email, bool isExternal, string lastName, string name, int timezoneOffset)
        {
            var userResult = User.Create(id, userName, email, null, null, null, isExternal, lastName, name, timezoneOffset);
            userResult.IsSuccess.Should().BeFalse();
            userResult.Error.Should().NotBeNullOrWhiteSpace();
        }

    }
}
