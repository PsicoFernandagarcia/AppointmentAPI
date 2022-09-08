using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Appointment.Domain.Entities
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        [JsonIgnore]
        public virtual ICollection<User> Users { get; private set; }
    }
}