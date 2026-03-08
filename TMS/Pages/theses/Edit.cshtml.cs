
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using TMS.Data;
using TMS.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TMS.Pages.Theses
{
    [Authorize(Roles = "Admin, Teacher")]
    public class EditModel : PageModel
    {
        private readonly PlatformContext _context;
        private readonly UserManager<PlatformUser> _userManager;

        public EditModel(UserManager<PlatformUser> userManager, PlatformContext context)
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

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Thesis = await _context.Thesis
                .Include(d => d.Teachers)
                .Include(d => d.Languages)
                .AsSplitQuery()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Thesis == null)
            {
                return NotFound();
            }

            Languages = _context.Languages.ToList();
            Teachers = _userManager.GetUsersInRoleAsync("Teacher").Result;
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "Id", "Name");

            var userID = _userManager.GetUserId(User);

            if (User.IsInRole("Teacher") && !Teachers.Contains(await _userManager.FindByIdAsync(userID)))
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            var ThesisToUpdate = await _context.Thesis
                .Include(d => d.Teachers)
                .Include(d => d.Languages)
                .Include(d => d.Notifications)
                .AsSplitQuery()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ThesisToUpdate == null)
            {
                return NotFound();
            }

            var userID = _userManager.GetUserId(User);
            var currentTeacher = await _userManager.FindByIdAsync(userID);

            if (User.IsInRole("Teacher") &&  !ThesisToUpdate.Teachers.Contains(currentTeacher))
            {
                return NotFound();
            }

            List<PlatformUser> Teachers = new List<PlatformUser>();
            List<Language> Languages = _context.Languages.Where(g => CheckedLanguages.Contains(g.Id))
                .AsSplitQuery()
                .ToList();

            foreach (var teacherID in CheckedTeachers)
            {
                if (_userManager.Users.Any(u => u.Id == teacherID.ToString()))
                {
                    Teachers.Add(_context.Users.Find(teacherID.ToString()));
                }
            }

            if (User.IsInRole("Teacher") && !Teachers.Contains(currentTeacher))
            {
                Teachers.Add(currentTeacher);
            }

            ThesisToUpdate.Languages = Languages;
            ThesisToUpdate.Teachers = Teachers;

            if (ThesisToUpdate.Status != Thesis.Status)
            {
                //Notification
                var notification = new Notification
                {
                    DateTime = DateTime.Now,
                    Title = $"{Thesis.Name} -- Updated",
                    Description = $"Status changed from {ThesisToUpdate.Status} to {Thesis.Status}"
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                ThesisToUpdate.Notifications.Add(notification);
            }

            if (await TryUpdateModelAsync<Thesis>(
                ThesisToUpdate,
                "Thesis",
                s => s.Status,
                s => s.Semester,
                s => s.ExpectedHours,
                s => s.Year,
                s => s.Name,
                s => s.Notifications,
                s => s.Languages,
                s => s.Teachers,
                s => s.Description,
                s => s.Materials))
            {
                await _context.SaveChangesAsync();

                if (User.IsInRole("Admin"))
                {
                    return RedirectToPage("./Index");
                }
                else
                {
                    return RedirectToPage("/Index");
                }
            }

            return Page();
        }
    }
}
