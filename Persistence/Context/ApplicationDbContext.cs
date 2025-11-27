using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Context
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            modelBuilder.Entity<User>(builder =>
            {
                builder.OwnsOne(u => u.Email);
                builder.OwnsOne(u => u.Phone);
                builder.OwnsOne(u => u.HashedPassword);
                builder.OwnsOne(u => u.FullName);
            });
            modelBuilder.Entity<Role>(builder =>
            {
                builder.OwnsOne(u => u.Name);
            });
            modelBuilder.Entity<Department>(builder =>
            {
                builder.OwnsOne(u => u.Name);
            });
            modelBuilder.Entity<Permission>(builder =>
            {
                builder.OwnsOne(u => u.Name);
            });

            modelBuilder.Entity<RefreshToken>(builder =>
            {
                builder.HasIndex(rt => rt.Token).IsUnique();
                builder.HasIndex(rt => rt.UserId);
                builder.HasOne(rt => rt.User)
                    .WithMany()
                    .HasForeignKey(rt => rt.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<UserRoleInDepartment> UserRoleInDepartments => Set<UserRoleInDepartment>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Permission>  Permissions => Set<Permission>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
        public DbSet<UserPermission> UserPermissions => Set<UserPermission>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();



    }

}
