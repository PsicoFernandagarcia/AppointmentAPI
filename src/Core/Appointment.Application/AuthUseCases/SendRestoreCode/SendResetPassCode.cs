using Appointment.Application.SendEmailUseCase.ResetPassword;
using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.AuthUseCases.SendResetPassCode
{
    public class SendResetPassCodeCommand : IRequest<Result<bool, ResultError>>
    {
        public string Email { get; set; }

    }

    public class SendResetPassCodeHandler : IRequestHandler<SendResetPassCodeCommand, Result<bool, ResultError>>
    {
        private readonly IMediator _mediator;
        private readonly IUserRepository _userRepository;
        public SendResetPassCodeHandler(IMediator mediator, IUserRepository userRepository)
        {
            _mediator = mediator;
            _userRepository = userRepository;
        }
        public async Task<Result<bool, ResultError>> Handle(SendResetPassCodeCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByEmail(request.Email);
            var resetPassCodeResult = ResetPasswordCode.Create(0, request.Email, GenerateCode());
            if (resetPassCodeResult.IsFailure)
                return Result.Failure<bool, ResultError>(new BadInputError(resetPassCodeResult.Error));
            
            var resetPassCode = await _userRepository.AddCode(resetPassCodeResult.Value);
            var sendEmailResult = await _mediator.Send(new SendResetPasswordCodeCommand { Code = resetPassCode, UserName = user.FullName });
            if(sendEmailResult.IsFailure)
                return Result.Failure<bool, ResultError>(new BadInputError(sendEmailResult.Error.Message));

            return true;
        }

        private static int GenerateCode()
        {
            var random = new Random();
            return random.Next(1000, 9999);
        }
    }
}
