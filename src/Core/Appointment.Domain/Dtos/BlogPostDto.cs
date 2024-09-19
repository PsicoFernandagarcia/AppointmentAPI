using Appointment.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Dtos
{
    public class BlogPostDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string SocialMediaTitle { get; set; }
        public string SocialMediaDescription { get; set; }
        public bool Visible { get; set; }
        public DateTime ShouldBePublicAfter { get; set; }
        public string Content { get; set; }
        public int TotalViews { get; set; }
        public int TotalShared { get; set; }

        public static BlogPostDto FromEntity(BlogPost entity)
        {
            return new BlogPostDto
            {
                Content = entity.Content,
                Description = entity.Description,
                Id = entity.Id,
                Title = entity.Title,
                ShouldBePublicAfter = entity.ShouldBePublicAfter,
                SocialMediaDescription = entity.SocialMediaDescription,
                Visible = entity.Visible,
                TotalViews = entity.TotalViews,
                TotalShared = entity.TotalShared,
                SocialMediaTitle = entity.SocialMediaTitle,
            };
        }
    }
}
