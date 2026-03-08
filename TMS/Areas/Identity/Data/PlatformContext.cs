using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TMS.Data;

namespace TMS.Areas.Identity.Data
{
    public class PlatformContext : IdentityDbContext<PlatformUser>
    {
        public PlatformContext(DbContextOptions<PlatformContext> options)
            : base(options)
        { 
        }
        public DbSet<Faculty> Faculties { get; set; }

        public DbSet<Student> Students { get; set; }

        public DbSet<Thesis> Thesis { get; set; }

        public DbSet<Grade> Grades { get; set; }

        public DbSet<Language> Languages { get; set; }

        public DbSet<Request> Requests { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Faculty>().ToTable("Faculty");

            builder.Entity<Student>().ToTable("Student");

            builder.Entity<Thesis>().ToTable("Thesis");

            builder.Entity<Thesis>()
                .HasKey(c => new { c.Id, c.Name });

            builder.Entity<Grade>().ToTable("Grade");

            builder.Entity<Language>().ToTable("Language");

            builder.Entity<Request>().ToTable("Request");

            builder.Entity<Notification>().ToTable("Notification");
        }
    }
}
