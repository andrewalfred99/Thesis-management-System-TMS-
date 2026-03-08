
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
using TMS.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TMS.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
namespace TMS
{
    public class Program
    {
        /*
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        */

        private static async Task ConfigureIdentity(IServiceScope scope)
        {
            string[] roleNames = { "Admin", "Teacher", "Student" };

            var roleManger = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetService<UserManager<PlatformUser>>();

            IdentityResult roleResult;
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManger.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await roleManger.CreateAsync(new IdentityRole(roleName));
                    if (!roleResult.Succeeded)
                    {
                        throw new InvalidOperationException($"Unable to create {roleName} role.");
                    }
                }
            }

            var powerUser = new PlatformUser
            {
                Name = "Administrator",
                UserName = "Admin@Admin.com",
                Email = "Admin@Admin.com",
            };

            var adminUser = await userManager.FindByEmailAsync("Admin@Admin.com");
            if (adminUser == null)
            {
                var userResult = await userManager.CreateAsync(powerUser, "Qwerty1234%");
                if (!userResult.Succeeded)
                {
                    throw new InvalidOperationException($"Unable to create Admininstrator");
                }

                await userManager.AddToRoleAsync(powerUser, "Admin");

                adminUser = await userManager.FindByEmailAsync("Admin@Admin.com");
            }

            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(powerUser, "Admin");
            }
        }

        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {

                var services = scope.ServiceProvider;

                try
                {
                    var context = scope.ServiceProvider.GetService<PlatformContext>();

                    context.Database.Migrate();
                    ConfigureIdentity(scope).GetAwaiter().GetResult();

                    SeedData.Initialize(services);
                    SeedData.SetupUsers(scope).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred seeding the DB");
                }
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
