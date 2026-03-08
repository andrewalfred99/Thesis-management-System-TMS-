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
    public class IndexModel : PageModel
    {
        private readonly PlatformContext _context;

        public IndexModel(PlatformContext context)
        {
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
        public PaginatedList<Faculty> Faculties { get; private set; }

        public async Task OnGetAsync()
        {
            IList<Faculty> facultiesAll = await _context.Faculties
                                        .Include(e => e.Thesis)
                                        .AsNoTracking()
                                        .ToListAsync();

            FirstLetters = facultiesAll.ToList()
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
                facultiesAll = facultiesAll.Where(x => x.Name.ToUpper().Contains(SearchString))
                                        .ToList();
            }

            if (string.IsNullOrEmpty(SelectedLetter) || SelectedLetter == "All")
            {
                SelectedLetter = "All";
            }
            else
            {
                facultiesAll = facultiesAll.Where(x => x.Name.ToUpper().StartsWith(SelectedLetter))
                                            .ToList();
            }

            Faculties = PaginatedList<Faculty>.Create(facultiesAll, PageIndex ?? 1, 40);
        }
    }
}
