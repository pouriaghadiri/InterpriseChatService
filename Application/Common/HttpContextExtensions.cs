using Microsoft.AspNetCore.Http;
using System;
using System.Text;

namespace Application.Common
{
    /// <summary>
    /// Extension methods for HttpContext to extract client information
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Gets the client IP address from HttpContext, handling proxy headers
        /// </summary>
        public static string? GetClientIpAddress(this HttpContext? context)
        {
            if (context == null)
                return null;

            // Check for forwarded IP (when behind proxy/load balancer)
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].ToString();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                // X-Forwarded-For can contain multiple IPs, take the first one
                var ips = forwardedFor.Split(',');
                if (ips.Length > 0)
                {
                    return ips[0].Trim();
                }
            }

            // Check for real IP header
            var realIp = context.Request.Headers["X-Real-IP"].ToString();
            if (!string.IsNullOrEmpty(realIp))
            {
                return realIp.Trim();
            }

            // Fallback to connection remote IP
            return context.Connection.RemoteIpAddress?.ToString();
        }

        /// <summary>
        /// Gets device information from User-Agent header
        /// </summary>
        public static string? GetDeviceInfo(this HttpContext? context)
        {
            if (context == null)
                return null;

            var userAgent = context.Request.Headers["User-Agent"].ToString();
            if (string.IsNullOrEmpty(userAgent))
                return null;

            // Extract basic device info from User-Agent
            var deviceInfo = new StringBuilder();

            // Try to extract browser
            if (userAgent.Contains("Chrome", StringComparison.OrdinalIgnoreCase))
                deviceInfo.Append("Chrome");
            else if (userAgent.Contains("Firefox", StringComparison.OrdinalIgnoreCase))
                deviceInfo.Append("Firefox");
            else if (userAgent.Contains("Safari", StringComparison.OrdinalIgnoreCase))
                deviceInfo.Append("Safari");
            else if (userAgent.Contains("Edge", StringComparison.OrdinalIgnoreCase))
                deviceInfo.Append("Edge");
            else if (userAgent.Contains("Opera", StringComparison.OrdinalIgnoreCase))
                deviceInfo.Append("Opera");

            // Try to extract OS
            if (userAgent.Contains("Windows", StringComparison.OrdinalIgnoreCase))
                deviceInfo.Append(" / Windows");
            else if (userAgent.Contains("Mac", StringComparison.OrdinalIgnoreCase))
                deviceInfo.Append(" / macOS");
            else if (userAgent.Contains("Linux", StringComparison.OrdinalIgnoreCase))
                deviceInfo.Append(" / Linux");
            else if (userAgent.Contains("Android", StringComparison.OrdinalIgnoreCase))
                deviceInfo.Append(" / Android");
            else if (userAgent.Contains("iOS", StringComparison.OrdinalIgnoreCase) || userAgent.Contains("iPhone", StringComparison.OrdinalIgnoreCase))
                deviceInfo.Append(" / iOS");

            // If we couldn't extract anything, return a truncated User-Agent
            if (deviceInfo.Length == 0)
            {
                return userAgent.Length > 100 ? userAgent.Substring(0, 100) : userAgent;
            }

            return deviceInfo.ToString();
        }
    }
}

