using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appointment.Domain.Entities;

namespace Appointment.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByName(string userName);
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserById(int id);
        Task<User> CreateUser(User u);
        Task<User> UpdateUser(User u);
        Task<IList<User>> GetUserByRole(RolesEnum role);
    }
}
