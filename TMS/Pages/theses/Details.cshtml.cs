
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

namespace TMS.Pages.Theses
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly UserManager<PlatformUser> _userManager;
        private readonly PlatformContext _context;

        public DetailsModel(
            UserManager<PlatformUser> userManager, 
            PlatformContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public Thesis Thesis { get; set; }

        public List<Notification> Notifications { get; set; }

        public List<PlatformUser> Teachers { get; set; }

        public List<Language> Languages { get; set; }

        public List<Request> Requests { get; set; }

        public async Task<IActionResult> OnPostRequestAccAsync(Guid? id)
        {
            var Thesis = await _context.Thesis.FirstOrDefaultAsync(m => m.Id == id);

            if (Thesis == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var student = await _context.Students.FindAsync(userId);

            if(Thesis.Students.Contains(student))
            {
                return NotFound();
            }

            var request = new Request
            {
                Thesis = Thesis,
                Student = student,
                Approved = false
            };

            await _context.Requests.AddAsync(request);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Thesis = await _context.Thesis
                .Include(d => d.Faculty)
                .Include(d => d.Students)
                .Include(d => d.Teachers)
                .Include(d => d.Languages)
                .Include(d => d.Notifications)
                .AsSplitQuery()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Thesis == null)
            {
                return NotFound();
            }

            Teachers = Thesis.Teachers.ToList();
            Languages = Thesis.Languages.ToList();
            Notifications = Thesis.Notifications.ToList();

            Requests = await _context.Requests
                .Include(r => r.Student)
                .Where(s => s.Thesis == Thesis)
                .AsNoTracking()
                .ToListAsync();

            return Page();
        }
    }
}
