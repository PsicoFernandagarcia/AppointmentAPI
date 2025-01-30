using Appointment.Domain;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using Appointment.Infrastructure.Security;
using CSharpFunctionalExtensions;
using MediatR;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.AuthUseCases.ResetPassword
{
    public class ResetPasswordCommand: IRequest<Result<bool, ResultError>>
    {
        public string Email { get; set; }
        public int Code { get; set; }
        public string NewPassword { get; set; }
    }
    public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, Result<bool, ResultError>>
    {
        public readonly IUserRepository _userRepository;
        private readonly ICrypt _crypt;

        public ResetPasswordHandler(IUserRepository userRepository, ICrypt crypt)
        {
            _userRepository = userRepository;
            _crypt = crypt;
        }

        public async Task<Result<bool, ResultError>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var code = await _userRepository.GetValidCode(request.Email, request.Code);
            if (code is null)
                return Result.Failure<bool, ResultError>(new BadInputError("El código ingresado no es válido para ese email, intente generar uno nuevo"));
            
            var user = await _userRepository.GetUserByEmail(request.Email);
            var password = _crypt.DecryptStringFromBytes_Aes(request.NewPassword);
            var (passwordHash, passwordSalt) = PassUtilities.CreatePasswordHash(password);
            user.UpdatePassword(passwordHash, passwordSalt);
            await _userRepository.UpdateUser(user);
            return true;
        }

       
    }
}
