using Appointment.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        Task<int> Delete(int id);
        Task<ResetPasswordCode> AddCode(ResetPasswordCode code);
        Task<ResetPasswordCode> GetValidCode(string email, int code);
    }
}
