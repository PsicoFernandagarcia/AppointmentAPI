using Appointment.Domain;
using Appointment.Domain.Dtos;
using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.MessageUseCases.CreateMessage
{
    public class CreateMessageCommand : IRequest<Result<MessageDto, ResultError>>
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public bool? Active { get; set; }

        public CreateMessageCommand(string title, string content, DateTime dateFrom, DateTime dateTo, bool? active = true)
        {
            Title = title;
            Content = content;
            DateFrom = dateFrom;
            DateTo = dateTo;
            Active = active;
        }
    }

    public class CreateMessageHandler : IRequestHandler<CreateMessageCommand, Result<MessageDto, ResultError>>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IOutputCacheStore _cachingStore;

        public CreateMessageHandler(IMessageRepository messageRepository, IOutputCacheStore cachingStore)
        {
            _messageRepository = messageRepository;
            _cachingStore = cachingStore;
        }

        public async Task<Result<MessageDto, ResultError>> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
        {
            var createResult = Message.Create(request.Title, request.Content, request.DateFrom, request.DateTo, request.Active);
            if (createResult.IsFailure)
            {
                return Result.Failure<MessageDto, ResultError>(new BadInputError(createResult.Error));
            }

            var inserted = await _messageRepository.Insert(createResult.Value);
            await _cachingStore.EvictByTagAsync(CacheKeys.Messages, cancellationToken);
            return Result.Success<MessageDto, ResultError>(MessageDto.FromEntity(inserted));
        }
    }

    public class CreateMessageValidator : AbstractValidator<CreateMessageCommand>
    {
        public CreateMessageValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Title cannot be empty");
            RuleFor(x => x.Content).NotEmpty().WithMessage("Content cannot be empty");
            RuleFor(x => x.DateFrom).NotEmpty().WithMessage("DateFrom cannot be empty");
            RuleFor(x => x.DateTo).NotEmpty().WithMessage("DateTo cannot be empty");
            RuleFor(x => x).Must(x => x.DateFrom.Date <= x.DateTo.Date).WithMessage("DateFrom cannot be later than DateTo");
        }
    }
}
