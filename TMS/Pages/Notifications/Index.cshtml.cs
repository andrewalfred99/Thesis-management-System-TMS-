
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

namespace TMS.Pages.Notifications
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly PlatformContext _context;

        public IndexModel(PlatformContext context)
        {
            _context = context;
        }

        public IList<Notification> Notifications { get;set; }

        public async Task OnGetAsync()
        {
            Notifications = await _context.Notifications.ToListAsync();
        }
    }
}
