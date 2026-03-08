using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TMS.Areas.Identity.Data;
using TMS.Data;

namespace TMS.Pages.Requests
{
    [Authorize(Roles = "Teacher")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<PlatformUser> _userManager;
        private readonly PlatformContext _context;

        public IndexModel(
            UserManager<PlatformUser> userManager,
            PlatformContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IList<Request> Requests { get;set; }

        public async Task<IActionResult> OnPostApproveAsync(Guid? id)
        {
            var request = await _context.Requests
                .Include(r => r.Student)
                .Include(r => r.Thesis)
                .ThenInclude(c => c.Teachers)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            var teacher = await _userManager.GetUserAsync(User);

            if (request.Thesis.Teachers.Contains(teacher))
            {
                var ThesisToUpdate = await _context.Thesis
                    .Include(c => c.Students)
                    .FirstOrDefaultAsync(m => m.Id == request.Thesis.Id);

                if (ThesisToUpdate == null)
                {
                    return NotFound();
                }

                ThesisToUpdate.Students.Add(request.Student);

                if (await TryUpdateModelAsync<Thesis>(
                    ThesisToUpdate,
                    "Thesis",
                    s => s.Students))
                {
                    await _context.SaveChangesAsync();
                }

                request.Approved = true;
            }
            else
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<Request>(
                request,
                "request",
                s => s.Approved))
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return RedirectToPage("./Index");
        }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            Requests = await _context.Requests
                .Include(r => r.Student)
                .Include(r => r.Thesis)
                 .ThenInclude(c => c.Teachers)
                .Where(r => r.Thesis.Teachers.Contains(user))
                .ToListAsync();
        }
    }
}
