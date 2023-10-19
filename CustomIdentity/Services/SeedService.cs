using CustomIdentity.Data;
using CustomIdentity.Enums;
using CustomIdentity.Models;
using CustomIdentity.Models.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CustomIdentity.Services
{
    public class SeedService
    {
        private readonly AppSettings _appSettings;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<CustomUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SeedService(IOptions<AppSettings> appSettings, ApplicationDbContext dbContext, UserManager<CustomUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _appSettings = appSettings.Value;
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task ManageDataAsync()
        {
            // Apply All Migrations
            await UpdateDatabaseAsync();

            // Add Roles Based on the Roles Enum
            await SeedRolesAsync();
            await SeedDefaultUserAsync();
        }

        private async Task UpdateDatabaseAsync()
        {
            await _dbContext.Database.MigrateAsync();
        }

        private async Task SeedRolesAsync()
        {

            try
            {
                // If Database Has Already Been Seeded, Do Nothing
                if (_dbContext.Roles.Any()) return;

                // Seed the AspNetRoles table with the Roles enum
                foreach (var role in Enum.GetNames(typeof(Roles)))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }

                // If DefaultCredentials is Missing from Settings, Do Nothing
                if (_appSettings.DefaultCredentials is null) return;

                var adminRole = _appSettings.DefaultCredentials.Role;

                await _roleManager.CreateAsync(new IdentityRole(adminRole));
            }
            catch (NullReferenceException)
            {
                return;
            }
            catch (Exception)
            {
                // TO-DO:  Show User Friendly Error Message Using Custom Alert Library
                return;
            }
        }

        private async Task SeedDefaultUserAsync()
        {
            try
            {
                // If the Database Already Has Users, Do Nothing
                if (_dbContext.Users.Any()) return;

                // Get the Default Credentials from AppSettings, if present
                var credentials = _appSettings.DefaultCredentials;
                if (credentials == null) return;

                var newUser = new CustomUser()
                {
                    FirstName = credentials.FirstName,
                    LastName = credentials.LastName,
                    Email = credentials.Email,
                    UserName = credentials.Email,
                    EmailConfirmed = true
                };

                await _userManager.CreateAsync(newUser, credentials.Password);
                await _userManager.AddToRoleAsync(newUser, credentials.Role);
            }
            catch (NullReferenceException)
            {
                return;
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
