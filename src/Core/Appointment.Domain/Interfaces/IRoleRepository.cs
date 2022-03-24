using System.Collections.Generic;
using System.Threading.Tasks;
using Appointment.Domain.Entities;

namespace Appointment.Domain.Interfaces
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetRoles();
    }
}