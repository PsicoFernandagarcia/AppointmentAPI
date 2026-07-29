using Appointment.Domain;
using Appointment.Domain.Dtos;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.MessageUseCases.DeleteMessage
{
    public class DeleteMessageCommand : IRequest<Result<MessageDto, ResultError>>
    {
        public int Id { get; set; }

        public DeleteMessageCommand(int id)
        {
            Id = id;
        }
    }

    public class DeleteMessageHandler : IRequestHandler<DeleteMessageCommand, Result<MessageDto, ResultError>>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IOutputCacheStore _cachingStore;

        public DeleteMessageHandler(IMessageRepository messageRepository, IOutputCacheStore cachingStore)
        {
            _messageRepository = messageRepository;
            _cachingStore = cachingStore;
        }

        public async Task<Result<MessageDto, ResultError>> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
        {
            var message = await _messageRepository.Get(request.Id);
            if (message is null)
            {
                return Result.Failure<MessageDto, ResultError>(new DoesNotExistError($"Message with id {request.Id} does not exist"));
            }

            await _messageRepository.Delete(message);
            await _cachingStore.EvictByTagAsync(CacheKeys.Messages, cancellationToken);
            return Result.Success<MessageDto, ResultError>(MessageDto.FromEntity(message));
        }
    }
}
