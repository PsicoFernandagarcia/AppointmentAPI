using Appointment.Domain;
using Appointment.Domain.Dtos;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.MessageUseCases.UpdateMessage
{
    public class UpdateMessageCommand : IRequest<Result<MessageDto, ResultError>>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public bool Active { get; set; }

        public UpdateMessageCommand(int id, string title, string content, DateTime dateFrom, DateTime dateTo, bool active)
        {
            Id = id;
            Title = title;
            Content = content;
            DateFrom = dateFrom;
            DateTo = dateTo;
            Active = active;
        }
    }

    public class UpdateMessageHandler : IRequestHandler<UpdateMessageCommand, Result<MessageDto, ResultError>>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IOutputCacheStore _cachingStore;

        public UpdateMessageHandler(IMessageRepository messageRepository, IOutputCacheStore cachingStore)
        {
            _messageRepository = messageRepository;
            _cachingStore = cachingStore;
        }

        public async Task<Result<MessageDto, ResultError>> Handle(UpdateMessageCommand request, CancellationToken cancellationToken)
        {
            var message = await _messageRepository.Get(request.Id);
            if (message is null)
            {
                return Result.Failure<MessageDto, ResultError>(new DoesNotExistError($"Message with id {request.Id} does not exist"));
            }

            var updateResult = message.Update(request.Title, request.Content, request.DateFrom, request.DateTo, request.Active);
            if (updateResult.IsFailure)
            {
                return Result.Failure<MessageDto, ResultError>(new BadInputError(updateResult.Error));
            }

            var updated = await _messageRepository.Update(message);
            await _cachingStore.EvictByTagAsync(CacheKeys.Messages, cancellationToken);
            return Result.Success<MessageDto, ResultError>(MessageDto.FromEntity(updated));
        }
    }

    public class UpdateMessageValidator : AbstractValidator<UpdateMessageCommand>
    {
        public UpdateMessageValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Id must be greater than 0");
            RuleFor(x => x.Title).NotEmpty().WithMessage("Title cannot be empty");
            RuleFor(x => x.Content).NotEmpty().WithMessage("Content cannot be empty");
            RuleFor(x => x.DateFrom).NotEmpty().WithMessage("DateFrom cannot be empty");
            RuleFor(x => x.DateTo).NotEmpty().WithMessage("DateTo cannot be empty");
            RuleFor(x => x).Must(x => x.DateFrom.Date <= x.DateTo.Date).WithMessage("DateFrom cannot be later than DateTo");
        }
    }
}
