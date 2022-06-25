using Appointment.Application.AuthUseCases.CreateUser;
using Appointment.Domain.Entities;
using Appointment.Domain.Infrastructure;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using Google.Apis.Auth;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.AuthUseCases.AuthenticateExternal
{
    public class LoginExternalHandler : IRequestHandler<LoginExternalCommand, Result<LoginExternalResult, UnauthorizedError>>
    {
        private readonly AuthOptions _authConfig;
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;

        public LoginExternalHandler(IOptions<AuthOptions> authOptions, IUserRepository userRepository, IMediator mediator)
            => (_authConfig, _userRepository, _mediator)
                = (authOptions.Value, userRepository, mediator);

        public async Task<Result<LoginExternalResult, UnauthorizedError>> Handle(LoginExternalCommand request, CancellationToken cancellationToken)
        {
            var validationSettings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new string[] { "278365989343-jml9q2b7b146ubc5f71r2f1rrfib1p5e.apps.googleusercontent.com" }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, validationSettings);
            var user = await _userRepository.GetUserByName(request.Email);
            if (user is null)
            {
                var command = new CreateUserCommand()
                {
                    Email = request.Email,
                    Name = request.FirstName,
                    IsExternal = true,
                    LastName = request.LastName,
                    Password = "_",
                    UserName = request.Email,
                    TimezoneOffset = request.TimezoneOffset
                };
                var resultCreateUser = await _mediator.Send(command);
                if (resultCreateUser.IsFailure) return Result.Failure<LoginExternalResult, UnauthorizedError>(new UnauthorizedError("cannot authenticate user"));
                user = resultCreateUser.Value;
            }
            else
            {
                user.TimezoneOffset = request.TimezoneOffset;
                await _userRepository.UpdateUser(user);
            }

            return Result.Success<LoginExternalResult, UnauthorizedError>(new LoginExternalResult
            {
                Token = GenerateJwtToken(user),
                UserName = user.UserName,
                Email = user.Email,
                Name = user.Name,
                LastName = user.LastName,
                Id = user.Id
            });
        }

        private string GenerateJwtToken(User user)
        {
            var key = Encoding.ASCII.GetBytes(_authConfig.Secret);
            var tokenHandler = new JwtSecurityTokenHandler();
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role,string.Join(',',user.Roles.Select(x=>x.Name))),
                }),
                Expires = DateTime.UtcNow.AddDays(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(descriptor);
            return tokenHandler.WriteToken(token);
        }

        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value annot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }

    }
}
