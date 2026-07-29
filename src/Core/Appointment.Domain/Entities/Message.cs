using CSharpFunctionalExtensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace Appointment.Domain.Entities
{
    public class Message
    {
        [Key]
        public int Id { get; private set; }
        public string Title { get; private set; }
        public string Content { get; private set; }
        public DateTime DateFrom { get; private set; }
        public DateTime DateTo { get; private set; }
        public bool Active { get; private set; }

        public Message()
        {
        }

        private Message(int id, string title, string content, DateTime dateFrom, DateTime dateTo, bool active)
        {
            Id = id;
            Title = title;
            Content = content;
            DateFrom = dateFrom;
            DateTo = dateTo;
            Active = active;
        }

        public static Result<Message> Create(string title, string content, DateTime dateFrom, DateTime dateTo, bool? active = true)
        {
            var validation = Validate(title, content, dateFrom, dateTo);
            if (validation.IsFailure) return Result.Failure<Message>(validation.Error);

            return new Message(0, title, content, dateFrom.Date, dateTo.Date, active ?? true);
        }

        public Result Update(string title, string content, DateTime dateFrom, DateTime dateTo, bool active)
        {
            var validation = Validate(title, content, dateFrom, dateTo);
            if (validation.IsFailure) return Result.Failure(validation.Error);

            Title = title;
            Content = content;
            DateFrom = dateFrom.Date;
            DateTo = dateTo.Date;
            Active = active;

            return Result.Success();
        }

        private static Result<string> Validate(string title, string content, DateTime dateFrom, DateTime dateTo)
        {
            string errors = string.Empty;
            if (string.IsNullOrWhiteSpace(title)) errors += "Title cannot be empty. ";
            if (string.IsNullOrWhiteSpace(content)) errors += "Content cannot be empty. ";
            if (dateFrom.Date > dateTo.Date) errors += "DateFrom cannot be later than DateTo. ";

            if (!string.IsNullOrWhiteSpace(errors)) return Result.Failure<string>(errors.Trim());
            return Result.Success(string.Empty);
        }
    }
}
