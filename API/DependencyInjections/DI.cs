

using Application.Features.AuthUseCase.Interfaces;
using Domain.Base.Interface;
using Domain.Repositories;
using Infrastructure.Auth;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Repositories;
using Persistence.UnitOfWork;

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

            // Register UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register Services
            services.AddSingleton<IJwtTokenService, JwtTokenService>();


            // اگر سرویس دیگه‌ای داری، اینجا اضافه کن

            return services;
        }
    }
}
