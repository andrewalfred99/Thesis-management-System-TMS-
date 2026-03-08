
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

using TMS.Data;
using TMS.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace TMS.Pages.theses
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly PlatformContext _context;
        private readonly UserManager<PlatformUser> _userManager;

        public CreateModel(UserManager<PlatformUser> userManager, PlatformContext context)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Language> Languages { get; set; }
        public IList<PlatformUser> Teachers { get; set; }

        [BindProperty]
        public Thesis Thesis { get; set; }

        [BindProperty]
        public List<Guid> CheckedLanguages { get; set; }
        
        [BindProperty]
        public List<Guid> CheckedTeachers { get; set; }

        public IActionResult OnGet()
        {
            Languages = _context.Languages.ToList();
            Teachers = _userManager.GetUsersInRoleAsync("Teacher").Result;
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "Id", "Name");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            List<Notification> Notifications = new List<Notification>();
            List<PlatformUser> Teachers = new List<PlatformUser>();
            List<Language> Languages = _context.Languages
                .Where(g => CheckedLanguages.Contains(g.Id))
                .ToList();

            //Notification
            var notification = new Notification
            {
                DateTime = DateTime.Now,
                Title = $"{Thesis.Name} -- NEW",
                Description = "Thesis just created"
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            Notifications.Add(notification);

            foreach (var teacherID in CheckedTeachers)
            {
                if (_userManager.Users.Any(u => u.Id == teacherID.ToString()))
                {
                    Teachers.Add(_context.Users.Find(teacherID.ToString()));
                }
            }

            var newThesis = new Thesis()
            {
                Year = Thesis.Year,
                Name = Thesis.Name,
                Teachers = Teachers,
                Languages = Languages,
                Notifications = Notifications
            };

            var entry = _context.Add(newThesis);

            entry.CurrentValues.SetValues(Thesis);

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
