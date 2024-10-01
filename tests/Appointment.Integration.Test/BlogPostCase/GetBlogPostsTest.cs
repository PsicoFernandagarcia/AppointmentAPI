using Application.Integration.Test.Abstractions;
using Appointment.Domain.Entities;
using Appointment.Infrastructure.Configuration;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;


namespace Appointment.Integration.Test.BlogPostCase
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
}
