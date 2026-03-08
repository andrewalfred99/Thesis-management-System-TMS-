
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

namespace TMS.Pages.theses
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly PlatformContext _context;

        public DeleteModel(PlatformContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Thesis Thesis { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Thesis = await _context.Thesis
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Thesis == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Thesis = await _context.Thesis.FindAsync(id);

            if (Thesis != null)
            {
                _context.Thesis.Remove(Thesis);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
