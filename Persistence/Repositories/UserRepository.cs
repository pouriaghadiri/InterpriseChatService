using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task<User?> GetbyEmailAsync(Email email)
        {
            return await _context.Users.Include(u => u.UserRoles).FirstOrDefaultAsync(x => x.Email.Value == email.Value);
        }

        public async Task<User?> GetbyIdAsync(Guid id)
        {
            return await _context.Users.Include(u => u.UserRoles).FirstOrDefaultAsync(x => x.Id == id);
        } 
    }
}
