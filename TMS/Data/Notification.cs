using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using TMS.Areas.Identity.Data;

namespace TMS.Data
{
    public class Notification
    {
        public Guid Id { get; set; }

        [StringLength(50)]
        public String Title { get; set; }

        public String Description { get; set; }

        public DateTime DateTime { get; set; }
    }
}
