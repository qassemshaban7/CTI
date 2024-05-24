using CTI.Models;

namespace CTI.ViewModels
{
    public class SuperAdminHomeVM
    {
        public IEnumerable<ApplicationUser>? Trainer { get; set; }
        public IEnumerable<ApplicationUser>? Trainee { get; set; }
        public IEnumerable<Course>? Courses { get; set; }
        public IEnumerable<ApplicationUserCourse>? TraineeCourse { get; set; }
        public IEnumerable<ApplicationUserCourse>? TrainerCourse { get; set; }
        public IEnumerable<Survey>? Surveys { get; set; }
    }
}