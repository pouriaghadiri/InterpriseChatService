

using Application.Features.AuthenticationUseCase.Services;
using Application.Features.AuthorizationUseCase.Handlers;
using Application.Features.AuthorizationUseCase.Provider;
using Application.Features.AuthorizationUseCase.Services;
using Domain.Base.Interface;
using Domain.Repositories;
using Domain.Services;
using Infrastructure.Extensions;
using Infrastructure.Services;
using Infrastructure.Services.Authentication;
using Infrastructure.Services.Email;
using Application.Common.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Repositories;
using Persistence.UnitOfWork;
using pplication.Features.AuthorizationUseCase.Services;
using Microsoft.Extensions.Options;

namespace API.DependencyInjections
{
    public static class DI
    {
        public static IServiceCollection AddDependencyInjections(this IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Register Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IUserRoleInDepartmentRepository, UserRoleInDepartmentRepository>();
            
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IUserPermissionRepository, UserPermissionRepository>();
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();

            // Register UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register Services
            services.AddSingleton<IJwtTokenService, JwtTokenService>();
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();
            services.AddScoped<IActiveDepartmentService, ActiveDepartmentService>();
            services.AddScoped<ICacheInvalidationService, CacheInvalidationService>();
            services.AddScoped<IEmailService, SmtpEmailService>();

            // Configure Email Settings
            services.Configure<EmailSettingsDTO>(configuration.GetSection("EmailSettings"));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<EmailSettingsDTO>>().Value);

            // Register Redis Cache
            services.AddRedisCache(configuration);

            // Register HttpContextAccessor for accessing current user context
            services.AddHttpContextAccessor();

            return services;
        }
    }
}
