using Appointment.Domain.Dtos;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.MessageUseCases.GetMessage
{
    public class GetMessageByIdQuery : IRequest<Result<MessageDto, ResultError>>
    {
        public int Id { get; set; }

        public GetMessageByIdQuery(int id)
        {
            Id = id;
        }
    }

    public class GetMessageByIdHandler : IRequestHandler<GetMessageByIdQuery, Result<MessageDto, ResultError>>
    {
        private readonly IMessageRepository _messageRepository;

        public GetMessageByIdHandler(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<Result<MessageDto, ResultError>> Handle(GetMessageByIdQuery request, CancellationToken cancellationToken)
        {
            var message = await _messageRepository.Get(request.Id);
            if (message is null)
            {
                return Result.Failure<MessageDto, ResultError>(new DoesNotExistError($"Message with id {request.Id} does not exist"));
            }

            return Result.Success<MessageDto, ResultError>(MessageDto.FromEntity(message));
        }
    }
}
