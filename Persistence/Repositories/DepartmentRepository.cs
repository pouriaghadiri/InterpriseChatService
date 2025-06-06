using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly ApplicationDbContext _context;
        public DepartmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Department department)
        {
            await _context.Departments.AddAsync(department);
        }

        public async Task<bool> ExistsAsync(Expression<Func<Department, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _context.Departments.AnyAsync(predicate, cancellationToken);
        }

        public async Task<Department?> GetbyIdAsync(Guid id)
        {
            return await _context.Departments.FirstOrDefaultAsync(x => x.Id == id);
        } 

    }
}
