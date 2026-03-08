using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using TMS.Data;
using TMS.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;

namespace TMS.Pages.Faculties
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly PlatformContext _context;

        public EditModel(PlatformContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Faculty Faculty { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Faculty = await _context.Faculties.FirstOrDefaultAsync(m => m.Id == id);

            if (Faculty == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            var facultyToUpdate = await _context.Faculties.FindAsync(id);

            if (facultyToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<Faculty>(
                facultyToUpdate,
                "faculty",
                s => s.Name))
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
