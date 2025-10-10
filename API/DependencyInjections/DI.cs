

using Application.Features.AuthenticationUseCase.Services;
using Application.Features.AuthorizationUseCase.Handlers;
using Application.Features.AuthorizationUseCase.Provider;
using Application.Features.AuthorizationUseCase.Services;
using Domain.Base.Interface;
using Domain.Repositories;
using Infrastructure.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Repositories;
using Persistence.UnitOfWork;
using pplication.Features.AuthorizationUseCase.Services;

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

            // اگر سرویس دیگه‌ای داری، اینجا اضافه کن

            return services;
        }
    }
}
