using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using Appointment.Infrastructure.Security;
using CSharpFunctionalExtensions;
using MediatR;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.AuthUseCases.CreateUser
{
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, Result<User, ResultError>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ICrypt _crypt;

        public CreateUserHandler(IUserRepository userRepository, IRoleRepository roleRepository, ICrypt crypt)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _crypt = crypt;
        }

        public async Task<Result<User, ResultError>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetUserByName(request.UserName);
            if (existingUser != null) return Result.Failure<User, ResultError>(new CreationError("User already exists"));
            if (string.IsNullOrWhiteSpace(request.Password)) return Result.Failure<User, ResultError>(new CreationError("Password empty"));
            
            var password = _crypt.DecryptStringFromBytes_Aes(request.Password);
            var (passwordHash, passwordSalt) = CreatePasswordHash(password);
            var role = (await _roleRepository.GetRoles()).Where(x => x.Name == "COMMON");
            var userEntityResult = User.Create(0, request.UserName, request.Email, passwordHash, passwordSalt,
                role.ToList(), request.IsExternal, request.Name, request.LastName, request.TimezoneOffset
                );
            
            if (userEntityResult.IsFailure) return Result.Failure<User, ResultError>(userEntityResult.Error);
            var userEntity = userEntityResult.Value;
            await _userRepository.CreateUser(userEntity);
            return userEntity;
        }

        private (byte[] passwordHash, byte[] passwordSalt) CreatePasswordHash(string password)
        {
            byte[] passwordHash;
            byte[] passwordSalt;

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
            return (passwordHash, passwordSalt);
        }
    }
}