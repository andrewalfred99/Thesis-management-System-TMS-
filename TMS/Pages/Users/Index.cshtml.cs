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

namespace TMS.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<PlatformUser> _userManager;
        private readonly PlatformContext _context;

        public IndexModel(
            UserManager<PlatformUser> userManager,
            PlatformContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int? PageIndex { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SelectedLetter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }

        public IList<string> Alphabet
        {
            get
            {
                var alphabet = Enumerable.Range(65, 26).Select(i => ((char)i).ToString()).ToList();

                alphabet.Insert(0, "All");

                return alphabet;
            }
        }

        public string CurrentFilter { get; private set; }
        public IList<string> FirstLetters { get; private set; }
        public PaginatedList<PlatformUser> Users { get; private set; }

        public async Task OnGetAsync()
        {
            IList<PlatformUser> usersAll = await _context.Users
                                        .AsNoTracking()
                                        .ToListAsync();

            FirstLetters = usersAll.ToList()
                .GroupBy(c => c.Name.ToString().Substring(0, 1))
                .Select(x => x.Key.ToUpper())
                .ToList();

            if (SearchString != null)
            {
                PageIndex = 1;
            }

            CurrentFilter = SearchString;
            SelectedLetter ??= "";

            if (!String.IsNullOrEmpty(SearchString))
            {
                SearchString = SearchString.ToUpper();
                usersAll = usersAll.Where(x => x.Name.ToUpper().Contains(SearchString))
                                        .ToList();
            }

            if (string.IsNullOrEmpty(SelectedLetter) || SelectedLetter == "All")
            {
                SelectedLetter = "All";
            }
            else
            {
                usersAll = usersAll.Where(x => x.Name.ToUpper().StartsWith(SelectedLetter))
                                            .ToList();
            }

            Users = PaginatedList<PlatformUser>.Create(usersAll, PageIndex ?? 1, 40);
        }
    }
}
