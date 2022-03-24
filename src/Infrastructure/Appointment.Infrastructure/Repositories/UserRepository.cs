using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appointment.Domain;
using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Appointment.Infrastructure.Repositories
{
    public class UserRepository:IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByName(string userName)
            => _context.Users
                .Include(r => r.Roles)
                .FirstOrDefault(x => x.UserName == userName );
        public async Task<User> GetUserByEmail(string email)
           => _context.Users
               .FirstOrDefault(x => x.Email == email);

        public async Task<User> GetUserById(int id)
            => _context.Users
                .Include(r => r.Roles)
                .FirstOrDefault(x => x.Id == id);

        public async Task<IList<User>> GetUserByRole(RolesEnum role)
            => await _context.Users
                .Where(u=> u.Roles.Any(r => r.Id == ((int)role)))
                .ToListAsync();

        public async Task<User> CreateUser(User u)
        {
            await _context.Users.AddAsync(u);
            await _context.SaveChangesAsync();
            return u;
        } 
    }
}
