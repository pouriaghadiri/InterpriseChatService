using Application.Common.DTOs;
using Domain.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Infrastructure.Services.Email
{
    public class SmtpEmailService : IEmailService
    {
        private readonly EmailSettingsDTO _emailSettings;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(IOptions<EmailSettingsDTO> emailSettings, ILogger<SmtpEmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            return await SendEmailAsync(to, subject, body, null, null, isHtml);
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body, List<string> cc, List<string> bcc, bool isHtml = true)
        {
            try
            {
                using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
                {
                    client.EnableSsl = _emailSettings.EnableSsl;
                    client.Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);

                    using (var message = new MailMessage())
                    {
                        message.From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName);
                        message.To.Add(to);
                        message.Subject = subject;
                        message.Body = body;
                        message.IsBodyHtml = isHtml;

                        if (cc != null && cc.Count > 0)
                        {
                            foreach (var ccEmail in cc)
                            {
                                message.CC.Add(ccEmail);
                            }
                        }

                        if (bcc != null && bcc.Count > 0)
                        {
                            foreach (var bccEmail in bcc)
                            {
                                message.Bcc.Add(bccEmail);
                            }
                        }

                        await client.SendMailAsync(message);
                        _logger.LogInformation("Email sent successfully to {To}", to);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}. Error: {ErrorMessage}", to, ex.Message);
                return false;
            }
        }

        public async Task<bool> SendPasswordResetEmailAsync(string to, string resetToken, string resetLink)
        {
            var subject = "Password Reset Request";
            var body = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .button {{ display: inline-block; padding: 12px 24px; background-color: #007bff; color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
                        .button:hover {{ background-color: #0056b3; }}
                        .footer {{ margin-top: 30px; font-size: 12px; color: #666; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <h2>Password Reset Request</h2>
                        <p>You have requested to reset your password. Click the button below to reset your password:</p>
                        <a href='{resetLink}' class='button'>Reset Password</a>
                        <p>Or copy and paste this link into your browser:</p>
                        <p>{resetLink}</p>
                        <p><strong>This link will expire in 1 hour.</strong></p>
                        <p>If you did not request a password reset, please ignore this email.</p>
                        <div class='footer'>
                            <p>This is an automated message, please do not reply to this email.</p>
                        </div>
                    </div>
                </body>
                </html>";

            return await SendEmailAsync(to, subject, body, true);
        }

        public async Task<bool> SendEmailVerificationEmailAsync(string to, string verificationToken, string verificationLink)
        {
            var subject = "Email Verification";
            var body = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .button {{ display: inline-block; padding: 12px 24px; background-color: #28a745; color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
                        .button:hover {{ background-color: #218838; }}
                        .footer {{ margin-top: 30px; font-size: 12px; color: #666; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <h2>Verify Your Email Address</h2>
                        <p>Thank you for registering! Please verify your email address by clicking the button below:</p>
                        <a href='{verificationLink}' class='button'>Verify Email</a>
                        <p>Or copy and paste this link into your browser:</p>
                        <p>{verificationLink}</p>
                        <p>If you did not create an account, please ignore this email.</p>
                        <div class='footer'>
                            <p>This is an automated message, please do not reply to this email.</p>
                        </div>
                    </div>
                </body>
                </html>";

            return await SendEmailAsync(to, subject, body, true);
        }
    }
}

