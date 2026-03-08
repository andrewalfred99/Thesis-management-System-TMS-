using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using TMS.Areas.Identity.Data;

namespace TMS.Data
{
    public class Request
    {
        public Guid Id { get; set; }

        public bool Approved { get; set; }

        public Student Student { get; set; }

        public Thesis Thesis { get; set; }
    }
}
