using Application.Integration.Test.Abstractions;
using Appointment.Application.BlogPostUseCases.CreateBlogPost;
using Appointment.Application.BlogPostUseCases.GetBlogPost;
using Appointment.Domain.Dtos;
using Appointment.Domain.Entities;
using Appointment.Infrastructure.Configuration;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;


namespace Application.Integration.Test.BlogPostCase
{
    public class GetBlogPostsTest(TestWebApplicationFactory factory) : BaseIntegrationTest(factory)
    {
        private async Task InsertBlogPost(BlogPost post)
        {
            using var scope = factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await context.AddAsync(post);
            await context.SaveChangesAsync();
        }


        private async Task DeleteAllModels(List<int> ids)
        {
            using var scope = factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await context.BlogPost.Where(entity => ids.Contains(entity.Id)).ExecuteDeleteAsync();
        }
        [Fact]
        public async Task Should_ReturnAllPosts()
        {
            var post = BlogPost.Create("title", "desc", "content", DateTime.UtcNow.AddDays(-1), true, "", "", 0, 0);
            var post2 = BlogPost.Create("title2", "desc", "content", DateTime.UtcNow.AddDays(-1), true, "", "", 0, 0);
            var post3 = BlogPost.Create("title3", "desc", "content", DateTime.UtcNow.AddDays(-1), true, "", "", 0, 0);
            await InsertBlogPost(post.Value);
            await InsertBlogPost(post2.Value);
            await InsertBlogPost(post3.Value);

            var res = await HttpClient.GetAsync("api/blog");
            await DeleteAllModels([post.Value.Id, post2.Value.Id, post3.Value.Id]);

            res.StatusCode.Should().Be(HttpStatusCode.OK);
            var list = await  res.ToObject<List<BlogPost>>();
            list.Should().NotBeNull();
            list.Should().HaveCount(3);
        }

        [Fact]
        public async Task Should_ReturnAllPosts_ForCommonUsers()
        {
            var post = BlogPost.Create("title", "desc", "content", DateTime.UtcNow.AddDays(-1), true, "", "", 0, 0);
            var post2 = BlogPost.Create("title2", "desc", "content", DateTime.UtcNow.AddDays(-1), true, "", "", 0, 0);
            var post3 = BlogPost.Create("title3", "desc", "content", DateTime.UtcNow.AddDays(-1), false, "", "", 0, 0);
            await InsertBlogPost(post.Value);
            await InsertBlogPost(post2.Value);
            await InsertBlogPost(post3.Value);

            SetAuthForCommonUser();
            var res = await HttpClient.GetAsync($"api/blog?showAll={true}");

            await DeleteAllModels([post.Value.Id, post2.Value.Id, post3.Value.Id]);

            res.StatusCode.Should().Be(HttpStatusCode.OK);
            var list = await res.ToObject<List<BlogPost>>();
            list.Should().NotBeNull();
            list.Should().HaveCount(2);
        }

        [Fact]
        public async Task Should_ReturnAllPosts_ForHost()
        {
            var post = BlogPost.Create("title", "desc", "content", null, true, "", "", 0, 0);
            var post2 = BlogPost.Create("title2", "desc", "content", null, true, "", "", 0, 0);
            var post3 = BlogPost.Create("title3", "desc", "content", null, false, "", "", 0, 0);
            await InsertBlogPost(post.Value);
            await InsertBlogPost(post2.Value);
            await InsertBlogPost(post3.Value);

            var res = await HttpClient.GetAsync($"api/blog?showAll={true}");
            await DeleteAllModels([post.Value.Id, post2.Value.Id, post3.Value.Id]);

            res.StatusCode.Should().Be(HttpStatusCode.OK);
            var list = await res.ToObject<List<BlogPost>>();
            list.Should().NotBeNull();
            list.Should().HaveCount(3);

        }
    }
    public class CreateBlogPostTest(TestWebApplicationFactory factory) : BaseIntegrationTest(factory)
    {
        private async Task DeleteAllModels(List<int> ids)
        {
            using var scope = factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await context.BlogPost.Where(entity => ids.Contains(entity.Id)).ExecuteDeleteAsync();
        }
        [Fact]
        public async Task Should_ReturnBadRequest_WhenEmptyFields()
        {
            var request = new CreateBlogPostCommand("","","","","",false,null);
            var res = await HttpClient.PostAsJsonAsync("api/blog",request);
            res.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var badRequestObject = await res.ToBadRequestObject();
            badRequestObject.Message.Should().Contain("Title");
            badRequestObject.Message.Should().Contain("Description");
            badRequestObject.Message.Should().Contain("Content");

        }

        [Fact]
        public async Task Should_ReturnForbidden_WhenCommonUser()
        {
            
            var request = new CreateBlogPostCommand("", "", "", "", "", false, null);
            SetAuthForCommonUser();
            var res = await HttpClient.PostAsJsonAsync("api/blog", request);
            res.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        }

        [Fact]
        public async Task Should_CreateBlogPost_WhenCorrectInformation()
        {
            var request = new CreateBlogPostCommand("Title", "Desc", "Socl", "SMD", "content", true, DateTime.Now.AddDays(-1));
            var res = await HttpClient.PostAsJsonAsync("api/blog", request);
            res.StatusCode.Should().Be(HttpStatusCode.OK);
            var resultObject = await res.ToObject<BlogPostDto>();
            resultObject.Content.Should().Be(request.Content);
            using var scope = factory.Services.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<AppDbContext>();
            var dbBlog = await db.BlogPost.FirstOrDefaultAsync(x => x.Id == resultObject.Id);

            await DeleteAllModels([resultObject.Id]);
            dbBlog.Should().BeEquivalentTo(resultObject, options => options
                .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1)))
                .WhenTypeIs<DateTime>());
        }
    }
}
