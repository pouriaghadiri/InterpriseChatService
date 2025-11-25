using System.Security.Claims;

namespace Application.Common
{
    /// <summary>
    /// Extension methods for ClaimsPrincipal to extract user information from JWT tokens
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Gets the current user ID from JWT token claims
        /// </summary>
        /// <param name="user">The ClaimsPrincipal</param>
        /// <returns>User ID as Guid, or null if not found or invalid</returns>
        public static Guid? GetUserId(this ClaimsPrincipal? user)
        {
            if (user == null)
                return null;

            var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                return null;
            }
            return userId;
        }

        /// <summary>
        /// Gets the current user ID from JWT token claims, throws exception if not found
        /// </summary>
        /// <param name="user">The ClaimsPrincipal</param>
        /// <returns>User ID as Guid</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when user ID is not found or invalid</exception>
        public static Guid GetUserIdOrThrow(this ClaimsPrincipal? user)
        {
            var userId = user.GetUserId();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("Invalid user id in token.");
            }
            return userId.Value;
        }
    }
}

