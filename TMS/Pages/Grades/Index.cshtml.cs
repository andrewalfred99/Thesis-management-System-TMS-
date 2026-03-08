
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TMS.Areas.Identity.Data;
using TMS.Data;

namespace TMS.Pages.Grades
{
    [Authorize(Roles = "Teacher")]
    public class IndexModel : PageModel
    {
        private readonly PlatformContext _context;

        public IndexModel(PlatformContext context)
        {
            _context = context;
        }

        public IList<Grade> Grades { get;set; }

        public async Task OnGetAsync()
        {
            Grades = await _context.Grades
                .Include(g => g.Thesis)
                .Include(g => g.Student)
                .ToListAsync();
        }
    }
}
