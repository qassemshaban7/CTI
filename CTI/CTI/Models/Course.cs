
using System.ComponentModel.DataAnnotations.Schema;

namespace CTI.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phase { get; set; }
        public string Specialization { get; set; }
        public long ReferenceNumber { get; set; }
        public long Coursecode { get; set; }
        public string Department { get; set; }
        public string TypeDivition { get; set; }


        public ICollection<ApplicationUserCourse> ApplicationUserCourses { get; set; }

        [ForeignKey("UserId")]
        public string UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }  
    }
}
