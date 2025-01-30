using Appointment.Domain;
using Appointment.Domain.Entities;
using Appointment.Domain.Interfaces;
using Appointment.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appointment.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByName(string userName)
            => await _context.Users
                .Include(r => r.Roles)
                .FirstOrDefaultAsync(x => x.UserName == userName);

        public async Task<User> GetUserByEmail(string email)
           => await _context.Users
               .FirstOrDefaultAsync(x => x.Email == email);

        public async Task<User> GetUserById(int id)
            => await _context.Users
                .Include(r => r.Roles)
                .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<IList<User>> GetUserByRole(RolesEnum role)
            => await _context.Users
                .Where(u => u.Roles.Any(r => r.Id == ((int)role)))
                .Select(x => User.Create(x.Id, x.UserName, x.Email, null, null, null, x.IsExternal, x.Name, x.LastName, x.TimezoneOffset).Value)
                .ToListAsync();

        public async Task<User> CreateUser(User u)
        {
            await _context.Users.AddAsync(u);
            await _context.SaveChangesAsync();
            return u;
        }

        public async Task<User> UpdateUser(User u)
        {
            _context.Users.Update(u);
            await _context.SaveChangesAsync();
            return u;
        }

        public async Task<int> Delete(int id)
        {
            var userToDelete = await _context.Users.FindAsync(id);
            if (userToDelete is not null)
            {
                _context.Users.Remove(userToDelete);
                return await _context.SaveChangesAsync();
            }
            return 0;
        }

        public async Task<ResetPasswordCode> AddCode(ResetPasswordCode code)
        {
            await _context.ResetPasswordCodes.AddAsync(code);
            await _context.SaveChangesAsync();
            return code;
        }
        public async Task<ResetPasswordCode> GetValidCode(string email, int code)
        {
            var codeEntity = await _context.ResetPasswordCodes.Where(c => 
                    c.UserEmail == email
                    && c.Code == code
                    && c.EndDate >= DateTime.Now)
                .FirstOrDefaultAsync();
            return codeEntity;
        }
    }
}
