using Appointment.Domain.Dtos;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.MessageUseCases.GetMessage
{
    public class GetMessagesQuery : IRequest<Result<IEnumerable<MessageDto>, ResultError>>
    {
        public bool ShowAll { get; set; }
    }

    public class GetMessagesHandler : IRequestHandler<GetMessagesQuery, Result<IEnumerable<MessageDto>, ResultError>>
    {
        private readonly IMessageRepository _messageRepository;

        public GetMessagesHandler(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<Result<IEnumerable<MessageDto>, ResultError>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
        {
            var entities = await _messageRepository.Get(request.ShowAll);
            if (entities is null || !entities.Any())
            {
                return Result.Success<IEnumerable<MessageDto>, ResultError>([]);
            }

            var list = entities.Select(MessageDto.FromEntity).ToList();
            return Result.Success<IEnumerable<MessageDto>, ResultError>(list);
        }
    }
}
