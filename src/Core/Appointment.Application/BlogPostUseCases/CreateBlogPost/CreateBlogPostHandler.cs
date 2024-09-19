using Appointment.Domain;
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

namespace Appointment.Application.BlogPostUseCases.CreateBlogPost
{
    public class CreateBlogPostCommand : IRequest<Result<BlogPost, ResultError>>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string SocialMediaTitle { get; set; }
        public string SocialMediaDescription { get; set; }
        public string Content { get; set; }
        public bool Visible { get; set; }
        public DateTime? ShouldBePublicAfter { get; set; }

        public CreateBlogPostCommand(string title,
                                 string description,
                                 string socialMediaTitle,
                                 string socialMediaDescription,
                                 string content,
                                 bool visible,
                                 DateTime? shouldBePublicAfter)
        {
            Title = title;
            Description = description;
            SocialMediaTitle = socialMediaTitle;
            SocialMediaDescription = socialMediaDescription;
            Content = content;
            Visible = visible;
            ShouldBePublicAfter = shouldBePublicAfter;
        }
    }
    public class CreateBlogPostHandler : IRequestHandler<CreateBlogPostCommand, Result<BlogPost, ResultError>>
    {

        private readonly IBlogRepository _blogRepository;
        private readonly IOutputCacheStore _cachingStore;

        public CreateBlogPostHandler(IBlogRepository blogRepository, IOutputCacheStore cachingStore)
        {
            _blogRepository = blogRepository;
            _cachingStore = cachingStore;
        }

        public async Task<Result<BlogPost, ResultError>> Handle(CreateBlogPostCommand request, CancellationToken cancellationToken)
        {
            await _cachingStore.EvictByTagAsync(CacheKeys.BlogPost, cancellationToken);
            return await _blogRepository.Insert(BlogPost.Create(
                                                                request.Title,
                                                                request.Description,
                                                                request.Content,
                                                                request.ShouldBePublicAfter,
                                                                request.Visible,
                                                                request.SocialMediaDescription,
                                                                request.SocialMediaTitle,
                                                                0,
                                                                0).Value);
        }
    }


    public class CreateBlogPostValidator : AbstractValidator<CreateBlogPostCommand>
    {
        public CreateBlogPostValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Title cannot be empty");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description cannot be empty");
            RuleFor(x => x.Content).NotEmpty().WithMessage("Content cannot be empty");
        }
    }
}
