using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<RefreshToken?> GetByIdAsync(Guid id);
        Task<List<RefreshToken>> GetByUserIdAsync(Guid userId);
        Task<List<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId);
        Task AddAsync(RefreshToken refreshToken);
        Task UpdateAsync(RefreshToken refreshToken);
        Task RevokeTokenAsync(string token, string? replacedByToken = null);
        Task RevokeAllUserTokensAsync(Guid userId, string? reason = null);
        Task RemoveExpiredTokensAsync();
        Task<bool> ExistsAsync(string token);
    }
}

