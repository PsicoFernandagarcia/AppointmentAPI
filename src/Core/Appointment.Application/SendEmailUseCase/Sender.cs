using Appointment.Domain.Infrastructure;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Mail;

namespace Appointment.Application.SendEmailUseCase
{
    public interface IEmailSender
    {
        Result<bool, ResultError> Send(string ToEmail, string Subject, string Body, bool IsHtml);
    }
    public class Sender : IEmailSender
    {
        public EmailOptions _emailOptions;

        public Sender(IOptions<EmailOptions> emailOptions)
        {
            _emailOptions = emailOptions.Value;
        }

        public Result<bool, ResultError> Send(string ToEmail, string Subject, string Body, bool IsHtml)
        {
            try
            {
                if (_emailOptions.SendEmail)
                {
                    var smtp = new SmtpClient();
                    smtp.Host = _emailOptions.GmailHost;
                    smtp.Port = _emailOptions.GmailPort;
                    smtp.EnableSsl = _emailOptions.GmailSSL;
                    smtp.Credentials = new NetworkCredential(_emailOptions.GmailUsername, _emailOptions.GmailPassword);

                    using (var message = new MailMessage(_emailOptions.GmailUsername, ToEmail))
                    {
                        message.Subject = Subject;
                        message.Body = Body;
                        message.IsBodyHtml = IsHtml;
                        smtp.Send(message);
                    }

                }

                return Result.Success<bool, ResultError>(true);
            }
            catch (Exception ex)
            {
                return Result.Failure<bool, ResultError>(ex.Message);
            }
        }
    }
}
