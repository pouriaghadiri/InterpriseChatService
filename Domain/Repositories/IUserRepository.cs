using Domain.Common.ValueObjects;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface IUserRepository
    {
        Task AddAsync(User user);
        Task<User?> GetbyEmailAsync(Email email);
        Task<User?> GetbyIdAsync(Guid id);
    }
}
