using System;

namespace Application.Common
{
    public static class CacheHelper
    {
        // User-related cache keys
        public static string UserKey(Guid userId) => $"user:{userId}";
        public static string UserByEmailKey(string email) => $"user:email:{email}";
        public static string UserPermissionsKey(Guid userId, Guid? departmentId = null) 
            => departmentId.HasValue ? $"user:{userId}:permissions:{departmentId}" : $"user:{userId}:permissions:global";
        
        // Role-related cache keys
        public static string RoleKey(Guid roleId) => $"role:{roleId}";
        public static string RolePermissionsKey(Guid roleId, Guid departmentId) => $"role:{roleId}:permissions:{departmentId}";
        
        // Department-related cache keys
        public static string DepartmentKey(Guid departmentId) => $"department:{departmentId}";
        public static string DepartmentUsersKey(Guid departmentId) => $"department:{departmentId}:users";
        
        // Permission-related cache keys
        public static string PermissionKey(Guid permissionId) => $"permission:{permissionId}";
        public static string AllPermissionsKey() => "permissions:all";
        
        // Session-related cache keys
        public static string UserSessionKey(Guid userId) => $"session:{userId}";
        public static string TokenBlacklistKey(string token) => $"blacklist:{token}";
        
        // Cache expiration times
        public static class Expiration
        {
            public static readonly TimeSpan User = TimeSpan.FromMinutes(30);
            public static readonly TimeSpan Role = TimeSpan.FromHours(1);
            public static readonly TimeSpan Department = TimeSpan.FromHours(2);
            public static readonly TimeSpan Permission = TimeSpan.FromHours(4);
            public static readonly TimeSpan Session = TimeSpan.FromDays(1);
            public static readonly TimeSpan TokenBlacklist = TimeSpan.FromDays(7);
        }
    }
}
