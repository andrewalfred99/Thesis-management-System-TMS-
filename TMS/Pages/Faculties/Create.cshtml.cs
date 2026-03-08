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

namespace TMS.Pages.Faculties
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly PlatformContext _context;

        public CreateModel(PlatformContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public FacultyVM Faculty { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var entry = _context.Add(new Faculty());

            entry.CurrentValues.SetValues(Faculty);

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
