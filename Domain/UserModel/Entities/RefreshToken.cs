using Domain.Base;
using System;

namespace Domain.Entities
{
    public class RefreshToken : Entity
    {
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; private set; }
        public string? RevokedBy { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string? ReplacedByToken { get; set; }
        public string? DeviceInfo { get; set; }
        public string? IpAddress { get; set; }
        
        public bool IsExpired => DateTime.Now >= ExpiresAt;
        public bool IsRevoked => RevokedAt != null;
        public bool IsActive => !IsRevoked && !IsExpired;

        public virtual User User { get; set; }

        private RefreshToken() { }

        public static RefreshToken Create(Guid userId, string token, DateTime expiresAt, string? deviceInfo = null, string? ipAddress = null)
        {
            return new RefreshToken
            {
                UserId = userId,
                Token = token,
                ExpiresAt = expiresAt,
                CreatedAt = DateTime.Now,
                DeviceInfo = deviceInfo,
                IpAddress = ipAddress
            };
        }

        public void Revoke(string? replacedByToken = null)
        {
            RevokedAt = DateTime.Now;
            ReplacedByToken = replacedByToken;
        }
    }
}

