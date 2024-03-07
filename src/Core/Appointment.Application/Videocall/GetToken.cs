using Appointment.Application.UsersUseCase.GetUserByRole;
using Appointment.Domain;
using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Appointment.Infrastructure.ExternalServices;
using Appointment.Infrastructure.ExternalServices.Dtos;
using Appointment.Domain.Infrastructure;
using Microsoft.Extensions.Options;

namespace Appointment.Application.Videocall
{
    public class GetTokenQuery : IRequest<Result<GetTokenQueryResponse, ResultError>>
    {
        public string TokenType { get; set; } = "rtc";
        public string Role { get; set; } = "subscriber";
        public string Channel { get; set; }
        public string Uid { get; set; }
        public RolesEnum UserRole { get; set; }
    }
    public class GetTokenQueryResponse
    {
        public string Token { get; set; }
        public string AppId { get; set; }
    }

    public class GetTokenHandler : IRequestHandler<GetTokenQuery, Result<GetTokenQueryResponse, ResultError>>
    {
        private readonly IAgoraClient _agoraClient;
        private readonly AgoraOptions _agoraOptions;

        public GetTokenHandler(IAgoraClient agoraClient, IOptions<AgoraOptions> agoraOptions)
        {
            _agoraClient = agoraClient;
            _agoraOptions = agoraOptions.Value;
        }

        public async Task<Result<GetTokenQueryResponse, ResultError>> Handle(GetTokenQuery request, CancellationToken cancellationToken)
        {
            var getTokenModel = new GetTokenModel
            {
                Channel = request.Channel,
                TokenType = request.TokenType,
                Role = request.Role,
                Uid = request.Uid,
            };
            var getTokenResponse = await _agoraClient.GetToken(getTokenModel);
            if (getTokenResponse.IsSuccessStatusCode)
                return Result.Success<GetTokenQueryResponse, ResultError>(new GetTokenQueryResponse
                {
                    AppId = _agoraOptions.AppId,
                    Token = getTokenResponse.Content.Token
                }) ;
            return Result.Failure<GetTokenQueryResponse, ResultError>(new ResultError(getTokenResponse.Error.Message));
        }
    }
}
