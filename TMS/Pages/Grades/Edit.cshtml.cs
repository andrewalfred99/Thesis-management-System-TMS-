
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TMS.Areas.Identity.Data;
using TMS.Data;

namespace TMS.Pages.Grades
{
    [Authorize(Roles = "Teacher")]
    public class EditModel : PageModel
    {
        private readonly PlatformContext _context;

        public EditModel(PlatformContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Grade Grade { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Grade = await _context.Grades.FirstOrDefaultAsync(m => m.Id == id);

            if (Grade == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            var gradeToUpdate = await _context.Grades.FindAsync(id);

            if (gradeToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<Grade>(
                gradeToUpdate,
                "grade",
                s => s.Exam,
                s => s.Midterm))
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
