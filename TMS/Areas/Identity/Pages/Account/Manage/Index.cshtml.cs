using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TMS.Areas.Identity.Data;

namespace TMS.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly PlatformContext _context;
        private readonly UserManager<PlatformUser> _userManager;
        private readonly SignInManager<PlatformUser> _signInManager;
        private readonly IWebHostEnvironment webHostEnvironment;

        public IndexModel(
            UserManager<PlatformUser> userManager,
            SignInManager<PlatformUser> signInManager,
            IWebHostEnvironment hostEnvironment,
            PlatformContext context)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            webHostEnvironment = hostEnvironment;
        }

        public string Username { get; set; }
        public string ProfilePicture { get; set; }

        public List<string> UserRoles { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

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

            [Display(Name = "Profile Image")]
            public IFormFile ProfileImage { get; set; }
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

            ProfilePicture = user.ProfilePic;
            if (ProfilePicture == null)
            {
                ProfilePicture = "avatar.png";
            }

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

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);

            if (!user.EmailConfirmed)
            {
                StatusMessage = "Error, email not verified";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            if (!user.EmailConfirmed)
            {
                StatusMessage = "Error email not verified, can't change the information";
                await LoadAsync(user);
                return Page();
            }

            string uniqueFileName = UploadedFile(Input.ProfileImage);

            if (uniqueFileName != user.ProfilePic)
            {
                user.ProfilePic = uniqueFileName;
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

            if (Input.Name != user.Name)
            {
                user.Name = Input.Name;
            }

            if (Input.DOB != user.DOB)
            {
                user.DOB = Input.DOB;
            }

            await _userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        private string UploadedFile(IFormFile ProfileImage)
        {
            string uniqueFileName = null;

            if (ProfileImage != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + ProfileImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    ProfileImage.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
    }
}
