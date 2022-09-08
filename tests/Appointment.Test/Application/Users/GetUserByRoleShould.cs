using Appointment.Application.UsersUseCase.GetUserByRole;
using Appointment.Domain;
using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Appointment.Test.Application.Users
{
    public class GetUserByRoleShould
    {
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly GetUserByRoleHandler _handler;
        public GetUserByRoleShould()
        {
            _handler = new GetUserByRoleHandler(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task Return_Users_From_Specific_Role()
        {
            _userRepositoryMock.Setup(ur => ur.GetUserByRole(It.IsAny<RolesEnum>()))
                .ReturnsAsync(new List<User> {
                    User.Create(1,"test","email",null,null,null,true,"test","test",10).Value
                });

            var getByRoleResult = await _handler.Handle(new GetUserByRoleQuery(RolesEnum.COMMON), CancellationToken.None);
            getByRoleResult.IsSuccess.Should().BeTrue();
            getByRoleResult.Value.Should().HaveCount(1);
        }

        [Fact]
        public async Task Return_Empty_List_If_There_Is_No_Users()
        {
            _userRepositoryMock.Setup(ur => ur.GetUserByRole(It.IsAny<RolesEnum>()))
                .ReturnsAsync(new List<User> { });

            var getByRoleResult = await _handler.Handle(new GetUserByRoleQuery(RolesEnum.COMMON), CancellationToken.None);
            getByRoleResult.IsSuccess.Should().BeTrue();
            getByRoleResult.Value.Should().HaveCount(0);
        }
    }
}
