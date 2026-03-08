using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.Data
{
    public class NotificationVM
    {
        public String Title { get; set; }

        public String Description { get; set; }

        public DateTime DateTime = DateTime.Now;
    }
}
