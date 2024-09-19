using CSharpFunctionalExtensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace Appointment.Domain.Entities
{
    public class BlogPost
    {
        [Key]
        public int Id { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string SocialMediaTitle { get; private set; }
        public string SocialMediaDescription {  get; private set; }
        public bool Visible { get; private set; }
        public DateTime ShouldBePublicAfter { get; private set; }
        public string Content { get; private set; }
        public int TotalViews { get; private set; }
        public int TotalShared { get; private set; }
        public BlogPost()
        {

        }

        private BlogPost(string title,
                        string description,
                        string socialMediaTitle,
                        string socialMediaDescription,
                        bool visible,
                        DateTime shouldBePublicAfter,
                        string content,
                        int totalViews,
                        int totalShared)
        {
            Title = title;
            Description = description;
            SocialMediaTitle = socialMediaTitle;
            SocialMediaDescription = socialMediaDescription;
            Visible = visible;
            ShouldBePublicAfter = shouldBePublicAfter;
            Content = content;
            TotalViews = totalViews;
            TotalShared = totalShared;
        }

        public static Result<BlogPost> Create(string title,
                        string description,
                        string content,
                        DateTime? shouldBePublicAfter,
                        bool? visible,
                        string socialMediaDescription,
                        string socialMediaTitle,
                        int totalViews,
                        int totalShared)
        {
            var validation = Validate(title, description, content);
            if (validation.IsFailure) return Result.Failure<BlogPost>(validation.Error);
            return new BlogPost(title,
                        description,
                        string.IsNullOrWhiteSpace(socialMediaTitle) ? title : socialMediaTitle,
                        string.IsNullOrWhiteSpace(socialMediaDescription) ? title : socialMediaDescription,
                        visible is null || (bool)visible,
                        shouldBePublicAfter is null ? DateTime.UtcNow : shouldBePublicAfter.Value,
                        content,
                        totalViews,
                        totalShared);
        }

        private static Result<string> Validate(string title,
                        string description,
                        string content)
        {
            string errors = string.Empty;
            if (string.IsNullOrWhiteSpace(title)) errors += " title not valid ";
            if (string.IsNullOrWhiteSpace(description)) errors += " description not valid ";
            if (string.IsNullOrWhiteSpace(content)) errors += " content not valid ";
            if (!string.IsNullOrWhiteSpace(errors)) return Result.Failure<string>(errors);
            return Result.Success(string.Empty);
        }
    }
}