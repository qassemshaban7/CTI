
namespace CTI.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Requirments { get; set; }
        public string Duration { get; set; }
        public long Code { get; set; }

        public ICollection<ApplicationUserCourse> ApplicationUserCourses { get; set; }  
        public ICollection<Survey> Surveys { get; set; }
    }
}
