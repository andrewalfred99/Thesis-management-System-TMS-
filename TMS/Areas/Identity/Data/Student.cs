using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using TMS.Data;

namespace TMS.Areas.Identity.Data
{
    public class Student : PlatformUser
    {
        public Faculty Faculty { get; set; }
    }
}
