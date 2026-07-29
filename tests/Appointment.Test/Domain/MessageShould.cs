using Appointment.Domain.Entities;
using FluentAssertions;
using System;
using Xunit;

namespace Appointment.Test.Domain
{
    public class MessageShould
    {
        [Fact]
        public void Be_Created_With_Valid_Properties()
        {
            var title = "Holiday Notice";
            var content = "Office will be closed on Monday.";
            var dateFrom = DateTime.UtcNow.Date;
            var dateTo = dateFrom.AddDays(5);

            var result = Message.Create(title, content, dateFrom, dateTo, null);

            result.IsSuccess.Should().BeTrue();
            result.Value.Title.Should().Be(title);
            result.Value.Content.Should().Be(content);
            result.Value.DateFrom.Should().Be(dateFrom);
            result.Value.DateTo.Should().Be(dateTo);
            result.Value.Active.Should().BeTrue();
        }

        [Theory]
        [InlineData("", "Content", 0, "Title cannot be empty.")]
        [InlineData("Title", "", 0, "Content cannot be empty.")]
        [InlineData("Title", "Content", 5, "DateFrom cannot be later than DateTo.")]
        public void Fail_To_Create_With_Invalid_Properties(string title, string content, int dateFromOffsetDays, string expectedErrorSubstring)
        {
            var dateFrom = DateTime.UtcNow.Date.AddDays(dateFromOffsetDays);
            var dateTo = DateTime.UtcNow.Date;

            var result = Message.Create(title, content, dateFrom, dateTo);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Contain(expectedErrorSubstring);
        }

        [Fact]
        public void Update_Properties_Successfully()
        {
            var message = Message.Create("Initial Title", "Initial Content", DateTime.UtcNow.Date, DateTime.UtcNow.Date.AddDays(1)).Value;

            var newTitle = "Updated Title";
            var newContent = "Updated Content";
            var newDateFrom = DateTime.UtcNow.Date.AddDays(1);
            var newDateTo = DateTime.UtcNow.Date.AddDays(3);

            var updateResult = message.Update(newTitle, newContent, newDateFrom, newDateTo, false);

            updateResult.IsSuccess.Should().BeTrue();
            message.Title.Should().Be(newTitle);
            message.Content.Should().Be(newContent);
            message.DateFrom.Should().Be(newDateFrom);
            message.DateTo.Should().Be(newDateTo);
            message.Active.Should().BeFalse();
        }
    }
}
