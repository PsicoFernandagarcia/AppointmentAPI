using Appointment.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Appointment.Domain.Interfaces
{
    public interface IMessageRepository
    {
        Task<Message> Insert(Message message);
        Task<Message> Update(Message message);
        Task Delete(Message message);
        Task<Message> Get(int id);
        Task<IEnumerable<Message>> Get(bool showAll);
    }
}
