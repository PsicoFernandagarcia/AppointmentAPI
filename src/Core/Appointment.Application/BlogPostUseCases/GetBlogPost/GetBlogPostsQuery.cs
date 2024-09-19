using Appointment.Domain.Dtos;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.BlogPostUseCases.GetBlogPost
{
    public class GetBlogPostsQuery : IRequest<Result<IEnumerable<BlogPostDto>, ResultError>>
    {
        public bool ShowAll { get; set; }
    }

    public class GetBlogPostHandler : IRequestHandler<GetBlogPostsQuery, Result<IEnumerable<BlogPostDto>, ResultError>>
    {
        private readonly IBlogRepository _repo;
        public GetBlogPostHandler(IBlogRepository repo)
        {
            _repo = repo;
        }
        public async Task<Result<IEnumerable<BlogPostDto>, ResultError>> Handle(GetBlogPostsQuery request, CancellationToken cancellationToken)
        {
            var entities = await _repo.Get(request.ShowAll);
            if(entities is null || !entities.Any()) return Result.Success<IEnumerable<BlogPostDto>,ResultError>([]);
            var listResult = new List<BlogPostDto>();
            foreach (var entity in entities)
            {
                listResult.Add(BlogPostDto.FromEntity(entity));
            }
            return listResult; 
        }
    }
}
