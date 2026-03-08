
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using TMS.Data;
using TMS.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TMS.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly UserManager<PlatformUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly PlatformContext _context;

        public EditModel(
            UserManager<PlatformUser> userManager,
            RoleManager<IdentityRole> roleManager,
            PlatformContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public Guid UserID { get; set; }

        public string Username { get; set; }

        public List<string> UserRoles { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Role")]
            [DataType(DataType.Text)]
            public string Role { get; set; }

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

            [DataType(DataType.Text)]
            public string Faculty { get; set; }
        }

        public StudentModel StudentData { get; private set; }

        public class StudentModel
        {
            public string Faculty { get; set; }
        }

        public JsonResult OnGetFaculties()
        {
            IEnumerable<Faculty> faculties = _context.Faculties.AsNoTracking().ToList();

            return new JsonResult(faculties);
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

                    Input.Faculty = StudentData.Faculty;
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

            var roles = _roleManager.Roles.Where(s => s.Name != "Admin").OrderBy(s => s.Name).ToList();
            roles.Insert(0, new IdentityRole { Id = "0", Name = "Select a role" });
            ViewData["roles"] = roles;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (id.ToString() == _userManager.GetUserId(User))
            {
                return NotFound();
            }

            var role = _roleManager.FindByIdAsync(Input.Role).Result;
            var user = await _userManager.FindByIdAsync(id.ToString());

            var roles = _roleManager.Roles.Where(s => s.Name != "Admin").OrderBy(s => s.Name).ToList();
            roles.Insert(0, new IdentityRole { Id = "0", Name = "Select a role" });
            ViewData["roles"] = roles;

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (role == null)
            {
                ModelState.AddModelError("Input.Role", "Please select a Role");
                return Page();
            }

            if (role.Name == "Student")
            {
                try
                {
                    var selFaculty = _context.Faculties.FindAsync(new Guid(Input.Faculty)).Result;
                    if (selFaculty == null) { ModelState.AddModelError("Input.Faculty", "Error in the selection"); }
                }
                catch { ModelState.AddModelError("Input.Faculty", "Error in the selection"); }
            }

            if (!ModelState.IsValid)
            {
                UserID = (Guid)id;

                await LoadAsync(user);

                return Page();
            }

            if (Input.Name != user.Name)
            {
                user.Name = Input.Name;
            }

            if (Input.DOB != user.DOB)
            {
                user.DOB = Input.DOB;
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            await _userManager.UpdateAsync(user);

            StatusMessage = "User information has been updated";

            if (role.Name == "Student")
            {
                var selFaculty = _context.Faculties.FindAsync(new Guid(Input.Faculty)).Result;

                var student = _context.Students.Find(user.Id);

                if (student != null)
                {
                    await _context.Entry(student).Reference(s => s.Faculty).LoadAsync();

                    if (selFaculty != student.Faculty)
                    {
                        student.Faculty = selFaculty;
                    }

                    _context.Students.Update(student);
                    await _context.SaveChangesAsync();

                    StatusMessage = "Student information has been updated";
                }
            }

            if (!(await _userManager.IsInRoleAsync(user, role.Name)))
            {
                await _userManager.AddToRoleAsync(user, role.Name);
            }

            UserID = (Guid)id;
            await LoadAsync(user);

            return Page();
        }
    }
}
