
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
    public class CreateModel : PageModel
    {
        private readonly UserManager<PlatformUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<CreateModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly PlatformContext _context;

        public CreateModel(
            UserManager<PlatformUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<CreateModel> logger,
            IEmailSender emailSender,
            PlatformContext context)
        {
            _userManager = userManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

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

            [Required]
            [Display(Name = "Role")]
            [DataType(DataType.Text)]
            public string Role { get; set; }

            [DataType(DataType.Text)]
            public string Faculty { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public JsonResult OnGetFaculties()
        {
            IEnumerable<Faculty> faculties = _context.Faculties.AsNoTracking().ToList();

            return new JsonResult(faculties);
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;

            var roles = await _roleManager.Roles.Where(s => s.Name != "Admin").ToListAsync();
            roles.Insert(0, new IdentityRole { Id = "0", Name = "Select a role" });
            ViewData["roles"] = roles;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            var role = _roleManager.FindByIdAsync(Input.Role).Result;

            var roles = _roleManager.Roles.Where(s => s.Name != "Admin").OrderBy(s => s.Name).ToList();
            roles.Insert(0, new IdentityRole { Id = "0", Name = "Select a role" });
            ViewData["roles"] = roles;

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

            if (ModelState.IsValid)
            {
                PlatformUser user;

                if (role.Name == "Student")
                {
                    var selFaculty = _context.Faculties.FindAsync(new Guid(Input.Faculty)).Result;

                    user = new Student
                    {
                        Name = Input.Name,
                        DOB = Input.DOB,
                        UserName = Input.Email,
                        Email = Input.Email,
                        Faculty = selFaculty
                    };
                }
                else
                {
                    user = new PlatformUser
                    {
                        Name = Input.Name,
                        DOB = Input.DOB,
                        UserName = Input.Email,
                        Email = Input.Email
                    };
                }
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    await _userManager.AddToRoleAsync(user, role.Name);

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email", $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}