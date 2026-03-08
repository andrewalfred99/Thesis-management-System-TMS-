
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
    public class OwnModel : PageModel
    {
        private readonly UserManager<PlatformUser> _userManager;
        private readonly PlatformContext _context;

        public OwnModel(
            UserManager<PlatformUser> userManager,
            PlatformContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public List<Request> Requests { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (User.IsInRole("Student"))
            {
                var id = _userManager.GetUserId(User);
                var student = _context.Students.Find(id);

                if (student != null)
                {
                    //await _context.Entry(student).Reference(s => s.Faculty).LoadAsync();

                    Requests = await _context.Requests
                        .Where(r => r.Student.Id == id)
                        .Include(r => r.Thesis)
                         .ThenInclude(c => c.Teachers)
                        .AsNoTracking()
                        .ToListAsync();
                }
            }
            return Page();
        }
    }
}
