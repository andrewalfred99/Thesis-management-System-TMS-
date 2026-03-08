using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using TMS.Data;
using TMS.Areas.Identity.Data;

namespace TMS.Models
{
    public class SeedData
    {

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new PlatformContext(serviceProvider.GetRequiredService<DbContextOptions<PlatformContext>>()))
            {
                // Notifications
                // Looks for any notoication
                if (!context.Notifications.Any())
                {
                    var notifications = new Notification[]
                    {
                        new Notification { Title="Thesis 1", Description="description...", DateTime = new DateTime(2022,01,18) },
                        new Notification { Title="Thesis 2", Description="description...", DateTime = new DateTime(2022,01,18) },
                        new Notification { Title="Thesis 3", Description="description...", DateTime = new DateTime(2022,01,18) },
                        new Notification { Title="Thesis 4", Description="description...", DateTime = new DateTime(2022,01,18) }
                    };
                    context.Notifications.AddRange(notifications);
                    context.SaveChanges();
                }

                // Languages
                // Look for any languages.
                if (!context.Languages.Any())
                {
                    var languages = new Language[]
                    {
                        new Language { Name="English", Code="EN" },
                        new Language { Name="Russian", Code="RU" }

                    };

                    context.Languages.AddRange(languages);
                    context.SaveChanges();
                }

                // Faculty
                // Look for any faculties.
                if (!context.Faculties.Any())
                {
                    var faculties = new Faculty[]
                    {
                        new Faculty { Name="HITS" },
                        new Faculty { Name="Mass Comunication" },
                        new Faculty { Name="History" },
                        new Faculty { Name="Law" }
                    };

                    context.Faculties.AddRange(faculties);
                    context.SaveChanges();
                }
            }
        }
        public static async Task SetupUsers(IServiceScope scope)
        {
            var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetService<UserManager<PlatformUser>>();
            var context = scope.ServiceProvider.GetService<PlatformContext>();

            // ensure roles exist
            foreach (var roleName in new[] { "Teacher", "Student" })
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Teachers
            var teachers = await userManager.GetUsersInRoleAsync("Teacher");
            if (teachers.Count == 0)
            {
                var teacher1 = new PlatformUser
                {
                    Name = "Teacher 1",
                    UserName = "teacher1@teacher.com",
                    Email = "teacher1@teacher.com",
                };
                await userManager.CreateAsync(teacher1, "Qwerty1234%");
                await userManager.AddToRoleAsync(teacher1, "Teacher");

                var teacher2 = new PlatformUser
                {
                    Name = "agnya",
                    UserName = "teacher2@teacher.com",
                    Email = "teacher2@teacher.com",
                };
                await userManager.CreateAsync(teacher2, "Qwerty1234%");
                await userManager.AddToRoleAsync(teacher2, "Teacher");
            }

            // Teacher (known demo account)
            const string teacherEmail = "teacher@tms.local";
            const string teacherPassword = "Teacher1234%";
            var teacher = await userManager.FindByEmailAsync(teacherEmail);
            if (teacher == null)
            {
                teacher = new PlatformUser
                {
                    Name = "Teacher",
                    UserName = teacherEmail,
                    Email = teacherEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(teacher, teacherPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(teacher, "Teacher");
                }
            }
            else if (!await userManager.IsInRoleAsync(teacher, "Teacher"))
            {
                await userManager.AddToRoleAsync(teacher, "Teacher");
            }

            // Students
            var students = await userManager.GetUsersInRoleAsync("Student");
            if (students.Count == 0)
            {
                var f1 = context.Faculties.Where(s => s.Name == "HITS").Single();
                var s1 = new Student
                {
                    Name = "Andrew Alfred",
                    UserName = "Student1@Student.com",
                    Email = "Student1@Student.com",
                    Faculty = f1
                };
                await userManager.CreateAsync(s1, "Qwerty1234%");
                await userManager.AddToRoleAsync(s1, "Student");

                var f2 = context.Faculties.Where(s => s.Name == "HITS").Single();
                var s2 = new Student
                {
                    Name = "Omar Assadi",
                    UserName = "Student2@Student.com",
                    Email = "Student2@Student.com",
                    Faculty = f2
                };
                await userManager.CreateAsync(s2, "Qwerty1234%");
                await userManager.AddToRoleAsync(s2, "Student");

                var f3 = context.Faculties.Where(s => s.Name == "History").Single();
                var s3 = new Student
                {
                    Name = "Student 3",
                    UserName = "Student1@Student.com",
                    Email = "Student3@Student.com",
                    Faculty = f3
                };
                await userManager.CreateAsync(s3, "Qwerty1234%");
                await userManager.AddToRoleAsync(s3, "Student");

             
            }

            // Student (known demo account)
            const string studentEmail = "student@tms.local";
            const string studentPassword = "Student1234%";
            var student = await userManager.FindByEmailAsync(studentEmail);
            if (student == null)
            {
                var faculty = context.Faculties.SingleOrDefault(s => s.Name == "HITS")
                    ?? context.Faculties.FirstOrDefault();

                student = new Student
                {
                    Name = "Student",
                    UserName = studentEmail,
                    Email = studentEmail,
                    EmailConfirmed = true,
                    Faculty = faculty
                };

                var result = await userManager.CreateAsync(student, studentPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(student, "Student");
                }
            }
            else if (!await userManager.IsInRoleAsync(student, "Student"))
            {
                await userManager.AddToRoleAsync(student, "Student");
            }
        }
    }
}
