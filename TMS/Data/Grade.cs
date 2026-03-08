using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using TMS.Areas.Identity.Data;

namespace TMS.Data
{
    public class Grade
    {
        public Guid Id { get; set; }

        public int Midterm { get; set; }

        public int Exam { get; set; }

        public Student Student { get; set; }

        public Thesis Thesis { get; set; }
    }
}
