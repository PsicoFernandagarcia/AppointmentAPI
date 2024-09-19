using Appointment.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Appointment.Domain.Interfaces
{
    public interface IBlogRepository
    {
        Task<BlogPost> Insert(BlogPost blogPost);
        Task<BlogPost> Get(int id);
        Task<IEnumerable<BlogPost>> Get(bool showAll);
    }
}