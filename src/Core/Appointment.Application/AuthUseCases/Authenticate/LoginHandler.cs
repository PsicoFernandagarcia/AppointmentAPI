using Appointment.Domain.Entities;
using Appointment.Domain.Infrastructure;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using Appointment.Infrastructure.Security;
using CSharpFunctionalExtensions;
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

namespace Appointment.Application.AuthUseCases.Authenticate
{
    public class LoginHandler : IRequestHandler<LoginCommand, Result<LoginResult, UnauthorizedError>>
    {
        private readonly AuthOptions _authConfig;
        private readonly IUserRepository _userRepository;
        private readonly ICrypt _crypt;

        public LoginHandler(IOptions<AuthOptions> authOptions,
                            IUserRepository userRepository, ICrypt crypt)
            => (_authConfig, _userRepository, _crypt)
                = (authOptions.Value, userRepository, crypt);

        public async Task<Result<LoginResult, UnauthorizedError>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByName(request.UserName.Trim());
            var password = _crypt.DecryptStringFromBytes_Aes(request.Password);
            if (user is null || !VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return Result.Failure<LoginResult, UnauthorizedError>(new UnauthorizedError("Usuario o contraseña no válido"));

            user.TimezoneOffset = request.TimezoneOffset;
            await _userRepository.UpdateUser(user);
            return Result.Success<LoginResult, UnauthorizedError>(new LoginResult
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
                Expires = DateTime.UtcNow.AddHours(5),
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
