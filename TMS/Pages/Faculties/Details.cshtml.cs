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

namespace TMS.Pages.Faculties
{
    [Authorize(Roles = "Admin")]
    public class DetailsModel : PageModel
    {
        private readonly PlatformContext _context;

        public DetailsModel(PlatformContext context)
        {
            _context = context;
        }

        public Faculty Faculty { get; set; }
        public List<Thesis> Thesis { get; private set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Faculty = await _context.Faculties
                            .Include(e => e.Thesis)
                            .AsNoTracking()
                            .FirstOrDefaultAsync(m => m.Id == id);

            if (Faculty == null)
            {
                return NotFound();
            }

            Thesis = await _context.Thesis
                                .Where(d => d.FacultyId == Faculty.Id)
                                .AsNoTracking()
                                .ToListAsync();

            return Page();
        }
    }
}
