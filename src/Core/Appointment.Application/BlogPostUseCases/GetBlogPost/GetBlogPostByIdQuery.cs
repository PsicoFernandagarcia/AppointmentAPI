using Appointment.Domain.Dtos;
using Appointment.Domain.Interfaces;
using Appointment.Domain.ResultMessages;
using CSharpFunctionalExtensions;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Appointment.Application.BlogPostUseCases.GetBlogPost
{
    public class GetBlogPostByIdQuery : IRequest<Result<BlogPostDto, ResultError>>
    {
        public int Id { get; set; }
    }

    public class GetBlogPostByIdHandler : IRequestHandler<GetBlogPostByIdQuery, Result<BlogPostDto, ResultError>>
    {
        private readonly IBlogRepository _repo;
        public GetBlogPostByIdHandler(IBlogRepository repo)
        {
            _repo = repo;
        }
        public async Task<Result<BlogPostDto, ResultError>> Handle(GetBlogPostByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repo.Get(request.Id);
            if(entity is null) return Result.Failure<BlogPostDto, ResultError>("Blog does not exists");
            return BlogPostDto.FromEntity(entity); 
        }
    }
}
