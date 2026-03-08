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

namespace TMS.Pages
{
    [Authorize]
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

        public List<Thesis> Thesis { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (User.IsInRole("Student"))
            {
                var id = _userManager.GetUserId(User);
                var student = _context.Students.Find(id);

                if (student != null)
                {
                    await _context.Entry(student).Reference(s => s.Faculty).LoadAsync();

                    Thesis = await _context.Thesis
                        .Include(c => c.Languages)
                        .Include(c => c.Teachers)
                        .Include(c => c.Students)
                        .AsSplitQuery()
                        .AsNoTracking()
                        .ToListAsync();
                }
            }

            if (User.IsInRole("Teacher"))
            {
                var id = _userManager.GetUserId(User);
                var teacher = _context.Users.Find(id);

                if (teacher != null)
                {
                    Thesis = await _context.Thesis
                        .Include(c => c.Languages)
                        .Include(c => c.Teachers)
                        .Include(c => c.Students)
                        .Where(d => d.Teachers.Contains(teacher))
                        .AsSplitQuery()
                        .AsNoTracking()
                        .ToListAsync();
                }
            }

            return Page();
        }
    }
}
