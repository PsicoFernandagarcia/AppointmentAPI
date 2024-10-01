using Application.Integration.Test.Abstractions;
using Appointment.Application.BlogPostUseCases.CreateBlogPost;
using Appointment.Domain.Dtos;
using Appointment.Infrastructure.Configuration;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;


namespace Appointment.Integration.Test.BlogPostCase
{
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
