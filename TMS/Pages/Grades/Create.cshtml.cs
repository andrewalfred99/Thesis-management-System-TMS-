using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TMS.Areas.Identity.Data;
using TMS.Data;

namespace TMS.Pages.Grades
{
    [Authorize(Roles = "Teacher")]
    public class CreateModel : PageModel
    {
        private readonly PlatformContext _context;
        private readonly UserManager<PlatformUser> _userManager;

        public CreateModel(UserManager<PlatformUser> userManager, PlatformContext context)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Grade Grade { get; set; }

        public async Task<IActionResult> OnPostAddGradeAsync(Guid? ThesisId, Guid? studentId)
        {
            if (ThesisId == null || studentId == null)
            {
                return NotFound();
            }

            var Thesis = await _context.Thesis.FirstOrDefaultAsync(m => m.Id == ThesisId);
            var student = await _context.Students.FindAsync(studentId.ToString());

            var teacher = await _userManager.GetUserAsync(User);

            if (Thesis == null || student == null || !Thesis.Teachers.Contains(teacher))
            {
                return NotFound();
            }
            
            Grade.Thesis = Thesis;
            Grade.Student = student;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var entry = _context.Add(new Grade());

            entry.CurrentValues.SetValues(Grade);

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
