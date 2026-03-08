using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
namespace TMS.Areas.Identity.Data
{
    public class PlatformUser : IdentityUser
    {
        [PersonalData]
        public string Name { get; set; }

        [PersonalData]
        public DateTime DOB { get; set; }

        [PersonalData]
        public string ProfilePic { get; set; }
    }
}
