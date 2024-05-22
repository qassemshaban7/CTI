using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CTI.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string UserFullName { get; set; } = null!; // اسم المتدرب

        public long? TraineeID { get; set; } // رقم الهوية
        public string? Status { get; set; } // حالة المتدرب
        public string? TrainingProgram { get; set; }  // البرنامج التدريبي

        public string? Degree { get; set; }  // الدرجة العلمية

        public ICollection<ApplicationUserCourse> ApplicationUserCourses { get; set; }
        public ICollection<Survey> Surveys { get; set; }    
    }
}
