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
    public class BlogRepository : IBlogRepository
    {
        private readonly AppDbContext _context;

        public BlogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<BlogPost> Get(int id)
        {
            return await _context.BlogPost.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<IEnumerable<BlogPost>> Get(bool showAll)
        {
            return await _context.BlogPost
                .Where(x => showAll || (x.Visible && x.ShouldBePublicAfter <= DateTime.UtcNow))
                .ToListAsync();
        }

        public async Task<BlogPost> Insert(BlogPost blogPost)
        {
            await _context.AddAsync(blogPost);
            await _context.SaveChangesAsync();
            return blogPost;
        }
    }
}