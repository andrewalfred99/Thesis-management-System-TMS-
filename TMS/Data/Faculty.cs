using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TMS.Data
{
    
    public class Faculty
    {
        public Guid Id { get; set; }

        [StringLength(50)]
        public String Name { get; set; }

        [DisplayFormat(NullDisplayText = "There are no Thesis")]
        public ICollection<Thesis> Thesis { get; set; }
    }
}
