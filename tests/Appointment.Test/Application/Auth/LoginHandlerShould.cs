using Appointment.Application.AuthUseCases.Authenticate;
using Appointment.Application.AuthUseCases.CreateUser;
using Appointment.Domain.Entities;
using Appointment.Domain.Infrastructure;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using Appointment.Infrastructure.Security;
using FluentAssertions;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Appointment.Test.Application.Auth
{
    public class LoginHandlerShould
    {
        private readonly Mock<IUserRepository> _userRepository = new();
        private readonly Mock<IRoleRepository> _roleRepository = new();
        private readonly Mock<IOutputCacheStore> _cacheStore = new();
        private readonly CreateUserHandler _createUserHandler;

        private readonly LoginHandler _loginHandler;
        public LoginHandlerShould()
        {
            var authOptions = Options.Create(new AuthOptions()
            {
                HashValue = "psicoe84ad660c4721ae0e84ad660c4721ae0fer",
                Secret = "AADDFIKCJMLDOCJKAADDFIKCJMLDOCJKAADDFIKCJMLDOCJKAADDFIKCJMLDOCJK"
            });
            Crypt _crypt = new(authOptions);
            _loginHandler = new(authOptions, _userRepository.Object, _crypt);
            _createUserHandler = new(_userRepository.Object, _roleRepository.Object, _crypt, _cacheStore.Object);
        }

        [Fact]
        public async Task Allow_User_To_Login_Due_To_Valid_Credentials()
        {
            var pass = "928YNOSAI7nGS/GpKAFFbOmvbtTK/unjj5pko2BEcw2sESReciWlKjT8a3hRy981";
            _userRepository.Setup(ur => ur.GetUserByName(It.IsAny<string>())).ReturnsAsync(null as User);
            var newUserResult = await _createUserHandler.Handle(new CreateUserCommand("UserName", "Name", "LastName", "email", pass, true, 0), CancellationToken.None);
            _userRepository.Setup(ur => ur.GetUserByName(It.IsAny<string>())).ReturnsAsync(newUserResult.Value);

            var loginUserResult = await _loginHandler.Handle(new LoginCommand("UserName", pass, 10), CancellationToken.None);
            loginUserResult.IsSuccess.Should().BeTrue();
            loginUserResult.Value.Should().BeOfType<LoginResult>();
            loginUserResult.Value.Token.Should().NotBeNullOrWhiteSpace();
            _userRepository.Verify(x => x.UpdateUser(It.IsAny<User>()), Times.Once);

        }

        [InlineData("TestName", "928YNOSAI7nGS/GpKAFFbOmvbtTK/unjj5pko2BEcw2sESReciWlKjT8a3hRy981", "TestName", "vsJOzyooWpfWsb1jio0oBVrKckitK + fvKfPTu4Jd68LumzSEI2AWasB9y3oxtFyw")]
        [InlineData("TestName", "928YNOSAI7nGS/GpKAFFbOmvbtTK/unjj5pko2BEcw2sESReciWlKjT8a3hRy981", "TestName2", "928YNOSAI7nGS/GpKAFFbOmvbtTK/unjj5pko2BEcw2sESReciWlKjT8a3hRy981")]
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
