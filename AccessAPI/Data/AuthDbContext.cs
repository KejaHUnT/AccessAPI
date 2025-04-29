using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AccessAPI.Data
{
    public class AuthDbContext : IdentityDbContext
    {
        public AuthDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var managerRoleId = "0f8fad5b-d9cb-469f-a165-70867728950e";
            var tenantRoleId = "7c9e6679-7425-40de-944b-e07fc1f90ae7";

            // create writer and reader roles
            var roles = new List<IdentityRole>
            {
                new IdentityRole()
                {
                    Id = managerRoleId,
                    Name = "Manager",
                    NormalizedName = "Manager".ToUpper(),
                    ConcurrencyStamp = managerRoleId
                },
                new IdentityRole()
                {
                    Id = tenantRoleId,
                    Name = "Tenant",
                    NormalizedName = "Tenant".ToUpper(),
                    ConcurrencyStamp = tenantRoleId
                }
            };
            // Seed roles
            builder.Entity<IdentityRole>().HasData(roles);

            //create admin user 
            var adminUserId = "3d813cbb-47fb-32ba-91df-831e1593ac29";
            var admin = new IdentityUser()
            {
                Id = adminUserId,
                UserName = "admin@KejaHUnT.com",
                Email = "admin@KejaHUnT.com",
                NormalizedEmail = "admin@KejaHUnT.com".ToUpper(),
                NormalizedUserName = "admin@KejaHUnT.com".ToUpper()
            };

            admin.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(admin, "Admin@m23");

            builder.Entity<IdentityUser>().HasData(admin);

            // Give roles to Admin

            var adminRoles = new List<IdentityUserRole<string>>()
            {
                new()
                {
                    UserId = adminUserId,
                    RoleId = managerRoleId
                },
                 new()
                {
                    UserId = adminUserId,
                    RoleId = tenantRoleId
                }
            };

            builder.Entity<IdentityUserRole<string>>().HasData(adminRoles);

        }
    }
}
