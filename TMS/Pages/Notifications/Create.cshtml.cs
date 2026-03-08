
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TMS.Areas.Identity.Data;
using TMS.Data;

namespace TMS.Pages.Notifications
{
    [Authorize(Roles = "Admin, Teacher")]
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
        public NotificationVM Notification { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var entry = _context.Add(new Notification());

            entry.CurrentValues.SetValues(Notification);

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
