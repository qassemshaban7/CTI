using System.ComponentModel.DataAnnotations.Schema;

namespace CTI.Models
{
    public class Survey
    {
        public int Id { get; set; }
        public string SurveyName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ForWho { get; set; }  // 1-student  2- vistor  3- all

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public ICollection<Question> Questions { get; set; } 

    }
}