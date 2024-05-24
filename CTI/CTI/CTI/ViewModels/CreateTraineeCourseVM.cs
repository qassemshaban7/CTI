using System.ComponentModel.DataAnnotations;

namespace CTI.ViewModels
{
    public class CreateTraineeCourseVM
    {
        public int CourseId { get; set; }
        public string Name { get; set; }
        [Required(ErrorMessage = "يجب اختيار مدرب واحد على الأقل")]
        public List<string> UserIds { get; set; }   
        
    }
}
