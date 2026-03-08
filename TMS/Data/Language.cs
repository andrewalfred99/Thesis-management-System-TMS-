using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TMS.Data
{
    public class Language
    {
        public Guid Id { get; set; }

        [StringLength(2)]
        public String Code { get; set; }

        [StringLength(50)]
        public String Name { get; set; }
    }
}
