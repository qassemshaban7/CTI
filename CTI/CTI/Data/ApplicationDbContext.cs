using CTI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

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
        public DbSet<Course> Courses { get; set; }
        public DbSet<ApplicationUserCourse> ApplicationUserCourses { get; set; }
        public DbSet<Survey> Surveys { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Result> Results { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            string ADMIN_ID = "ecc07b18-f55e-4f6b-95bd-0e84f556135f";
            string ADMIN_ROLE_ID = "ba51b8f7-2a1d-45c6-9c00-68099eebd485";

            string TRAINER_ROLE_ID = "68099eeb-4f6b-45c6-9c00-68099eebd485";

            string TRAINEE_ROLE_ID = "9eebd485-2a1d-45c6-9c00-68099eebd485";

            builder.Entity<ApplicationRole>().HasData(new ApplicationRole
            {
                Name = "Admin",
                NormalizedName = "ADMIN",
                Id = ADMIN_ROLE_ID,
                ConcurrencyStamp = ADMIN_ROLE_ID,
                ArabicRoleName = "الادمن"
            });

            builder.Entity<ApplicationRole>().HasData(new ApplicationRole
            {
                Name = "Trainee",
                NormalizedName = "TRAINEE",
                Id = TRAINEE_ROLE_ID,
                ConcurrencyStamp = TRAINEE_ROLE_ID,
                ArabicRoleName = "المتدرب"
            });

            builder.Entity<ApplicationRole>().HasData(new ApplicationRole
            {
                Name = "Trainer",
                NormalizedName = "TRAINER",
                Id = TRAINER_ROLE_ID,
                ConcurrencyStamp = TRAINER_ROLE_ID,
                ArabicRoleName = "المدرب"
            });

            var appUser = new ApplicationUser
            {
                Id = ADMIN_ID,
                EmailConfirmed = true,
                UserFullName = "الادمن",
                UserName = "Admin",
                NormalizedUserName = "ADMIN",
            };

            PasswordHasher<ApplicationUser> ph = new PasswordHasher<ApplicationUser>();
            appUser.PasswordHash = ph.HashPassword(appUser, "P@ssw0rd");

            builder.Entity<ApplicationUser>().HasData(appUser);

            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = ADMIN_ROLE_ID,
                UserId = ADMIN_ID
            });

            builder.Entity<ApplicationUserCourse>().HasKey(am => new
            {
                am.CourseId,
                am.UserId
            });

            builder.Entity<ApplicationUserCourse>().HasOne(m => m.Course).WithMany(am => am.ApplicationUserCourses).HasForeignKey(m => m.CourseId);
            builder.Entity<ApplicationUserCourse>().HasOne(m => m.ApplicationUser).WithMany(am => am.ApplicationUserCourses).HasForeignKey(m => m.UserId);

            builder.Entity<Course>()
                .HasOne(a => a.ApplicationUser)
                .WithOne(c => c.Course)
                .HasForeignKey<Course>(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Question>()
                .HasMany(q => q.Results)
                .WithOne(r => r.Question)
                .HasForeignKey(r => r.QuestionId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Answer>()
                .HasMany(a => a.Results)
                .WithOne(r => r.Answer)
                .HasForeignKey(r => r.AnswerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Survey>()
                .HasMany(s => s.Questions)
                .WithOne(q => q.Survey)
                .HasForeignKey(q => q.SurveyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Question>()
                .HasMany(q => q.Answers)
                .WithOne(a => a.Question)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(builder);
        }
    }
}