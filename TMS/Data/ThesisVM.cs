using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMS.Areas.Identity.Data;
namespace TMS.Data
{
    public class ThesisVM
    {
        public ICollection<PlatformUser> Teachers { get; set; }

        public ICollection<Language> Languages { get; set; }

        public ICollection<Notification> Notifications { get; set; }
    }
}
