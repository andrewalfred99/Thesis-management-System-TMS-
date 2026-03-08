
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TMS.Areas.Identity.Data;

namespace TMS.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class DetailsModel : PageModel
    {
        private readonly UserManager<PlatformUser> _userManager;
        private readonly PlatformContext _context;

        public DetailsModel(
            UserManager<PlatformUser> userManager,
            PlatformContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public Guid UserID { get; set; }

        public string Username { get; set; }

        public List<string> UserRoles { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Full name")]
            public string Name { get; set; }

            [Required]
            [Display(Name = "Birth Date")]
            [DataType(DataType.Date)]
            public DateTime DOB { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        public StudentModel StudentData { get; private set; }

        public class StudentModel
        {
            public string Faculty { get; set; }
        }

        private async Task LoadAsync(PlatformUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            Username = userName;
            UserRoles = userRoles.ToList();

            Input = new InputModel
            {
                Name = user.Name,
                DOB = user.DOB,
                PhoneNumber = phoneNumber
            };

            if (await _userManager.IsInRoleAsync(user, "Student"))
            {
                var student = _context.Students.Find(user.Id);

                if (student != null)
                {
                    await _context.Entry(student).Reference(s => s.Faculty).LoadAsync();

                    StudentData = new StudentModel
                    {
                        Faculty = student.Faculty.Name
                    };
                }
            }
        }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (id.ToString() == _userManager.GetUserId(User))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            UserID = (Guid) id;

            await LoadAsync(user);

            return Page();
        }
    }
}
