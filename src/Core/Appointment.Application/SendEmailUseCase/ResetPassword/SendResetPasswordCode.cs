using Appointment.Domain.Entities;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Appointment.Application.SendEmailUseCase.ResetPassword
{
    public class SendResetPasswordCodeCommand : IRequest<Result<bool, ResultError>>
    {
        public ResetPasswordCode Code { get; set; }
        public string UserName { get; set; }

    }
    public class SendResetPasswordCodeHandler : IRequestHandler<SendResetPasswordCodeCommand, Result<bool, ResultError>>
    {

        private readonly IEmailSender _emailSender;
        public SendResetPasswordCodeHandler(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }
        public async Task<Result<bool, ResultError>> Handle(SendResetPasswordCodeCommand request, CancellationToken cancellationToken)
        {
            var reminderBody = await File.ReadAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(), "Content/password_reset_code.html"), cancellationToken);
            reminderBody = reminderBody.Replace("#_name_#", request.UserName)
                        .Replace("#_code_#", request.Code.Code.ToString());


            return _emailSender.Send(request.Code.UserEmail, $"Reestablecer contraseña", reminderBody, true, false);
        }
    }
}
