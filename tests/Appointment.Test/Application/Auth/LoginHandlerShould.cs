using Appointment.Application.AuthUseCases.Authenticate;
using Appointment.Application.AuthUseCases.CreateUser;
using Appointment.Domain.Entities;
using Appointment.Domain.Infrastructure;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using Appointment.Infrastructure.Security;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Appointment.Test.Application.Auth
{
    public class LoginHandlerShould
    {
        private readonly Mock<IUserRepository> _userRepository = new();
        private readonly Mock<IRoleRepository> _roleRepository = new();
        private readonly CreateUserHandler _createUserHandler;

        private readonly LoginHandler _loginHandler;
        public LoginHandlerShould()
        {
            var authOptions = Options.Create(new AuthOptions()
            {
                HashValue = "psicoe84ad660c4721ae0e84ad660c4721ae0fer",
                Secret = "AADDFIKCJMLDOCJK"
            });
            Crypt _crypt = new(authOptions);
            _loginHandler = new(authOptions, _userRepository.Object, _crypt);
            _createUserHandler = new(_userRepository.Object, _roleRepository.Object, _crypt);
        }

        [Fact]
        public async Task Allow_User_To_Login_Due_To_Valid_Credentials()
        {
            var pass = "ynrgYZKi+RLVhwXwrY2yGUSEpuSVRuVlK64Jz4qESC1x9u+8CuI8lMtA6Aj9BQim";
            _userRepository.Setup(ur => ur.GetUserByName(It.IsAny<string>())).ReturnsAsync(null as User);
            var newUserResult = await _createUserHandler.Handle(new CreateUserCommand("UserName", "Name", "LastName", "email", pass, true, 0), CancellationToken.None);
            _userRepository.Setup(ur => ur.GetUserByName(It.IsAny<string>())).ReturnsAsync(newUserResult.Value);

            var loginUserResult = await _loginHandler.Handle(new LoginCommand("UserName", pass, 10), CancellationToken.None);
            loginUserResult.IsSuccess.Should().BeTrue();
            loginUserResult.Value.Should().BeOfType<LoginResult>();
            loginUserResult.Value.Token.Should().NotBeNullOrWhiteSpace();
            _userRepository.Verify(x => x.UpdateUser(It.IsAny<User>()), Times.Once);

        }

        [InlineData("TestName", "ynrgYZKi+RLVhwXwrY2yGUSEpuSVRuVlK64Jz4qESC1x9u+8CuI8lMtA6Aj9BQim", "TestName", "L0IwV+ZsOqRUQfLI7scAlCfYwWtgfj8YYa74gg96MGhAqVJM/uTYLPtSU7c8lSzY")]
        [InlineData("TestName", "ynrgYZKi+RLVhwXwrY2yGUSEpuSVRuVlK64Jz4qESC1x9u+8CuI8lMtA6Aj9BQim", "TestName2", "ynrgYZKi+RLVhwXwrY2yGUSEpuSVRuVlK64Jz4qESC1x9u+8CuI8lMtA6Aj9BQim")]
        [Theory]
        public async Task Dont_Allow_User_To_Login_Due_To_Invalid_Credentials(string userNameToCreate, string passToCreate, string userNameToLogin, string passToLogin)
        {
            _userRepository.Setup(ur => ur.GetUserByName(It.IsAny<string>())).ReturnsAsync(null as User);
            var newUserResult = await _createUserHandler.Handle(new CreateUserCommand(userNameToCreate, "Name", "LastName", "email", passToCreate, true, 0), CancellationToken.None);

            _userRepository.Setup(ur => ur.GetUserByName(userNameToLogin))
                .ReturnsAsync(() =>
                    {
                        if (newUserResult.Value.UserName.Equals(userNameToLogin))
                            return newUserResult.Value;
                        return null;
                    }
                );

            var loginUserResult = await _loginHandler.Handle(new LoginCommand(userNameToLogin, passToLogin, 10), CancellationToken.None);
            loginUserResult.IsSuccess.Should().BeFalse();
            loginUserResult.Error.Should().BeOfType<UnauthorizedError>();
            _userRepository.Verify(x => x.UpdateUser(It.IsAny<User>()), Times.Never);

        }


    }
}
