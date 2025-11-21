using System;

namespace Application.Common.CacheModels
{
    public class PasswordResetTokenCacheModel
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}

