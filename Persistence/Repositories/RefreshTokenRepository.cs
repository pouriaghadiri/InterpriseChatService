using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _context;

        public RefreshTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task<RefreshToken?> GetByIdAsync(Guid id)
        {
            return await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Id == id);
        }

        public async Task<List<RefreshToken>> GetByUserIdAsync(Guid userId)
        {
            return await _context.RefreshTokens
                .Where(rt => rt.UserId == userId)
                .OrderByDescending(rt => rt.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId)
        {
            var now = DateTime.Now;
            return await _context.RefreshTokens
                .Where(rt => rt.UserId == userId 
                    && rt.RevokedAt == null 
                    && rt.ExpiresAt > now)
                .OrderByDescending(rt => rt.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
        }

        public async Task UpdateAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);
            await Task.CompletedTask;
        }

        public async Task RevokeTokenAsync(string token, string? replacedByToken = null)
        {
            var refreshToken = await GetByTokenAsync(token);
            if (refreshToken != null && !refreshToken.IsRevoked)
            {
                refreshToken.Revoke(replacedByToken);
                await UpdateAsync(refreshToken);
            }
        }

        public async Task RevokeAllUserTokensAsync(Guid userId, string? reason = null)
        {
            var tokens = await GetActiveTokensByUserIdAsync(userId);
            foreach (var token in tokens)
            {
                if (!token.IsRevoked)
                {
                    token.Revoke();
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task RemoveExpiredTokensAsync()
        {
            var expiredTokens = await _context.RefreshTokens
                .Where(rt => rt.ExpiresAt < DateTime.Now && rt.RevokedAt != null)
                .ToListAsync();

            _context.RefreshTokens.RemoveRange(expiredTokens);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string token)
        {
            return await _context.RefreshTokens
                .AnyAsync(rt => rt.Token == token);
        }
    }
}

