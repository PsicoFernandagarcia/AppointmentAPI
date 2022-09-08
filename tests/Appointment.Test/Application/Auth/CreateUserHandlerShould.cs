using Appointment.Application.AuthUseCases.CreateUser;
using Appointment.Domain.Entities;
using Appointment.Domain.Infrastructure;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using Appointment.Infrastructure.Repositories;
using Appointment.Infrastructure.Security;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Appointment.Test.Application.Auth
{
    public class CreateUserHandlerShould
    {
        private readonly Mock<IUserRepository> _userRepository = new();
        private readonly Mock<IRoleRepository> _roleRepository = new();
        private readonly CreateUserHandler _createUserHandler;

        public CreateUserHandlerShould()
        {
            var authOptions = Options.Create(new AuthOptions()
            {
                HashValue = "psicoe84ad660c4721ae0e84ad660c4721ae0fer",
                Secret = "AADDFIKCJMLDOCJK"
            });
            Crypt _crypt = new(authOptions);
            _createUserHandler = new(_userRepository.Object, _roleRepository.Object, _crypt);
        }

        [Fact]
        public async Task Create_User_Due_To_Valid_User_Information()
        {
            var pass = "ynrgYZKi+RLVhwXwrY2yGUSEpuSVRuVlK64Jz4qESC1x9u+8CuI8lMtA6Aj9BQim";
            _userRepository.Setup(ur => ur.GetUserByName(It.IsAny<string>())).ReturnsAsync(null as User);
            var newUserResult = await _createUserHandler.Handle(new CreateUserCommand("UserName", "Name", "LastName", "email", pass, true, 0), CancellationToken.None);

            newUserResult.IsSuccess.Should().BeTrue();
            newUserResult.Value.Should().BeOfType<User>();
            _userRepository.Verify(x => x.CreateUser(It.IsAny<User>()), Times.Once);

        }


        [InlineData("", "Name", "LastName", "email", "ynrgYZKi+RLVhwXwrY2yGUSEpuSVRuVlK64Jz4qESC1x9u+8CuI8lMtA6Aj9BQim", true)]
        [InlineData("fsdfsda", "", "LastName", "email", "ynrgYZKi+RLVhwXwrY2yGUSEpuSVRuVlK64Jz4qESC1x9u+8CuI8lMtA6Aj9BQim", true)]
        [InlineData("fsdfsda", "ffsdsafdsa", "", "email", "ynrgYZKi+RLVhwXwrY2yGUSEpuSVRuVlK64Jz4qESC1x9u+8CuI8lMtA6Aj9BQim", true)]
        [InlineData("fsdfsda", "fdsafdsfdsa", "LastName", "", "ynrgYZKi+RLVhwXwrY2yGUSEpuSVRuVlK64Jz4qESC1x9u+8CuI8lMtA6Aj9BQim", true)]
        [Theory]
        public async Task Not_Create_User_Due_To_Invalid_User_Information(string userName, string name, string lastName, string email, string pass, bool isExternal)
        {
            _userRepository.Setup(ur => ur.GetUserByName(It.IsAny<string>())).ReturnsAsync(null as User);
            var newUserResult = await _createUserHandler.Handle(new CreateUserCommand(userName, name, lastName, email, pass, isExternal, 0), CancellationToken.None);

            newUserResult.IsSuccess.Should().BeFalse();
            newUserResult.Error.Should().BeOfType<ResultError>();
            _userRepository.Verify(x => x.CreateUser(It.IsAny<User>()), Times.Never);

        }

        [Fact]
        public async Task Not_Create_User_Because_User_Already_Exists()
        {
            var pass = "ynrgYZKi+RLVhwXwrY2yGUSEpuSVRuVlK64Jz4qESC1x9u+8CuI8lMtA6Aj9BQim";
            _userRepository.Setup(ur => ur.GetUserByName(It.IsAny<string>())).ReturnsAsync(
                 User.Create(1, "UserName", "email", null, null, null, true, "name", "lastName", 0).Value);
            var newUserResult = await _createUserHandler.Handle(new CreateUserCommand("UserName", "Name", "LastName", "email", pass, true, 0), CancellationToken.None);
            newUserResult.IsSuccess.Should().BeFalse();
            newUserResult.Error.Should().BeOfType<CreationError>();
            newUserResult.Error.Message.Should().Contain("User already exists");
            _userRepository.Verify(x => x.CreateUser(It.IsAny<User>()), Times.Never);

        }

        [Fact]
        public async Task Not_Create_User_Because_Of_Missing_Password()
        {
            var pass = "";
            _userRepository.Setup(ur => ur.GetUserByName(It.IsAny<string>())).ReturnsAsync(null as User);
            var newUserResult = await _createUserHandler.Handle(new CreateUserCommand("UserName", "Name", "LastName", "email", pass, true, 0), CancellationToken.None);
            newUserResult.IsSuccess.Should().BeFalse();
            newUserResult.Error.Should().BeOfType<CreationError>();
            newUserResult.Error.Message.Should().Contain("Password empty");
            _userRepository.Verify(x => x.CreateUser(It.IsAny<User>()), Times.Never);

        }


    }
}
