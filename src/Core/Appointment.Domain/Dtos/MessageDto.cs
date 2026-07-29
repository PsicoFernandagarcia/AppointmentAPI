using Appointment.Domain.Entities;
using System;

namespace Appointment.Domain.Dtos
{
    public class MessageDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public bool Active { get; set; }

        public static MessageDto FromEntity(Message entity)
        {
            if (entity is null) return null;
            return new MessageDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Content = entity.Content,
                DateFrom = entity.DateFrom,
                DateTo = entity.DateTo,
                Active = entity.Active
            };
        }
    }
}
