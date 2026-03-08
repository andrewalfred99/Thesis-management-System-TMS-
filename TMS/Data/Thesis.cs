using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using TMS.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMS.Data
{
    public enum status
    {
        Open,
        Inprogress,
        NeedRevision,
        MidtermRevision,
        FinalRevision,
        Closed
    }

    public enum Semester
    {
        First,
        Second
    }
    public class Thesis
    {
        public Guid Id { get; set; }

        [StringLength(50)]
        public String Name { get; set; }

        public String Description { get; set; }

        public String Materials { get; set; }

        public int Year { get; set; }

        public int ExpectedHours { get; set; }

        public Guid FacultyId { get; set; }

        public Faculty Faculty { get; set; }

        public Semester Semester { get; set; }

        public status Status { get; set; }

        [DisplayFormat(NullDisplayText = "There are no teachers")]
        public ICollection<PlatformUser> Teachers { get; set; }

        [DisplayFormat(NullDisplayText = "There are no students")]
        public ICollection<Student> Students { get; set; }

        [DisplayFormat(NullDisplayText = "There are no languages")]
        public ICollection<Language> Languages { get; set; }

        [DisplayFormat(NullDisplayText = "There are no notifications")]
        public ICollection<Notification> Notifications { get; set; }
    }
}
