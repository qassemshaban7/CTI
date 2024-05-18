using CTI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CTI.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            string SUPER_ADMIN_ID = "ecc07b18-f55e-4f6b-95bd-0e84f556135f";
            string SUPER_ADMIN_ROLE_ID = "ba51b8f7-2a1d-45c6-9c00-68099eebd485";

            builder.Entity<ApplicationRole>().HasData(new ApplicationRole
            {
                Name = "SuperAdmin",
                NormalizedName = "SUPERADMIN",
                Id = SUPER_ADMIN_ROLE_ID,
                ConcurrencyStamp = SUPER_ADMIN_ROLE_ID,
                ArabicRoleName = "الادمن"
            });

            var appUser = new ApplicationUser
            {
                Id = SUPER_ADMIN_ID,
                EmailConfirmed = true,
                FullName = "الادمن",
                UserName = "Admin",
                NormalizedUserName = "ADMIN",
            };

            //set user password
            PasswordHasher<ApplicationUser> ph = new PasswordHasher<ApplicationUser>();
            appUser.PasswordHash = ph.HashPassword(appUser, "P@ssw0rd");

            builder.Entity<ApplicationUser>().HasData(appUser);

            //set user role to admin
            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = SUPER_ADMIN_ROLE_ID,
                UserId = SUPER_ADMIN_ID
            });
            base.OnModelCreating(builder);
        }
    }
}