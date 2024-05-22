using System.ComponentModel.DataAnnotations;

namespace CTI.Models
{
    public class ApplicationUserCourse
    {
        public string UserId { get; set; }
        public int CourseId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }    
        public Course Course { get; set; }
    }
}
